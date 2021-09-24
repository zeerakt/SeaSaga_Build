using UnityEngine;
using UnityEngine.UI;

/*
	22.11.2019 - first
    27.02.2020 - PSlider
	30.04.2020 - left, right images
    27.05.2020 - leftsize, pointer
*/
namespace Mkey
{
    [ExecuteInEditMode]
    public class ProgressSlider : PSlider
    {
        [SerializeField]
        private Image left;
        [SerializeField]
        private Image right;
        [SerializeField]
        private RectTransform pointer;

        [SerializeField]
        private float leftSize = 0.1f;
        [SerializeField]
        private float minPointerAmount = 0;
        [SerializeField]
        private float maxPointerAmount = 1;

        [SerializeField]
        [Range(0, 1f)]
        private float fillAmount;

        public float FillAmount { get { return fillAmount; }}

        #region temp vars
        private RectTransform rtL;
        private RectTransform rtR;
        #endregion temp vars

        #region regular
        private void OnEnable()
        {
            if (left && !rtL)
            {
                rtL = left.GetComponent<RectTransform>();
            }
            if (right && !rtR)
            {
                rtR = right.GetComponent<RectTransform>();
            }
        }

        private void OnValidate()
        {
            fillAmount = Mathf.Clamp01(fillAmount);
            leftSize = Mathf.Clamp01(leftSize);

            minPointerAmount = Mathf.Clamp01(minPointerAmount);
            maxPointerAmount = Mathf.Clamp01(maxPointerAmount);
            maxPointerAmount = Mathf.Max(minPointerAmount, maxPointerAmount);
        }

        private void Update()
        {
            if (!left) return;

            left.fillAmount = leftSize * fillAmount;

            if (right)
            {
                right.fillAmount = (1f-leftSize) * fillAmount;
                rtR.anchoredPosition = new Vector2(rtL.anchoredPosition.x + (fillAmount - 1f) * rtL.rect.width, rtR.anchoredPosition.y);
            }
            if (pointer) pointer.gameObject.SetActive(fillAmount >= minPointerAmount && fillAmount <= maxPointerAmount);
        }
        #endregion regular

        public override void SetFillAmount(float fillAmount)
        {
            this.fillAmount = Mathf.Clamp01(fillAmount);
        }
    }
}