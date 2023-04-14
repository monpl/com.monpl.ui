using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Monpl.UI
{
    public enum TransitionType
    {
        Scale,
        Up,
        Down,
        Left,
        Right,
    }

    [CreateAssetMenu(menuName = "UI/Transition", fileName = "PopupTransition")]
    public class PopupTransitionData : ScriptableObject
    {
        [SerializeField] private bool isInAnimation;
        [SerializeField] private float delay;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private Ease ease = Ease.OutQuad;
        [SerializeField] private TransitionType transitionType;
        [SerializeField] private float alphaDuration = 0.1f;

        public async UniTask Play(Transform trs, CanvasRootObject popupRoot)
        {
            var screenSize = trs.parent.parent.GetComponent<RectTransform>().sizeDelta;
            
            popupRoot.SetActiveCanvasGroup(isInAnimation, alphaDuration);

            switch (transitionType)
            {
                case TransitionType.Scale:
                    trs.localScale = isInAnimation ? new Vector3(0f, 0f) : trs.localScale;
                    await trs.DOScale(isInAnimation ? 1f : 0f, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                case TransitionType.Up:
                    trs.localPosition = isInAnimation ? new Vector3(0, screenSize.y, 0) : trs.localPosition;
                    await trs.DOLocalMoveY(isInAnimation ? 0f : screenSize.y, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                case TransitionType.Down:
                    trs.localPosition = isInAnimation ? new Vector3(0, -screenSize.y, 0) : trs.localPosition;
                    await trs.DOLocalMoveY(isInAnimation ? 0f : -screenSize.y, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                case TransitionType.Left:
                    trs.localPosition = isInAnimation ? new Vector3(-screenSize.x, 0, 0) : trs.localPosition;
                    await trs.DOLocalMoveX(isInAnimation ? 0f : -screenSize.x, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                case TransitionType.Right:
                    trs.localPosition = isInAnimation ? new Vector3(screenSize.x, 0, 0) : trs.localPosition;
                    await trs.DOLocalMoveX(isInAnimation ? 0f : screenSize.x, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}