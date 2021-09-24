using System;
using UnityEngine;
using UnityEngine.UI;
/*
 13.01.19
    -add fillImage exist
 13.05.20
    - PSlider
 */

namespace Mkey
{
    [ExecuteInEditMode]
    public class SimpleSlider :  PSlider
    {
        public Image fillImage;

        [ObsoleteAttribute("This property is obsolete. Use FillAmount and SetFillAmount() instead.", false)]
        public float value
        {
            get
            {
                return (fillImage)?fillImage.fillAmount:0;
            }
            set
            {
               if (fillImage) fillImage.fillAmount = value;
            }
        }

        [SerializeField]
        [Range(0, 1f)]
        private float fillAmount;

        public float FillAmount { get { return fillAmount; } }

        #region temp vars
        private RectTransform rtL;
        private RectTransform rtR;
        #endregion temp vars

        #region regular
        private void OnEnable()
        {
          
        }

        private void OnValidate()
        {
            fillAmount = Mathf.Clamp01(fillAmount);
        }

        private void Update()
        {
            if (!fillImage) return;
            fillImage.fillAmount = fillAmount;
        }
        #endregion regular

        public override void SetFillAmount(float fillAmount)
        {
            this.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }
}