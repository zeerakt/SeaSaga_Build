using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MkeyFW
{
	public class WheelStarter : MonoBehaviour
	{
        [SerializeField]
        private WheelController wheelController;

        #region temp vars
        private Mkey.SlotPlayer MPlayer { get { return Mkey.SlotPlayer.Instance; } }
        #endregion temp vars

        #region regular
        private IEnumerator Start()
		{
            yield return new WaitForEndOfFrame();
            if (!wheelController) GetComponent<WheelController>();
            if (wheelController)
            {
                wheelController.SetBlocked(false, true);
                wheelController.SpinResultEvent = (coins, isBigWin) =>
                {
                    if(MPlayer)MPlayer.AddCoins(coins);
                    wheelController.SetBlocked(false, true);
                };
            }
        }
		#endregion regular
	}
}
