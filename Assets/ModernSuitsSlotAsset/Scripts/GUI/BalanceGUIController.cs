using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	public class BalanceGUIController : MonoBehaviour
	{
        [SerializeField]
        private Text balanceAmountText;

        #region temp vars
        private TweenIntValue balanceTween;
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        #endregion temp vars

        #region regular
        private IEnumerator Start()
        {
            while (!MPlayer)
            {
                yield return new WaitForEndOfFrame();
            }

            // set player event handlers
            MPlayer.ChangeCoinsEvent += ChangeBalanceHandler;
            MPlayer.LoadCoinsEvent += LoadBalanceHandler;
            if (balanceAmountText) balanceTween = new TweenIntValue(balanceAmountText.gameObject, MPlayer.Coins, 1, 3, true, (b) => { if (this && balanceAmountText) balanceAmountText.text = (b > 0) ? b.ToString("# ### ### ### ###") : "0"; });
            Refresh();
        }

        private void OnDestroy()
        {
            if (MPlayer)
            {
                // remove player event handlers
                MPlayer.ChangeCoinsEvent -= ChangeBalanceHandler;
                MPlayer.LoadCoinsEvent -= LoadBalanceHandler;
            }
        }
        #endregion regular

        /// <summary>
        /// Refresh gui balance
        /// </summary>
        private void Refresh()
        {
            if (balanceAmountText && MPlayer) balanceAmountText.text = (MPlayer.Coins > 0) ? MPlayer.Coins.ToString("# ### ### ### ###") : "0";
        }

        #region eventhandlers
        private void ChangeBalanceHandler(int newBalance)
        {
            if (balanceTween != null) balanceTween.Tween(newBalance, 100);
            else
            {
                if (balanceAmountText) balanceAmountText.text = (newBalance > 0) ? newBalance.ToString("# ### ### ### ###") : "0";
            }
        }

        private void LoadBalanceHandler(int newBalance)
        {
            if (balanceAmountText) balanceAmountText.text = (newBalance > 0) ? newBalance.ToString("# ### ### ### ###") : "0";
        }
        #endregion eventhandlers
    }
}
