using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Mkey
{
    public class MenuButtonTransition : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Font normalFont;
        public Font pressedFont;
        public Material normalFontMat;
        public Material pressedFontMat;

        public Text buttonText;

        public void OnPointerDown(PointerEventData data)
        {
            if (GetComponent<Button>().interactable)
            {
                if (buttonText)
                {
                    buttonText.font = pressedFont;
                    buttonText.material = pressedFontMat;
                }
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            if (GetComponent<Button>().interactable)
            {
                if (buttonText)
                {
                    buttonText.font = normalFont;
                    buttonText.material = normalFontMat;
                }
            }
        }

    }
}

