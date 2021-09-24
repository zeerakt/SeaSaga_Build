using UnityEngine;
using MkeyFW;

namespace Mkey
{
    public class SlotEvents : MonoBehaviour
    {
        [SerializeField]
        private PopUpsController chestsPrefab;
        public FortuneWheelInstantiator Instantiator;
        public bool autoStartMiniGame = true;

        public static SlotEvents Instance;

        public bool MiniGameStarted { get { return (Instantiator && Instantiator.MiniGame); } }

        #region temp vars
        private Mkey.SlotPlayer MPlayer { get { return Mkey.SlotPlayer.Instance; } }
        private SlotSoundController MSound { get { return SlotSoundController.Instance; } }
        private SlotGuiController MGUI { get { return SlotGuiController.Instance; } }
        #endregion temp vars

        private void Awake()
        {
            Instance = this; 
        }

        public void AddLevelProgress_100()
        {
            MPlayer.AddLevelProgress(100f);
            MSound.SoundPlayWinCoins(0, null);
        }

        public void AddLevelProgress_50()
        {
            MPlayer.AddLevelProgress(50f);
            MSound.SoundPlayWinCoins(0, null);
        }

        public void ShowChestMiniGame()
        {
            MGUI.ShowPopUp(chestsPrefab);
        }

        public void ShowFortuneWheel()
        {
            MSound.SoundPlayBonusGame(0, null);
            Instantiator.Create(autoStartMiniGame);
            if (Instantiator.MiniGame)
            {
                Instantiator.MiniGame.SetBlocked(autoStartMiniGame, autoStartMiniGame);
                Instantiator.SpinResultEvent += (coins, isBigWin) => { MPlayer.AddCoins(coins); };
            }
        }
    }
}