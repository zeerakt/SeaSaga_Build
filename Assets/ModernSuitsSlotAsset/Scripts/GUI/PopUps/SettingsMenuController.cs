using UnityEngine.UI;
using UnityEngine;

namespace Mkey
{
    public class SettingsMenuController : PopUpsController
    {
        [SerializeField]
        private Image[] volume;

        [SerializeField]
        private string ANDROID_RATE_URL;
        [SerializeField]
        private string IOS_RATE_URL;


        #region temp vars
        private SlotSoundController MSound { get { return SlotSoundController.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        #endregion temp vars

        public void SoundPlusButton_Click()
        {
            MSound.SetVolume(MSound.Volume+0.1f);
            SetSoundButtVolume(MSound.Volume);
        }

        public void SoundMinusButton_Click()
        {
            MSound.SetVolume(MSound.Volume - 0.1f);
            SetSoundButtVolume(MSound.Volume);
        }
    
        private void SetSoundButtVolume(float soundVolume)
        {
           if(volume!=null && volume.Length > 0)
            {
                int length = volume.Length;
                float vpl = 1.0f / (float)length;
                int count =Mathf.RoundToInt(soundVolume / vpl);
                Debug.Log("soundVol: " + soundVolume + " ; count: " + count + " ;s/vpl: " + soundVolume / vpl);
                SetVolume(count);
            }
        }

        private void SetVolume(int count)
        {
            for (int i = 0; i < volume.Length; i++)
            {
                 volume[i].gameObject.SetActive(i < count);
            }
        }

        public override void RefreshWindow()
        {
            SetSoundButtVolume(MSound.Volume);
            base.RefreshWindow();
        }
    }
}