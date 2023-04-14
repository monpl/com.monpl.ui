using Cysharp.Threading.Tasks;
using DG.Tweening;
using Monpl.Utils.Extensions;
using UnityEngine;

namespace Monpl.UI
{
    public class PopupBase : MonoBehaviour
    {
        [SerializeField] private PopupTransitionData showTransition;
        [SerializeField] private PopupTransitionData hideTransition;
        [SerializeField] private bool isHaveDimming = true;

        protected CanvasRootObject _popupRoot;
        protected Transform _popupTrs;
        protected Vector2 _oriLocalPos;

        public bool IsShown { get; protected set; }
        public DimmingImage DimmingImage { get; set; }

        public bool IsHaveDimming() => isHaveDimming;

        public virtual void PreInit()
        {
            if (PreInitChildPopup() == false)
                return;

            IsShown = false;

            gameObject.SetActive(true);
            _popupRoot.SetActiveCanvasGroup(false);
        }

        private bool PreInitChildPopup()
        {
            var popup = transform.Find("Popup");

            if (popup == null)
            {
                Debug.LogError($"Popup is not exist! name: {name}");
                return false;
            }

            _popupRoot = popup.GetComponent<CanvasRootObject>();
            _oriLocalPos = popup.transform.localPosition;

            if (_popupRoot == null)
            {
                Debug.LogError($"Popup is not have CanvasRootObject.. name: {name}");
                return false;
            }

            _popupTrs = _popupRoot.transform;
            return true;
        }

        public virtual async UniTask ShowPopup(bool isReOpen = false)
        {
            transform.SetAsLastSibling();

            if (isHaveDimming)
                DimmingImage.SetEnable(true);

            _popupTrs.DOKill();
            _popupTrs.Reset();
            _popupTrs.localPosition = _oriLocalPos;

            if (showTransition != null)
                await showTransition.Play(_popupTrs, _popupRoot);
            else
                _popupRoot.SetActiveCanvasGroup(true);

            if (isReOpen == false)
                ShowDone();
            else
            {
                SetGoodsArea();
                IsShown = true;
            }
        }

        protected virtual UniTask ShowPopupCustom()
        {
            return UniTask.Yield().ToUniTask();
        }

        protected virtual UniTask HidePopupCustom()
        {
            return UniTask.Yield().ToUniTask();
        }

        public virtual void SetGoodsArea()
        {
        }

        public virtual void ShowWill()
        {
            IsShown = false;
        }

        protected virtual void ShowDone()
        {
            IsShown = true;
        }

        public virtual async UniTask HidePopup(bool isTemporaryHide = false)
        {
            _popupTrs.DOKill();

            if (isHaveDimming)
                DimmingImage.SetEnable(false);

            if (!isTemporaryHide)
            {
                if (hideTransition != null)
                    await hideTransition.Play(_popupTrs, _popupRoot);
            }

            _popupRoot.SetActiveCanvasGroup(false);
            IsShown = false;

            HideDone();
        }

        public virtual void HideWill()
        {
        }

        private void HideDone()
        {
            IsShown = false;
        }

        public virtual void OnPressedBackKey()
        {
            UIManager.PopupContainer.PopHidePopup();
        }
    }
}