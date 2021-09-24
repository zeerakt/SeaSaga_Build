using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class DailyRewardLine : MonoBehaviour
    {
        [SerializeField]
        private Text coinsText;
        [SerializeField]
        private Text dayText;
        [SerializeField]
        private Image rewardImage;
        [SerializeField]
        private Image plateImage;
        [SerializeField]
        private Image glowImage;

        [SerializeField]
        private Sprite activePlateFirstSprite;
        [SerializeField]
        private Sprite notActivePlateFirstSprite;
        [SerializeField]
        private Sprite activePlateSprite;
        [SerializeField]
        private Sprite notActivePlateSprite;
        [SerializeField]
        private Sprite activePlateLastSprite;
        [SerializeField]
        private Sprite notActivePlateLastSprite;
        [SerializeField]
        private Font normalFont;
        [SerializeField]
        private Font highLightFont;
        [SerializeField]
        private Font highLightCoinsFont;

        #region temp vars

        #endregion temp vars

        public void SetData(GameReward reward, int day, int minDay, int maxDay, int rewardDay)
        {
            if (coinsText) coinsText.text = reward.coins.ToString();
            if (coinsText && normalFont && highLightCoinsFont) coinsText.font = (day >= rewardDay) ? highLightCoinsFont : normalFont;
            if (rewardImage ) rewardImage.sprite =  (day >= rewardDay) ? reward.icon : reward.iconOld;
            if (plateImage)
            {
                if (day == minDay) plateImage.sprite = (day == rewardDay) ? activePlateFirstSprite : notActivePlateFirstSprite;
                else if (day == maxDay) plateImage.sprite = (day == rewardDay) ? activePlateLastSprite : notActivePlateLastSprite;
                else plateImage.sprite = (day == rewardDay) ? activePlateSprite : notActivePlateSprite;
            }
            if (glowImage) glowImage.enabled = (day == rewardDay);
            if (dayText) dayText.text = (day == rewardDay) ? "GET" : "DAY " + (day + 1).ToString();
            if (dayText && normalFont && highLightFont) dayText.font = (day >= rewardDay) ? highLightFont : normalFont;
        }
    }
}
