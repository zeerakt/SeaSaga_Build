using System;
using UnityEngine;
using UnityEngine.UI;

/*
  use touchpad scene button, need collider (possible asTrigger)
    11.11.2019 - first
    18.11.2019 - fix pointer up
    21.07.2020 - add set pressed
 */
namespace Mkey
{
    public class SceneButton : TouchPadMessageTarget
    {
        [SerializeField]
        private Sprite normalSprite;
        [SerializeField]
        private Sprite pressedSprite;
        [SerializeField]
        private bool canFixed;

        public bool interactable = true;

        #region events
        public Button.ButtonClickedEvent clickEvent;
        public Action <SceneButton> clickEventAction;
        #endregion events

        #region temp vars
        private SpriteRenderer spriteRenderer;
        private bool pDown = false;
        #endregion temp vars

        public bool Pressed { get; private set; }

        #region regular
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            PointerUpEvent += (tpea) => 
            {
                if(!interactable) return;
                if (!pDown) return;
                pDown = false;
                if (!canFixed) Pressed = false;
                if (spriteRenderer && normalSprite && pressedSprite) spriteRenderer.sprite = (Pressed) ? pressedSprite : normalSprite;
                clickEvent?.Invoke();
                clickEventAction?.Invoke(this);
            };

            PointerDownEvent += (tpea) => 
            {
                if (!interactable) return;
                pDown = true;
                if (canFixed) Pressed = !Pressed;
                if (spriteRenderer && pressedSprite) spriteRenderer.sprite =  pressedSprite;
            };
        }
        #endregion regular

        public void Release()
        {
            Pressed = false;
            if (spriteRenderer && normalSprite && pressedSprite) spriteRenderer.sprite = (Pressed) ? pressedSprite : normalSprite;
        }

        public void SetPressed()
        {
            Pressed = true;
            if (spriteRenderer && normalSprite && pressedSprite) spriteRenderer.sprite = (Pressed) ? pressedSprite : normalSprite;
        }
    }
}
