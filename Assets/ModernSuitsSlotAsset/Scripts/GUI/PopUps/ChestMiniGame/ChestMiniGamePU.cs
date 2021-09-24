using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	public class ChestMiniGamePU : PopUpsController
	{
        [SerializeField]
        private List<ChestBehavior> chests;
        [SerializeField]
        private GameObject touchBlocker;
        [Header ("Coins random amount in chest, hundreds")]
        [SerializeField]
        private int minCoinsInChest;
        [SerializeField]
        private int maxCoinsInChest;

        #region temp vars
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        #endregion temp vars


        #region regular
        public override void RefreshWindow()
        {
            OnValidate();
            touchBlocker.SetActive(true);
            ParallelTween pt = new ParallelTween();
            float d = 0;
            foreach (var item in chests)
            {
                float f = d;
                pt.Add(callBack=> { item.ScaleOut(callBack, f); });
                d += 0.35f;
                item.Coins = UnityEngine.Random.Range(minCoinsInChest, maxCoinsInChest + 1)*100;
                item.GetComponent<Button>().onClick.AddListener(() => { touchBlocker.SetActive(true); MPlayer.AddCoins(item.Coins); });
            }
            pt.Start(()=> { touchBlocker.SetActive(false); }); 
            base.RefreshWindow();
        }

        private void OnValidate()
        {
            minCoinsInChest = Mathf.Max(0, minCoinsInChest);
            maxCoinsInChest = Math.Max(minCoinsInChest, maxCoinsInChest);
        }
		#endregion regular
	}
}
