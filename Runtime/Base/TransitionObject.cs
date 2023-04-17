using Cysharp.Threading.Tasks;
using DG.Tweening;
using Monpl.Utils.Extensions;
using UnityEngine;

namespace Monpl.UI
{
    public class TransitionObject : MonoBehaviour
    {
        [SerializeField] private UITransition showTransition;
        [SerializeField] private UITransition hideTransition;

        private Vector2 _oriLocalPos;
        private Vector2 _screenSize;
        private Transform _transitionTrs;
        private CanvasRootObject _alphaObject;

        public void PreInit(Transform transitionTrs, Vector2 oriLocalPos, Vector2 screenSize, CanvasRootObject alphaObject = null)
        {
            _transitionTrs = transitionTrs;
            _oriLocalPos = oriLocalPos;
            _screenSize = screenSize;
            _alphaObject = alphaObject;
        }

        public async UniTask ShowTransition()
        {
            ResetPosition();

            if (showTransition != null)
                await showTransition.Play(_transitionTrs, _screenSize, _alphaObject);
        }

        public async UniTask HideTransition()
        {
            _transitionTrs.DOKill();

            if (hideTransition != null)
                await hideTransition.Play(_transitionTrs, _screenSize, _alphaObject);
        }

        public void ResetPosition()
        {
            _transitionTrs.DOKill();
            _transitionTrs.Reset();
            _transitionTrs.localPosition = _oriLocalPos;
        }
    }
}