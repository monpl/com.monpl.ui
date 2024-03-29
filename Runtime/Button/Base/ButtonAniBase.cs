using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Monpl.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(RectTransform))]
    public class ButtonAniBase : Graphic, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler
    {
        protected bool isPressed;

        // Invisible Graphic
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (GetComponent<Button>().interactable == false)
                return;

            isPressed = true;

            PlayButtonAni(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isPressed == false)
                return;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;

            PlayButtonAni(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isPressed == false)
                return;
        }


        protected virtual void PlayButtonAni(bool isDown)
        {
        }
    }
}