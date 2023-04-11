using System;
using System.Collections;
using System.Collections.Generic;
using Monpl.Utils;
using Monpl.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Monpl.UI
{
    public class PopupManagerSettings
    {
        public string[] popupNames;
        public bool isOnlyOnePopup;
        public float dimmingTime = 0.1f;
        public bool isPortrait;
        public Action<List<string>> onChangedPopupList;
    }

    public class PopupActionData
    {
        public PopupAction actionType;
        public string popupName;
        public float delay;
    }

    public enum PopupAction
    {
        Show,
        Hide,
        PopHide,
    }

    public class PopupContainer : MonoBehaviour
    {
        private RectTransform _popupRoot;
        private PopupManagerSettings _settings;

        public Dictionary<string, PopupBase> PopupDic { get; private set; }
        public List<string> showingPopupList { get; private set; }
        public Queue<PopupActionData> waitingPopupQueue { get; private set; }

        // Action을 받으면 코루틴을 돌려 큐로 돌리기로!
        private Queue<PopupActionData> _popupActions;
        private Action<List<string>> _popupChangedAction;

        public void PreInit()
        {
            // _popupRoot = popupRoot;
            // _settings = settings;

            PopupDic = new Dictionary<string, PopupBase>();
            showingPopupList = new List<string>();
            waitingPopupQueue = new Queue<PopupActionData>();
            _popupActions = new Queue<PopupActionData>();
            // _popupChangedAction = settings.onChangedPopupList;

            // FindPopups(popupResRoot);

            StopAllCoroutines();
            StartCoroutine(PopupRoutine());
        }

        private void FindPopups(string popupResRoot)
        {
            if (_settings.popupNames == null)
                return;

            foreach (var popupName in _settings.popupNames)
            {
                var popup = Resources.Load<PopupBase>($"{popupResRoot}/{popupName}");

                if (popup == null)
                    continue;

                var newPopup = Instantiate(popup, _popupRoot, false);
                newPopup.gameObject.SetActive(true);
                newPopup.PreInit(_popupRoot, _settings.dimmingTime);

                PopupDic.Add(popup.name, newPopup);
            }

            _popupRoot.GetComponent<CanvasScaler>().matchWidthOrHeight = DeviceUtil.GetScaleMatch();
        }

        public void AddWaitingPopupQueue(string waitPopupName, float delay = 0.0f)
        {
            waitingPopupQueue.Enqueue(new PopupActionData
                {actionType = PopupAction.Show, delay = delay, popupName = waitPopupName});
        }

        public T AddWaitingPopupQueue<T>(float delay = 0f) where T : PopupBase
        {
            var popupName = typeof(T).Name;
            AddWaitingPopupQueue(popupName, delay);
            return (T) PopupDic[popupName];
        }

        private void PopWaitingQueue()
        {
            if (waitingPopupQueue.Count == 0)
                return;

            var waitingPopupData = waitingPopupQueue.Dequeue();
            AddPopupAction(waitingPopupData.actionType, waitingPopupData.popupName, waitingPopupData.delay);
        }

        public void ShowPopup(string showingPopup, float delay = 0.0f)
        {
            AddPopupAction(PopupAction.Show, showingPopup, delay);
        }

        public T ShowPopup<T>(float delay = 0.0f) where T : PopupBase
        {
            var popupName = typeof(T).Name;
            
            ShowPopup(popupName, delay);
            return (T)PopupDic[popupName];
        }

        public void PopHidePopup(float delay = 0.0f)
        {
            AddPopupAction(PopupAction.PopHide, "", delay);
        }

        public void HidePopup(string hidingPopup, float delay = 0.0f)
        {
            AddPopupAction(PopupAction.Hide, hidingPopup, delay);
        }

        private void AddPopupAction(PopupAction actionType, string popupName, float delay = 0.0f)
        {
            _popupActions.Enqueue(new PopupActionData
            {
                actionType = actionType,
                popupName = popupName,
                delay = delay
            });
        }

        private IEnumerator PopupRoutine()
        {
            while (true)
            {
                if (_popupActions.Count == 0)
                {
                    if (showingPopupList.Count == 0 && waitingPopupQueue.Count > 0)
                        PopWaitingQueue();

                    yield return null;
                    continue;
                }

                var action = _popupActions.Dequeue();
                var popupName = action.popupName;

                yield return new WaitForSeconds(action.delay);

                switch (action.actionType)
                {
                    case PopupAction.Show:
                        // TODO: 대기 팝업인 경우엔 중복검사xx
                        if (showingPopupList.Contains(popupName))
                        {
                            Debug.Log($"Popup is overlap.. popup: {popupName}");
                            break;
                        }

                        yield return ShowPopupRoutine(popupName);
                        AddPopupListInList(popupName);
                        break;
                    case PopupAction.Hide:
                        if (showingPopupList.Contains(popupName) == false)
                        {
                            Debug.Log($"It Is not have in ShowingPopupList / name: {action.actionType}");
                            break;
                        }

                        RemovePopupInList(popupName);
                        yield return HidePopupRoutine(popupName);
                        break;
                    case PopupAction.PopHide:
                        if (showingPopupList.Count == 0)
                            break;

                        var lastPopupName = RemoveLastPopupInList();
                        yield return HidePopupRoutine(lastPopupName);
                        break;
                    default:
                        break;
                }

                yield return null;
            }
        }

        private IEnumerator ShowPopupRoutine(string showPopupName)
        {
            var curPopup = PopupDic[showPopupName];

            curPopup.ShowWill();

            if (_settings.isOnlyOnePopup && showingPopupList.Count >= 1)
            {
                var hidePopupName = showingPopupList.GetLast();
                StartCoroutine(PopupDic[hidePopupName].HidePopup(true));
            }

            yield return curPopup.ShowPopup();

            while (curPopup.IsShown == false)
                yield return null;
        }

        private IEnumerator HidePopupRoutine(string hidingPopupName)
        {
            var hidingPopup = PopupDic[hidingPopupName];

            // TODO: Show Only one Popup 처리
            if (_settings.isOnlyOnePopup && showingPopupList.Count >= 1)
            {
                if (PopupDic[hidingPopupName].IsShown && _popupActions.Count == 0)
                {
                    var prevPopupName = showingPopupList.GetLast();
                    StartCoroutine(PopupDic[prevPopupName].ShowPopup(true));
                }
            }

            hidingPopup.HideWill();

            yield return hidingPopup.HidePopup();
        }

        private void AddPopupListInList(string popupName)
        {
            showingPopupList.Add(popupName);
            _popupChangedAction?.Invoke(showingPopupList);
        }

        private void RemovePopupInList(string popupName)
        {
            showingPopupList.Remove(popupName);
            _popupChangedAction?.Invoke(showingPopupList);
        }

        private string RemoveLastPopupInList()
        {
            var ret = showingPopupList.GetLastAndRemove();
            _popupChangedAction?.Invoke(showingPopupList);
            
            return ret;
        }
    }
}