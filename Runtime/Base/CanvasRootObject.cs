using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Monpl.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasRootObject : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;
        [SerializeField] private bool _isNotChangeGroupSetting;

        public virtual void PreInit()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void SetAlpha(float alpha)
        {
            canvasGroup.alpha = alpha;
        }

        public void SetActiveCanvasGroup(bool enable, float fadeTime = 0.0f)
        {
            if (canvasGroup == null)
                PreInit();

            canvasGroup.DOKill();

            if (_isNotChangeGroupSetting == false)
                canvasGroup.interactable = enable;

            if (fadeTime > 0f)
                canvasGroup.DOFade(enable ? 1f : 0f, fadeTime);
            else
                canvasGroup.alpha = enable ? 1f : 0f;

            if (_isNotChangeGroupSetting == false)
                canvasGroup.blocksRaycasts = enable;
        }

        public IEnumerator SetActiveCanvasGroupCo(bool enable, float fadeTime = 0.0f)
        {
            if (canvasGroup == null)
                PreInit();

            canvasGroup.DOKill();

            if (_isNotChangeGroupSetting == false)
            {
                canvasGroup.interactable = enable;
                canvasGroup.blocksRaycasts = enable;
            }

            if (fadeTime > 0f)
                yield return canvasGroup.DOFade(enable ? 1f : 0f, fadeTime).WaitForCompletion();
            else
                canvasGroup.alpha = enable ? 1f : 0f;
            yield break;
        }
    }
}