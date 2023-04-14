using Monpl.Utils.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Monpl.UI
{
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(Image))]
    public class DimmingImage : MonoBehaviour
    {
        private Image _image;
        private RectTransform _rectTrs;

        public void PreInit(float dimmingAlpha)
        {
            _rectTrs = GetComponent<RectTransform>();
            _image = GetComponent<Image>();

            _rectTrs.SetStretchAll();
            _rectTrs.SetAsFirstSibling();

            _image.color = new Color(0, 0, 0, dimmingAlpha);

            SetEnable(false);
        }

        public void SetEnable(bool isEnable)
        {
            _image.enabled = isEnable;
        }
    }
}