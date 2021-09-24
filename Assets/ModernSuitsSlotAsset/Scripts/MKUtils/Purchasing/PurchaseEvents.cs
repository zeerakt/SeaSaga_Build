using UnityEngine;

namespace Mkey
{
    public class PurchaseEvents : MonoBehaviour
    {
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }

        public void AddCoins(int count)
        {
            if (MPlayer != null)
            {
                MPlayer.AddCoins(count);
            }
        }

        internal void GoodPurchaseMessage(string prodId, string prodName)
        {
            if (MGui)
            {
                MGui.ShowMessageWithCloseButton("Succesfull!!!", prodName + " purchased successfull.", ()=> { } );
            }
        }

        internal void FailedPurchaseMessage(string prodId, string prodName)
        {
            if (MGui)
            {
                MGui.ShowMessageWithCloseButton("Sorry.", prodName + " - purchase failed.", ()=> { });
            }
        }
    }
}