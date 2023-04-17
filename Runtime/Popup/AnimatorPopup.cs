using System.Collections;
using Cysharp.Threading.Tasks;
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

        public override void PreInit()
        {
            base.PreInit();
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

        public override void ShowDone()
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

        public override async UniTask HidePopup(bool isTemporaryHide = false)
        {
            popupAnimator.enabled = false;
            await base.HidePopup(isTemporaryHide);
        }
    }
}