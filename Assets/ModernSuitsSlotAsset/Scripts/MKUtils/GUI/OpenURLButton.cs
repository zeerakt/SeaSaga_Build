using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
	22.11.2019 - first
    31.01.2020 - rename to OpenURLButton
*/
namespace Mkey
{
	public class OpenURLButton : MonoBehaviour
	{
        [SerializeField]
        private string URL;

        public void Click()
        {
            if (!string.IsNullOrEmpty(URL)) Application.OpenURL(URL);
        }
    }
}
