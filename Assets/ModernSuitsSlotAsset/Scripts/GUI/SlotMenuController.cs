using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class SlotMenuController : MonoBehaviour
    {
        [Space(16, order = 0)]
        [SerializeField]
        private SlotController slot;

        #region temp vars
        private Button[] buttons;
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        #endregion temp vars

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
              if(b)  b.interactable = activity;
            }
        }

        #region header menu
        public void Lobby_Click()
        {
            SceneLoader.Instance.LoadScene(0);
        }
        #endregion header menu

        private string GetMoneyName(int count)
        {
            if (count > 1) return "coins";
            else return "coin";
        }
    }
}