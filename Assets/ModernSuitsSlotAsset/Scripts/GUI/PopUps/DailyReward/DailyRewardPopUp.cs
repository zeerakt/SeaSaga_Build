using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	public class DailyRewardPopUp : PopUpsController
	{
        [SerializeField]
        private DailyRewardLine dailyRewardLinePrefab;
        [SerializeField]
        private RectTransform rewardsParent;

        #region temp vars
        private DailyRewardController DRC { get { return DailyRewardController.Instance; } }
        private GameReward reward;
        #endregion temp vars
		
		#region regular
		
        #endregion regular

        public override void RefreshWindow()
        {
            if (!dailyRewardLinePrefab || !rewardsParent) return;

            DailyRewardLine[] dls = rewardsParent.GetComponentsInChildren<DailyRewardLine>();
            foreach (var item in dls)
            {
                Destroy(item);
            }

            List<GameReward> rewards = new List<GameReward>(DRC.Rewards);
            int rewDay = DRC.RewardDay;
            int minDay = Mathf.Max(rewDay - 2, 0);
            int maxDay = minDay + 6;// Debug.Log("rday:" + rewDay + " ;min:" + minDay + " ; max:" + maxDay);
            reward = DRC.RepeatingReward ? rewards[rewDay % rewards.Count] : rewards[Mathf.Clamp(rewDay, 0, rewards.Count)];

            for (int i = minDay; i <= maxDay; i++)
            {
                int rI = DRC.RepeatingReward ? i % rewards.Count : Mathf.Clamp(i, 0, rewards.Count-1);

                var item = rewards[rI];
                dailyRewardLinePrefab.SetData(item, i, minDay, maxDay, rewDay);
                DailyRewardLine dl = Instantiate(dailyRewardLinePrefab);
                dl.transform.parent = rewardsParent;
                dl.transform.localScale = Vector3.one;
            }

            base.RefreshWindow();
        }

        public void Apply_Click()
        {
            DRC.ApplyReward(reward);
            CloseWindow();
        }
    }
}
