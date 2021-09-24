using System;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class LobbyMenuController : MonoBehaviour
    {
        #region temp vars
        private Button[] buttons;
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        #endregion temp vars

        #region properties
        #endregion properties

        #region regular
        void Start()
        {
            buttons = GetComponentsInChildren<Button>();
        }
        #endregion regular

        /// <summary>
        /// Set all buttons interactble = activity
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            if (buttons == null) return;
            foreach (Button b in buttons)
            {
                if (b) b.interactable = activity;
            }
        }

        public void ResetPlayer()
        {
            SlotPlayer.Instance.SetDefaultData();
        }
    }
}