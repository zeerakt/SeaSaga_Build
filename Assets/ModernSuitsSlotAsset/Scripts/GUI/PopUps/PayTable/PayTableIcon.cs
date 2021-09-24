using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class PayTableIcon : MonoBehaviour
    {
        public Image iconImage;
        public Text iconText_3;
        public Text iconText_4;
        public Text iconText_5;
        private int iconId;

        internal void Init(Sprite iconSprite, int iconId, int iconCost_3, int iconCost_4, int iconCost_5)
        {
            iconImage.sprite = iconSprite;
            this.iconId = iconId;
          //  iconText_3.text = (IsFreeSpin()) ? "3: 1 free spin" : "3: " + iconCost_3.ToString() + " coins";
          //  iconText_4.text = (IsFreeSpin()) ? "4: 2 free spin" : "4: " + iconCost_4.ToString() + " coins";
          //  iconText_5.text = (IsFreeSpin()) ? "5: 3 free spin" : "5: " + iconCost_5.ToString() + " coins";
        }
    }
}
