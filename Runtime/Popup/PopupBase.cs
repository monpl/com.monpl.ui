using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Monpl.UI
{
    [RequireComponent(typeof(TransitionObject))]
    public class PopupBase : MonoBehaviour
    {
        [SerializeField] private bool isHaveDimming = true;

        protected CanvasRootObject _popupRootObject;
        protected TransitionObject _transitionObject;
        protected Transform _popupTrs;

        public bool IsShown { get; protected set; }
        public DimmingImage DimmingImage { get; set; }

        public bool IsHaveDimming() => isHaveDimming;

        public virtual void PreInit()
        {
            if (IsValidPopup() == false)
                return;

            IsShown = false;

            _transitionObject = GetComponent<TransitionObject>();
            _popupRootObject = _popupTrs.GetComponent<CanvasRootObject>();

            _popupRootObject.PreInit(false);
            _transitionObject.PreInit(_popupTrs, _popupTrs.localPosition, GetScreenSize(), _popupRootObject);
        }

        public virtual async UniTask ShowPopup(bool isReOpen = false)
        {
            transform.SetAsLastSibling();

            if (isHaveDimming)
                DimmingImage.SetEnable(true);

            await _transitionObject.ShowTransition();

            if (isReOpen)
            {
                SetGoodsArea();
                IsShown = true;
            }
        }

        public virtual async UniTask HidePopup(bool isTemporaryHide = false)
        {
            if (isHaveDimming)
                DimmingImage.SetEnable(false);

            if (!isTemporaryHide)
                await _transitionObject.HideTransition();
            else
                await _popupRootObject.SetActiveCanvasGroupTask(false);
        }

        public virtual void ShowWill()
        {
            IsShown = false;
        }

        public virtual void ShowDone()
        {
            IsShown = true;
        }

        public virtual void HideWill()
        {
        }

        public virtual void HideDone()
        {
            IsShown = false;
        }

        public virtual void SetGoodsArea()
        {
        }

        public virtual void OnPressedBackKey()
        {
            UIManager.PopupContainer.PopHidePopup();
        }

        private bool IsValidPopup()
        {
            _popupTrs = transform.Find("Popup");

            if (_popupTrs == null)
            {
                Debug.LogError($"Popup is not exist! name: {name}");
                return false;
            }

            if (!_popupTrs.TryGetComponent(out _popupRootObject))
            {
                Debug.LogError($"Popup is not have CanvasRootObject.. name: {name}");
                return false;
            }

            return true;
        }

        private Vector2 GetScreenSize()
        {
            return transform.parent.GetComponent<RectTransform>().sizeDelta;
        }
    }
}