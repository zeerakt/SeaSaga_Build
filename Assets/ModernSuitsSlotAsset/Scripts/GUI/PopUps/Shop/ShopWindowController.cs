using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class ShopWindowController : PopUpsController
    {
        [SerializeField]
        private RectTransform ThingsParent;
        [SerializeField]
        public RealShopType shopType = RealShopType.Coins;
        [SerializeField]
        private GameObject scrollFlag;
        [SerializeField]
        public List<ShopThingHelper> shopThings;

        public override void RefreshWindow()
        {
            CreateThingTab();
            base.RefreshWindow();
        }

        private void CreateThingTab()
        {
            ShopThingHelper[] sT = ThingsParent.GetComponentsInChildren<ShopThingHelper>();
            foreach (var item in sT)
            {
                DestroyImmediate(item.gameObject);
            }

            Purchaser p = Purchaser.Instance;
            if (p == null) return;

            List<ShopThingDataReal> products = new List<ShopThingDataReal>();
            if (p.consumable != null && p.consumable.Length > 0) products.AddRange(p.consumable);
            if (p.nonConsumable != null && p.nonConsumable.Length > 0) products.AddRange(p.nonConsumable);
            if (p.subscriptions != null && p.subscriptions.Length > 0) products.AddRange(p.subscriptions);

            if (products.Count==0) return;

            shopThings = new List<ShopThingHelper>();
            for (int i = 0; i < products.Count; i++)
            {
              if(products[i]!=null && products[i].shopType == shopType && products[i].prefab)  shopThings.Add(products[i].prefab.Create(ThingsParent, products[i]));
            }

            if (scrollFlag) scrollFlag.SetActive(shopThings.Count > 5);
        }
    }
}