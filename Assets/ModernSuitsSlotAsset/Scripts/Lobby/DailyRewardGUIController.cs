using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class DailyRewardGUIController : MonoBehaviour
    {
        [SerializeField]
        private PopUpsController dailyRewardPUPrefab;

        #region temp vars
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private DailyRewardController DRC { get { return DailyRewardController.Instance; } }
        private GameReward reward;
        private List<GameReward> rewards;
        private int rewDay = -1;
        #endregion temp vars

        #region regular
        private IEnumerator Start()
        {
            while (!DRC) yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            rewards = new List<GameReward>(DRC.Rewards);
            rewDay = DRC.RewardDay;
            if (rewDay >= 0)
            {
                StartCoroutine(ShowRewardPopup(1.5f, rewDay));
            }
        }

        private void OnDestroy()
        {

        }
        #endregion regular

        private IEnumerator ShowRewardPopup(float delay, int rewDay)
        {
            if (rewards != null && rewDay >= 0)
            {
                yield return new WaitForSeconds(delay);
                MGui.ShowPopUp(dailyRewardPUPrefab);
            }
        }
    }
}
