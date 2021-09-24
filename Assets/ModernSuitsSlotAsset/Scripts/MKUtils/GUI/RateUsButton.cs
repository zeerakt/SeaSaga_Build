using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	public class RateUsButton : MonoBehaviour
	{
        [SerializeField]
        private string ANDROID_RATE_URL;
        [SerializeField]
        private string IOS_RATE_URL;

        public void Click()
        {
#if UNITY_ANDROID
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#elif UNITY_IOS
            if (!string.IsNullOrEmpty(IOS_RATE_URL)) Application.OpenURL(IOS_RATE_URL);
#else
            if (!string.IsNullOrEmpty(ANDROID_RATE_URL)) Application.OpenURL(ANDROID_RATE_URL);
#endif
        }
    }
}
