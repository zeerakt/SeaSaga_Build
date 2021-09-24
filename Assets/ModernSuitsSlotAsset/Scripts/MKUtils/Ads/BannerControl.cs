using UnityEngine;
using System.Collections;

/*
   15.04.2020- first
   04.01.2021 -
        -avoid duplicate banner
 */

namespace Mkey
{
    public enum BannerState {Show, Hide }
    public class BannerControl : MonoBehaviour
    {
        [SerializeField]
        private BannerState banner = BannerState.Show;
#if ADDGADS
        #region temp vars
        private AdsControl GADS { get { return AdsControl.Instance; } }
        #endregion temp vars

        private IEnumerator Start()
        {
            while (!GADS)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();

            if (banner == BannerState.Show)
            {
                GADS.HideBanner();
                GADS.ShowBanner();
            }
            else
            {
                GADS.HideBanner();
            }
        }

#endif
    }
}