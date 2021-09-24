using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
	public class SupportButton : MonoBehaviour
	{
        [SerializeField]
        private string SUPPORT_URL;

        public void Click()
        {
            if (!string.IsNullOrEmpty(SUPPORT_URL)) Application.OpenURL(SUPPORT_URL);
        }
    }
}
