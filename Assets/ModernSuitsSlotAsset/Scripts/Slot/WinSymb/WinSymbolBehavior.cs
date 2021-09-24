using System;
using UnityEngine;

namespace Mkey
{
    public class WinSymbolBehavior : MonoBehaviour
    {
        [SerializeField]
        private string winTag;

        protected static int AddSortingOrder { get; set; }
        protected SlotSymbol Symbol {get; private set; }
        protected Sprite SymbolSprite { get; private set; }
        protected int SymbolSortingOrder { get; private set; }
        protected int SymbolSortingLayerID { get; private set; }
        protected SlotController Slot { get; private set; }
        public string WinTag { get { return winTag; } }

        protected Action<GameObject, float, Action> delayAction = (g, time, callBack) => { SimpleTween.Value(g, 0, 1, time).AddCompleteCallBack(callBack); };

        private int defaultSortingOrder = 10;
        private int defaultSortingLayerID = 0;// ID of the default sorting layer is always 0.

        #region regular
        private void Start()
        {
            Symbol = GetComponentInParent<SlotSymbol>();
           // Debug.Log(Symbol);
            SymbolSprite = (Symbol) ? Symbol.GetSprite() : null;
            SymbolSortingOrder = (Symbol) ? Symbol.GetSortingOrder() : defaultSortingOrder;
            SymbolSortingLayerID = (Symbol) ? Symbol.GetSortingLayerID() : defaultSortingLayerID;
            Slot = GetComponentInParent<SlotController>();

            PlayWin();
        }

        private void OnDestroy()
        {
            Cancel();
        }
        #endregion regular

        #region virtual
        protected virtual void PlayWin()
        {

        }

        protected virtual void Cancel()
        {

        }
        #endregion virtual

        protected int GetNextAddSortingOrder()
        {
            AddSortingOrder = (AddSortingOrder < 100) ? ++AddSortingOrder : 0;
            return AddSortingOrder;
        }
    }
}