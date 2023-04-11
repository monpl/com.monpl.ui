using System;
using Cysharp.Threading.Tasks;
using Monpl.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Monpl.UI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class ScreenBase : CanvasRootObject
    {
        public override void PreInit()
        {
            base.PreInit();
            GetComponent<CanvasScaler>().matchWidthOrHeight = DeviceUtil.GetScaleMatch();
            SetActiveCanvasGroup(false);
        }

        public virtual void ShowWill() { }

        public virtual void ShowDone() { }

        public virtual void DismissWill() { }

        public virtual void DismissDone() { }

        public virtual async UniTask ShowRoutine(float fadingTime = 0.1f)
        {
            SetActiveCanvasGroup(true, fadingTime);
            await UniTask.Delay(TimeSpan.FromSeconds(fadingTime));
        }

        public virtual async UniTask DismissRoutine(float fadingTime = 0.1f)
        {
            SetActiveCanvasGroup(false, fadingTime);
            await UniTask.Delay(TimeSpan.FromSeconds(fadingTime));
        }

        public virtual void OnPressedBackKey() { }
    }
}