using UnityEngine;

/*
    24.10.2019 - first
    30.06.2020 remove reverence to =GuiController
 */

namespace Mkey
{
	public class ShowGuiPopUp : MonoBehaviour
	{
        #region temp vars
        protected static GuiController mGui;
        #endregion temp vars

        public void ShowPopUp(PopUpsController popUpsController)
        {
            if (!mGui) mGui = FindObjectOfType<GuiController>();
            if (mGui) mGui.ShowPopUp(popUpsController);
        }
    }
}
