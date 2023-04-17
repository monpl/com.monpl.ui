using Cysharp.Threading.Tasks;
using Monpl.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Monpl.UI
{
    [RequireComponent(typeof(TransitionObject))]
    [RequireComponent(typeof(CanvasRootObject))]
    [RequireComponent(typeof(CanvasScaler))]
    public class ScreenBase : MonoBehaviour
    {
        protected TransitionObject _transitionObject;
        protected CanvasRootObject _canvasRootObject;

        public void PreInit()
        {
            _canvasRootObject = GetComponent<CanvasRootObject>();
            _transitionObject = GetComponent<TransitionObject>();

            GetComponent<CanvasScaler>().matchWidthOrHeight = DeviceUtil.GetScaleMatch();

            var screenAnimTrs = transform.GetChild(0);

            _canvasRootObject.PreInit(false);
            _transitionObject.PreInit(screenAnimTrs, screenAnimTrs.localPosition, GetScreenSize(), _canvasRootObject);
        }

        public virtual void ShowWill()
        {
        }

        public virtual void ShowDone()
        {
        }

        public virtual void DismissWill()
        {
        }

        public virtual void DismissDone()
        {
        }

        public virtual async UniTask ShowRoutine()
        {
            await _transitionObject.ShowTransition();
        }

        public virtual async UniTask DismissRoutine()
        {
            await _transitionObject.HideTransition();
        }

        public virtual void OnPressedBackKey()
        {
        }

        private Vector2 GetScreenSize()
        {
            return transform.GetComponent<RectTransform>().sizeDelta;
        }
    }
}