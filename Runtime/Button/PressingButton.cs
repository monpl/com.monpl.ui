using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Monpl.UI
{
    public class PressingButton : ButtonAniBase
    {
        [SerializeField] private int pressingDown = 30;
        [SerializeField] private Sprite buttonSprite;
        [SerializeField] private Color shadowColor = new Color(0, 0, 0, 0.5f);
        [SerializeField] private bool isSmooth = true;

        public int PressingDown
        {
            get => pressingDown;
            set
            {
                if (pressingDown == value) 
                    return;
                
                pressingDown = value;
                AcceptImages(false);
            }
        }

        public Sprite ButtonSprite
        {
            get => buttonSprite;
            set
            {
                if (buttonSprite == value) 
                    return;
                    
                buttonSprite = value;
                AcceptImages(false);
            }
        }

        public Color ShadowColor
        {
            get => shadowColor;
            set
            {
                if (shadowColor == value) 
                    return;
                
                shadowColor = value;
                AcceptImages(false);
            }
        }

        public RectTransform MainTrs { get { Init(); return _mainTrs; } }
        public RectTransform ShadowTrs { get { Init(); return _shadowTrs; } }
        public Image MainImage { get { Init(); return _mainImage; } }
        public Image ShadowImage { get { Init(); return _shadowImage; } }

        private RectTransform _mainTrs;
        private RectTransform _shadowTrs;
        private Image _mainImage;
        private Image _shadowImage;

        private void Init()
        {
            _mainTrs = _mainTrs ? _mainTrs : transform.Find("MainImage").GetComponent<RectTransform>();
            _shadowTrs = _shadowTrs ? _shadowTrs : transform.Find("ShadowImage").GetComponent<RectTransform>();

            _mainImage = _mainImage ? _mainImage : _mainTrs.GetComponent<Image>();
            _shadowImage = _shadowImage ? _shadowImage : _shadowTrs.GetComponent<Image>();
            _shadowImage.color = ShadowColor;
            
            _mainTrs.SetAsLastSibling();
        }

        public void AcceptImages(bool isSetNative)
        {
            if (buttonSprite == null)
            {
                Debug.Log("Please Insert Sprite..!");
                return;
            }
            
            var rectTrs = GetComponent<RectTransform>();
            var border = buttonSprite.border;
            var isSliced = border.x > 0 || border.y > 0 || border.z > 0 || border.w > 0;

            MainImage.sprite = buttonSprite;
            ShadowImage.sprite = buttonSprite;

            MainImage.type = isSliced ? Image.Type.Sliced : Image.Type.Simple;
            ShadowImage.type = isSliced ? Image.Type.Sliced : Image.Type.Simple;

            if (isSliced)
            {
                ShadowTrs.sizeDelta = MainTrs.sizeDelta;

                MainImage.rectTransform.sizeDelta = rectTrs.sizeDelta;
                ShadowImage.rectTransform.sizeDelta = rectTrs.sizeDelta;                

                SetAllDirty();
            }
            else
            {
                if (isSetNative)
                {
                    MainImage.SetNativeSize();
                    ShadowImage.SetNativeSize();

                    rectTrs.sizeDelta = MainImage.rectTransform.sizeDelta;
                }
                else
                {
                    MainImage.rectTransform.sizeDelta = rectTrs.sizeDelta;
                    ShadowImage.rectTransform.sizeDelta = rectTrs.sizeDelta;
                }
            }

            ShadowImage.color = shadowColor;
            ShadowTrs.localPosition = new Vector2(MainTrs.localPosition.x, MainTrs.localPosition.y - PressingDown);
        }

        protected override void PlayButtonAni(bool isDown)
        {
            base.PlayButtonAni(isDown);
            MainTrs.DOKill();

            if (isDown)
                MainTrs.DOLocalMoveY(ShadowTrs.localPosition.y, isSmooth ? 0.1f : 0f);
            else
                MainTrs.DOLocalMoveY(0f, isSmooth ? 0.1f : 0f);
        }
    }
}