using UnityEngine;
using System;

namespace Mkey
{
    public class LineButtonBehavior : TouchPadMessageTarget
    {
        [SerializeField]
        private Sprite normalSprite;
        [SerializeField]
        private Sprite pressedSprite;
        private SpriteRenderer spriteRenderer;

        public bool interactable = true;

        #region regular
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        #endregion regular

        internal void Refresh(bool lineSelected)
        {
            if (spriteRenderer) spriteRenderer.sprite = (lineSelected) ? pressedSprite : normalSprite;
        }
    }
}

/*
   public Action PointerDownEvent;
  #region touch callbacks
        public void PointerUp(TouchPadEventArgs tpea)
        {

        }
        public void PointerDown(TouchPadEventArgs tpea)
        {
           if (interactable)  PointerDownEvent?.Invoke();
        }
        public void DragBegin(TouchPadEventArgs tpea) { }
        public void DragEnter(TouchPadEventArgs tpea) { }
        public void DragExit(TouchPadEventArgs tpea) { }
        public void DragDrop(TouchPadEventArgs tpea) { }
        public void Drag(TouchPadEventArgs tpea) { }
        public GameObject GetDataIcon()
        {
            return null;
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        #endregion touch callbacks
 */
