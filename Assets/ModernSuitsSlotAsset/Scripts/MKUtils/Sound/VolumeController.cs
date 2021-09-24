using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 *31.03.2020 - first
 */
namespace Mkey
{
	public class VolumeController : MonoBehaviour
	{
        [SerializeField]
        private ProgressBarSlider barSlider;

        #region temp vars
        private SoundMaster MSound { get { return SoundMaster.Instance; } }
        #endregion temp vars
		
		#region regular
		private IEnumerator Start()
		{
            while (!MSound) yield return new WaitForEndOfFrame();

            if (barSlider) barSlider.SetFillAmount(MSound.Volume);
            MSound.ChangeVolumeEvent += ChangeVolumeEventHandler;
		}

        private void OnDestroy()
        {
            if(MSound) MSound.ChangeVolumeEvent -= ChangeVolumeEventHandler;
        }
        #endregion regular

        public void VolumePlusButton_Click()
        {
            MSound.SetVolume(MSound.Volume + 0.1f);
        }

        public void VolumeMinusButton_Click()
        {
            MSound.SetVolume(MSound.Volume - 0.1f);
        }

        private void ChangeVolumeEventHandler(float volume)
        {
            if (barSlider) barSlider.SetFillAmount(volume);
        }
    }
}
