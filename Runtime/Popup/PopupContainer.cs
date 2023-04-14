using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Monpl.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Monpl.UI
{
    public class PopupActionData
    {
        public PopupAction actionType;
        public string popupName;
        public float delay;
    }

    public enum PopupAction
    {
        Show,
        ShowOverlap,
        PopHide,
        HideAll,
    }

    public class PopupContainer : MonoBehaviour
    {
        [SerializeField] private float dimmingAlpha = 0.6f;

        private RectTransform _popupRoot;

        public Dictionary<string, PopupBase> PopupDic { get; private set; }
        public Queue<PopupActionData> waitingPopupQueue { get; private set; }

        private Stack<string> _popupStack; // 기본 팝업 스택
        private Stack<string> _overlapPopupStack; // 팝업 위에 뜨는 팝업 스택

        private Queue<PopupActionData> _popupActions;

        private bool _isTransitioning;
        private Action<List<string>> _popupChangedAction;

        public async UniTask PreInit()
        {
            PopupDic = new Dictionary<string, PopupBase>();
            _popupStack = new Stack<string>();
            _overlapPopupStack = new Stack<string>();

            waitingPopupQueue = new Queue<PopupActionData>();
            _popupActions = new Queue<PopupActionData>();
            _popupChangedAction = null;
            _popupRoot = GetComponent<RectTransform>();

            await LoadPopups();
            PopupRoutine().Forget();
        }

        #region Public Methods

        public void ShowPopup(string showingPopup, bool isOverlap = false, float delay = 0.0f)
        {
            AddPopupAction(isOverlap ? PopupAction.ShowOverlap : PopupAction.Show, showingPopup, delay);
        }

        public T ShowPopup<T>(bool isOverlap = false, float delay = 0.0f) where T : PopupBase
        {
            var popupName = typeof(T).Name;

            ShowPopup(popupName, isOverlap, delay);
            return (T)PopupDic[popupName];
        }

        public void PopHidePopup(float delay = 0.0f)
        {
            AddPopupAction(PopupAction.PopHide, "", delay);
        }

        public void HideAllPopup(bool isIgnoreTransition = false)
        {
            AddPopupAction(PopupAction.HideAll, "", 0f, isIgnoreTransition);
        }

        public void AddWaitingPopupQueue(string waitPopupName, float delay = 0.0f)
        {
            waitingPopupQueue.Enqueue(new PopupActionData
            {
                actionType = PopupAction.Show,
                popupName = waitPopupName,
                delay = delay
            });
        }

        public T AddWaitingPopupQueue<T>(float delay = 0f) where T : PopupBase
        {
            var popupName = typeof(T).Name;
            AddWaitingPopupQueue(popupName, delay);
            return (T)PopupDic[popupName];
        }

        public void AddPopupChangeAction(Action<List<string>> action)
        {
            _popupChangedAction += action;
        }

        public void RemoveAllChangeAction()
        {
            _popupChangedAction = null;
        }

        #endregion

        #region Private Methods

        private async UniTask LoadPopups()
        {
            _popupRoot.GetComponent<CanvasScaler>().matchWidthOrHeight = DeviceUtil.GetScaleMatch();

            await Addressables.LoadAssetsAsync<GameObject>("popup", popup =>
            {
                if (popup == null)
                    return;

                PopupDic.Add(popup.name, CreateNewPopup(popup));
            }).ToUniTask();
        }

        private void AddPopupAction(PopupAction actionType, string popupName, float delay = 0.0f, bool isIgnoreTransition = false)
        {
            if (_isTransitioning && !isIgnoreTransition)
                return;

            _popupActions.Enqueue(new PopupActionData
            {
                actionType = actionType,
                popupName = popupName,
                delay = delay
            });
        }

        private async UniTask PopupRoutine()
        {
            while (true)
            {
                if (_popupActions.Count == 0)
                {
                    if (_popupStack.Count == 0 && _overlapPopupStack.Count == 0 && waitingPopupQueue.Count > 0)
                        PopWaitingQueue();

                    await UniTask.Yield();
                    continue;
                }

                var action = _popupActions.Dequeue();
                var popupName = action.popupName;
                _isTransitioning = true;

                await UniTask.Delay(TimeSpan.FromSeconds(action.delay));

                switch (action.actionType)
                {
                    case PopupAction.Show:
                        await ShowPopupRoutine(popupName, true);
                        _popupStack.Push(popupName);
                        break;
                    case PopupAction.ShowOverlap:
                        await ShowPopupRoutine(popupName);
                        _overlapPopupStack.Push(popupName);
                        break;
                    case PopupAction.HideAll:
                        var taskList = new List<UniTask>();
                        while (_overlapPopupStack.Count > 0 || _popupStack.Count > 0)
                        {
                            var (hidePopupName2, isOverlap2) = GetStackedPopupOrEmpty();
                            taskList.Add(HidePopupRoutine(hidePopupName2, isOverlap2));
                        }

                        await UniTask.WhenAll(taskList);
                        break;
                    case PopupAction.PopHide:
                        var (hidePopupName, isOverlap) = GetStackedPopupOrEmpty();

                        if (string.IsNullOrEmpty(hidePopupName))
                            break;

                        await HidePopupRoutine(hidePopupName, isOverlap);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await UniTask.Yield();
                _isTransitioning = false;
            }
        }

        private (string, bool) GetStackedPopupOrEmpty()
        {
            if (_overlapPopupStack.Count > 0)
                return (_overlapPopupStack.Pop(), true);

            if (_popupStack.Count > 0)
                return (_popupStack.Pop(), false);

            return (string.Empty, false);
        }

        private void PopWaitingQueue()
        {
            if (waitingPopupQueue.Count == 0)
                return;

            var waitingPopupData = waitingPopupQueue.Dequeue();
            AddPopupAction(waitingPopupData.actionType, waitingPopupData.popupName, waitingPopupData.delay);
        }

        private async UniTask ShowPopupRoutine(string showPopupName, bool isChangePopup = false)
        {
            var curPopup = PopupDic[showPopupName];
            var hideTask = UniTask.CompletedTask;

            curPopup.ShowWill();

            if (_popupStack.Count >= 1 && isChangePopup)
            {
                var hidePopupName = _popupStack.Peek();
                hideTask = PopupDic[hidePopupName].HidePopup(true);
            }

            await UniTask.WhenAll(curPopup.ShowPopup(), hideTask);
        }

        private async UniTask HidePopupRoutine(string hidingPopupName, bool isOverlap)
        {
            var hidingPopup = PopupDic[hidingPopupName];

            hidingPopup.HideWill();
            var hideTask = hidingPopup.HidePopup();
            var lastShowTask = UniTask.CompletedTask;

            if (!isOverlap)
            {
                if (_popupStack.TryPeek(out var lastPopupName))
                    lastShowTask = PopupDic[lastPopupName].ShowPopup(true);
            }

            await UniTask.WhenAll(hideTask, lastShowTask);
        }

        private PopupBase CreateNewPopup(GameObject popupObject)
        {
            var newPopupObject = Instantiate(popupObject, _popupRoot, false);
            var newPopup = newPopupObject.GetComponent<PopupBase>();
            newPopup.PreInit();

            if (newPopup.IsHaveDimming())
                newPopup.DimmingImage = GetNewDimmingImage(newPopupObject.transform);

            return newPopup;
        }

        private DimmingImage GetNewDimmingImage(Transform parentTrs)
        {
            var newDimming = new GameObject("Dimming").AddComponent<DimmingImage>();
            newDimming.transform.parent = parentTrs;
            newDimming.PreInit(dimmingAlpha);

            return newDimming;
        }

        #endregion
    }
}