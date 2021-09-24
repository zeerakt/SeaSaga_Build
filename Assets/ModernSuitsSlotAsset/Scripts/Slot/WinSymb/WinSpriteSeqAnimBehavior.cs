using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class WinSpriteSeqAnimBehavior : WinSymbolBehavior
    {
        #region override
        protected override void PlayWin()
        {
            if(Symbol) Symbol.HideSymbol();
            SpriteRenderer sR = GetComponent<SpriteRenderer>();
            if (sR)
            {
                sR.sortingOrder = SymbolSortingOrder + GetNextAddSortingOrder();
            }
        }

        protected override void Cancel()
        {

        }
        #endregion override
    }
}