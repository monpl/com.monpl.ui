using DG.Tweening;
using Monpl.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Monpl.UI
{
    public enum AnimationType
    {
        NonTween,
        Tween,
        Jelly,
    }

    public class ScaleButton : ButtonAniBase
    {
        [SerializeField] private float pressingScale = 0.9f;
        [SerializeField] private AnimationType animationType = AnimationType.NonTween;

        public RectTransform AnimationObject { get { Init(); return _animationObject; } }
        public float PressingScale
        {
            get => pressingScale;
            set => pressingScale = value;
        }

        private Button _button;
        private RectTransform _animationObject;

        private void Init()
        {
            _animationObject = _animationObject ? _animationObject : transform.FindRecursively("AnimationObject") as RectTransform;
            _button = GetComponent<Button>();
        }

        protected override void PlayButtonAni(bool isDown)
        {
            base.PlayButtonAni(isDown);

            AnimationObject.DOComplete();
            
            if(!_button.interactable) 
                return;

            if (isDown)
            {
                switch (animationType)
                {
                    case AnimationType.NonTween:
                        AnimationObject.DOScale(pressingScale, 0.0f);
                        break;
                    case AnimationType.Tween:
                        AnimationObject.DOScale(pressingScale, 0.2f);
                        break;
                    case AnimationType.Jelly:
                        AnimationObject.DOScale(pressingScale, 0.2f);
                        break;
                }
            }
            else
            {
                switch (animationType)
                {
                    case AnimationType.NonTween:
                        AnimationObject.DOScale(1f, 0.0f);
                        break;
                    case AnimationType.Tween:
                        AnimationObject.DOScale(1f, 0.2f);
                        break;
                    case AnimationType.Jelly:
                        AnimationObject.localScale = Vector3.one;
                        AnimationObject.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.4f, 10, 5);
                        break;
                }
            }
        }
    }
}