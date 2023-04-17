using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Monpl.UI
{
    public enum UITransitionType
    {
        None,
        Scale,
        Up,
        Down,
        Left,
        Right,
    }

    [CreateAssetMenu(menuName = "UI/UITransition", fileName = "UITransition")]
    public class UITransition : ScriptableObject
    {
        [SerializeField] private bool isInAnimation;
        [SerializeField] private float delay;
        [SerializeField] private float duration = 0.3f;
        [SerializeField] private float alphaDuration = 0.3f;
        [SerializeField] private Ease ease = Ease.OutQuad;
        [SerializeField] private UITransitionType uiTransitionType;

        public async UniTask Play(Transform trs, Vector2 screenSize, CanvasRootObject alphaObject)
        {
            var alphaTask = alphaObject.SetActiveCanvasGroupTask(isInAnimation, alphaDuration);
            var trsTask = UniTask.CompletedTask;

            switch (uiTransitionType)
            {
                case UITransitionType.None:
                    break;
                case UITransitionType.Scale:
                    trs.localScale = isInAnimation ? new Vector3(0f, 0f) : trs.localScale;
                    trsTask = trs.DOScale(isInAnimation ? 1f : 0f, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                case UITransitionType.Up:
                    trs.localPosition = isInAnimation ? new Vector3(0, screenSize.y, 0) : trs.localPosition;
                    trsTask = trs.DOLocalMoveY(isInAnimation ? 0f : screenSize.y, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                case UITransitionType.Down:
                    trs.localPosition = isInAnimation ? new Vector3(0, -screenSize.y, 0) : trs.localPosition;
                    trsTask = trs.DOLocalMoveY(isInAnimation ? 0f : -screenSize.y, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                case UITransitionType.Left:
                    trs.localPosition = isInAnimation ? new Vector3(-screenSize.x, 0, 0) : trs.localPosition;
                    trsTask = trs.DOLocalMoveX(isInAnimation ? 0f : -screenSize.x, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                case UITransitionType.Right:
                    trs.localPosition = isInAnimation ? new Vector3(screenSize.x, 0, 0) : trs.localPosition;
                    trsTask = trs.DOLocalMoveX(isInAnimation ? 0f : screenSize.x, duration).SetDelay(delay).SetEase(ease).ToUniTask();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await UniTask.WhenAll(alphaTask, trsTask);
        }
    }
}