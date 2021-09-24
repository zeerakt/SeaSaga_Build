using UnityEngine.UI;


namespace Mkey
{
    public class LevelXP_PU : PopUpsController
    {
        public Text XPText;
        public Text XPValue;

        #region temp vars
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        private SlotSoundController MSound { get { return SlotSoundController.Instance; } }
        #endregion temp vars

        public override void  RefreshWindow()
        {
            if(XPValue) XPValue.text = MPlayer.LevelProgress.ToString("00.0");
            base.RefreshWindow();
        }
    }

   
}