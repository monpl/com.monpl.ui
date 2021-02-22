using System.Collections;
using UnityEngine;

namespace Monpl.UI
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorPopup : PopupBase
    {
        [SerializeField] private bool haveInAnimation = true;

        protected string animationIn = "In"; 
        protected string animationReset = "Reset";

        protected Animator popupAnimator;

        public override void PreInit(RectTransform popupCanvasTrs, float dimmingTime)
        {
            base.PreInit(popupCanvasTrs, dimmingTime);
            popupAnimator = GetComponent<Animator>();
        }

        public override void ShowWill()
        {
            base.ShowWill();

            if (haveInAnimation)
            {
                popupAnimator.enabled = true;
                popupAnimator.Play(animationReset, -1, 0.0f);
            }
        }

        protected override void ShowDone()
        {
            base.ShowDone();

            if (haveInAnimation)
            {
                IsShown = false;
                popupAnimator.Play(animationIn, -1, 0.0f);
            }
        }

        protected virtual void OnInAnimationDone()
        {
            IsShown = true;
        }

        public override IEnumerator HidePopup(bool isTemporaryHide = false)
        {
            popupAnimator.enabled = false;
            yield return base.HidePopup(isTemporaryHide);
        }
    }
}