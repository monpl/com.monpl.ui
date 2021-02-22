using System.Collections;
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

        public virtual void SetGoodsArea() { }

        public virtual void ShowWill() { }

        public virtual void ShowDone() { }

        public virtual void DismissWill() { }

        public virtual void DismissDone() { }

        public virtual IEnumerator ShowRoutine(float fadingTime = 0.1f)
        {
            SetActiveCanvasGroup(true, fadingTime);
            yield return new WaitForSeconds(fadingTime);
        }

        public virtual IEnumerator DismissRoutine(float fadingTime = 0.1f)
        {
            SetActiveCanvasGroup(false, fadingTime);
            yield return new WaitForSeconds(fadingTime);
        }

        public virtual void OnPressedBackKey() { }
    }
}