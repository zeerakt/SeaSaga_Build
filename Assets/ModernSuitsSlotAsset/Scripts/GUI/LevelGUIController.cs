using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class LevelGUIController : MonoBehaviour
    {
        [SerializeField]
        private Text LevelNumberText;
        [SerializeField]
        private ProgressSlider progressSlider;
        [SerializeField]
        private WarningMessController LevelUpCongratulationPrefab;

        [SerializeField]
        private string levelNumberPrefix;

        #region temp vars
        private int levelTweenId;
        private float levelxp;
        private float oldLevelxp;
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        #endregion temp vars

        #region regular
        private void Start()
        {
            StartCoroutine(StartC());
        }

        private IEnumerator StartC()
        {
            while (!MPlayer)
            {
                yield return new WaitForEndOfFrame();
            }
            MPlayer.ChangeLevelProgressEvent += ChangeLevelProgressHandler;
            MPlayer.ChangeLevelEvent += ChangeLevelHandler;
            RefreshLevel();
        }

        private void OnDestroy()
        {
            if (MPlayer) MPlayer.ChangeLevelProgressEvent -= ChangeLevelProgressHandler;
            if (MPlayer) MPlayer.ChangeLevelEvent -= ChangeLevelHandler;
        }
        #endregion regular

        /// <summary>
        /// Refresh gui level
        /// </summary>
        private void RefreshLevel()
        {
            SimpleTween.Cancel(levelTweenId, false);
            if (MPlayer)
            {
                if (progressSlider)
                {
                    levelxp = MPlayer.LevelProgress;
                    if (levelxp > oldLevelxp)
                    {
                        levelTweenId = SimpleTween.Value(gameObject, oldLevelxp, levelxp, 0.3f).SetOnUpdate((float val) =>
                        {
                            oldLevelxp = val;
                            progressSlider.SetFillAmount(oldLevelxp / 100f);
                        }).ID;
                    }
                    else
                    {
                        progressSlider.SetFillAmount(levelxp / 100f);
                        oldLevelxp = levelxp;
                    }
                }
                if (LevelNumberText) LevelNumberText.text =levelNumberPrefix + MPlayer.Level.ToString();
            }
        }

        #region eventhandlers
        private void ChangeLevelHandler(int newLevel, int reward, bool useLevelReward)
        {
            if (this) RefreshLevel();

            if (useLevelReward && reward > 0)MGui.ShowMessageWithYesNoCloseButton(LevelUpCongratulationPrefab, reward.ToString(), newLevel.ToString(), () => { MPlayer.AddCoins(reward); }, null, null);
        }

        private void ChangeLevelProgressHandler(float newProgress)
        {
            if (this) RefreshLevel();
        }
        #endregion eventhandlers
    }
}