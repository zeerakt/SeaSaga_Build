using System.Collections.Generic;
using UnityEngine;

/* Scale scene objects according base resolution
    changes
    18.01.2019
    - add h-w adjust
    - add validate
    10.10.2019
    -remove base width, base height

 */
namespace Mkey
{
    [ExecuteInEditMode]
    public class SceneScaler : MonoBehaviour {
        [SerializeField]
        private float baseRatio = 0.75f;
        [HideInInspector]
        [SerializeField]
        private float baseRatioOld = 0.0f;

        [Range(0,1)]
        [SerializeField]
        private float Height_Width = 1;
        [HideInInspector]
        [SerializeField]
        private float Height_WidthOld = 1;

        [SerializeField]
        private float additionalScale = 1;
        [HideInInspector]
        [SerializeField]
        private float additionalScaleOld = 1;

      //  [SerializeField]
        private bool useLerpScale = false;
      //  [SerializeField]
        private LerpScale lerpScale_0_5;

        #region temp vars
        private float currRatio;
        private int width = 0;
        private int height = 0;
        private float sc = 1f;
        private bool debug = false;
        #endregion temp vars

        #region regular
        void Start()
        {
            SetScale();
        }

        void Update()
        {
            SetScale();
        }
        #endregion regular

        void SetScale()
        {
            #region validate
            Height_Width = Mathf.Clamp01(Height_Width);
            additionalScale = Mathf.Clamp01(additionalScale);
            baseRatio = Mathf.Clamp(baseRatio, 0.3f, 2.5f);
            #endregion validate

            if (!useLerpScale)
            {
                if (width != Screen.width || height != Screen.height || baseRatio != baseRatioOld || Height_Width != Height_WidthOld || additionalScaleOld != additionalScale)
                {
                    width = Screen.width;
                    height = Screen.height;
                    currRatio = width / (float)height;
                    Height_WidthOld = Height_Width;
                    additionalScaleOld = additionalScale;
                    baseRatioOld = baseRatio;
                    BaseScale();
                }
            }

            else
            {

            }
           

            if (debug) Debug.Log("width: " + width + " ; height: " + height + " ;baseW/baseH: " + baseRatio + "currW/currH: " + currRatio + " ;scale: " + sc);
        }

        void BaseScale()
        {
            sc = currRatio / baseRatio;
            sc = Mathf.Lerp(1, sc, Height_Width) * additionalScale;
            gameObject.transform.localScale = new Vector3(sc, sc, sc);
        }
    }

   public  class LerpScale
    {
        public float scale = 1;
        public Vector3 offset = Vector3.zero;

        public static bool operator  == (LerpScale lS1, LerpScale lS2)
        {
            return ((lS1!=null) && (lS2!=null) && (lS1.scale == lS2.scale)  && (lS1.offset == lS2.offset));
        }

        public static bool operator != (LerpScale lS1, LerpScale lS2)
        {
            return ((lS1 == null) || (lS2 == null) || (lS1.scale != lS2.scale) || (lS1.offset != lS2.offset));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }

}
