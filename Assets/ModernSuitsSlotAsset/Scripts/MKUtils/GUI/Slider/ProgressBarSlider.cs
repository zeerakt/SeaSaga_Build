using UnityEngine;
using UnityEngine.UI;
/*
	27.02.2020 - first
*/
namespace Mkey
{
    public class ProgressBarSlider : PSlider
    {
        [SerializeField]
        private Image[] full;

        #region temp vars
        #endregion temp vars

        #region regular
       
        #endregion regular

        public override void SetFillAmount(float fillAmount)
        {
            int fullCount = (int)(fillAmount * 10.0f);
            for (int i = 0; i < full.Length; i++)
            {
              if(full[i])  full[i].enabled = (fullCount >= (i + 1));
            }
        }
    }
}