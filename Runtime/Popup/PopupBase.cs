using System.Collections;
using DG.Tweening;
using Monpl.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Monpl.UI
{
    public enum PopupShowTransitionType
    {
        None,
        ScaleUp,
        Up,
        Down,
        Left,
        Right,
        Custom,
    }
    public enum PopupHideTransitionType
    {
        None,
        ScaleDown,
        Up,
        Down,
        Left,
        Right,
        Custom,
    }
    
    public class PopupBase : MonoBehaviour
    {
        [SerializeField] private PopupShowTransitionType popupShowType; 
        [SerializeField] private PopupHideTransitionType popupHideType;
        [SerializeField] private Ease showEase = Ease.Linear;
        [SerializeField] private Ease hideEase = Ease.Linear;
        [SerializeField] private float startPopupAlpha = 0.5f;
        [SerializeField] private float showingTime = 0.3f;
        [SerializeField] private float hidingTime = 0.3f;
        [SerializeField] private bool dimmingHave = true;

        protected CanvasRootObject _popupRoot;
        protected Transform _popupTrs;
        protected Vector2 _oriLocalPos;
        
        private Image _dimmingImg;

        private RectTransform _popupCanvasTrs;
        private Vector2 _screenSize;
        private float _dimmingTime;
        
        public bool IsShown { get; protected set; }

        public virtual void PreInit(RectTransform popupCanvasTrs, float dimmingTime)
        {
            if (PreInitChildPopup() == false)
                return;

            if (dimmingHave)
            {
                if (PreInitDimming() == false)
                    return;
            }

            _popupCanvasTrs = popupCanvasTrs;
            _popupRoot.SetActiveCanvasGroup(false);
            _dimmingTime = dimmingTime; 
            IsShown = false;
            
            Invoke(nameof(GetScreenSize), 0.05f);
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

        private bool PreInitDimming()
        {
            var dimming = transform.Find("Dimming");
            if (dimming == null)
            {
                Debug.LogError($"Dimming is not exist! name: {name}");
                return false;
            }

            // TODO: Replace to GetOrAddComponent<Image>();
            _dimmingImg = dimming.GetComponent<Image>();

            if (_dimmingImg == null)
            {
                Debug.LogError($"Dimming's Image is not exist! name: {name}");
                return false;
            }

            _dimmingImg.enabled = false;
            
            return true;
        }

        private void GetScreenSize()
        {
            _screenSize = _popupCanvasTrs.sizeDelta;
        }

        public virtual IEnumerator ShowPopup(bool isReOpen = false)
        {
            transform.SetAsLastSibling();
            _popupTrs.DOKill();
            _popupTrs.Reset();
            _popupTrs.localPosition = _oriLocalPos;

            if (dimmingHave)
                _dimmingImg.enabled = true;

            switch (popupShowType)
            {
                case PopupShowTransitionType.ScaleUp:
                    _popupTrs.localScale = new Vector3(0.5f, 0.5f);
                    _popupRoot.SetAlpha(startPopupAlpha);
                    _popupRoot.SetActiveCanvasGroup(true, showingTime);

                    if (isReOpen)
                        _popupTrs.DOScale(1f, showingTime).SetEase(showEase);
                    else
                        yield return _popupTrs.DOScale(1f, showingTime).SetEase(showEase).WaitForCompletion();
                    break;

                case PopupShowTransitionType.Up:
                    _popupTrs.localPosition = new Vector3(0, -_screenSize.y, 0);
                    _popupRoot.SetAlpha(startPopupAlpha);
                    _popupRoot.SetActiveCanvasGroup(true, showingTime);

                    if (isReOpen)
                        _popupTrs.DOLocalMoveY(0f, showingTime).SetEase(showEase);
                    else
                        yield return _popupTrs.DOLocalMoveY(0f, showingTime).SetEase(showEase).WaitForCompletion();
                    break;

                case PopupShowTransitionType.Down:
                    _popupTrs.localPosition = new Vector3(0, _screenSize.y, 0);
                    _popupRoot.SetAlpha(startPopupAlpha);
                    _popupRoot.SetActiveCanvasGroup(true, showingTime);

                    if (isReOpen)
                        _popupTrs.DOLocalMoveY(0f, showingTime).SetEase(showEase);
                    else
                        yield return _popupTrs.DOLocalMoveY(0f, showingTime).SetEase(showEase).WaitForCompletion();
                    break;

                case PopupShowTransitionType.Left:
                    _popupTrs.localPosition = new Vector3(-_screenSize.x, 0, 0);
                    _popupRoot.SetAlpha(startPopupAlpha);
                    _popupRoot.SetActiveCanvasGroup(true, showingTime);

                    if (isReOpen)
                        _popupTrs.DOLocalMoveX(0f, showingTime).SetEase(showEase);
                    else
                        yield return _popupTrs.DOLocalMoveX(0f, showingTime).SetEase(showEase).WaitForCompletion();
                    break;

                case PopupShowTransitionType.Right:
                    _popupTrs.localPosition = new Vector3(_screenSize.x, 0, 0);
                    _popupRoot.SetAlpha(startPopupAlpha);
                    _popupRoot.SetActiveCanvasGroup(true, showingTime);

                    if (isReOpen)
                        _popupTrs.DOLocalMoveX(0f, showingTime).SetEase(showEase);
                    else
                        yield return _popupTrs.DOLocalMoveX(0f, showingTime).SetEase(showEase).WaitForCompletion();
                    break;
                case PopupShowTransitionType.Custom:
                    if (isReOpen)
                        ShowPopupCustom();
                    else
                        yield return ShowPopupCustom();
                    break;
                default:
                    _popupRoot.SetActiveCanvasGroup(true, showingTime);
                    
                    if(isReOpen)
                        yield return new WaitForSeconds(showingTime);
                    break;
            }

            if(isReOpen == false)
                ShowDone();
            else
            {
                SetGoodsArea();
                IsShown = true;
            }
        }

        protected virtual YieldInstruction ShowPopupCustom()
        {
            return (YieldInstruction) null;
        }
        
        protected virtual YieldInstruction HidePopupCustom()
        {
            return (YieldInstruction) null;
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

        public virtual IEnumerator HidePopup(bool isTemporaryHide = false)
        {
            _popupTrs.DOKill();

            if (dimmingHave)
                _dimmingImg.enabled = false;

            _popupRoot.SetActiveCanvasGroup(false, hidingTime);
            
            IsShown = false;
            if (isTemporaryHide == false)
            {
                switch (popupHideType)
                {
                    case PopupHideTransitionType.ScaleDown:
                        yield return _popupTrs.DOScale(0, hidingTime).SetEase(hideEase).WaitForCompletion();
                        break;
                    case PopupHideTransitionType.Up:
                        yield return _popupTrs.DOLocalMoveY(_screenSize.y, hidingTime).SetEase(hideEase).WaitForCompletion();
                        break;
                    case PopupHideTransitionType.Down:
                        yield return _popupTrs.DOLocalMoveY(-_screenSize.y, hidingTime).SetEase(hideEase).WaitForCompletion();
                        break;
                    case PopupHideTransitionType.Left:
                        yield return _popupTrs.DOLocalMoveX(-_screenSize.x, hidingTime).SetEase(hideEase).WaitForCompletion();
                        break;
                    case PopupHideTransitionType.Right:
                        yield return _popupTrs.DOLocalMoveX(_screenSize.x, hidingTime).SetEase(hideEase).WaitForCompletion();
                        break;
                    case PopupHideTransitionType.Custom:
                        yield return HidePopupCustom();
                        break;
                    default:
                        yield return new WaitForSeconds(hidingTime);
                        break;
                }
            }

            HideDone();
        }

        public void SetDimmingTween(bool enable, float time = 0.2f)
        {
            if(enable)
                _dimmingImg.enabled = true;

            _dimmingImg.DOKill();
            _dimmingImg.DOFade(enable ? 0.5f : 0f, time).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (enable == false)
                    _dimmingImg.enabled = false;
            });
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
            PopupManager.Instance.PopHidePopup();
        }
    }
}