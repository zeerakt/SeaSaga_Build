using System;
using System.Collections.Generic;
using UnityEngine;

/* changes
 28032019
  -add webgl purchasing stub

 -  28032019
  -add life
  -set infinite life

 -31072019
   -add purchase events component

 -30.08.19
    -add #define NOIAP - symbol

09.03.20220 
        - change  NOIAP -> ADDIAP – symbol (from player settings)

 */
#if ((UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID) && ADDIAP)
using UnityEngine.Purchasing;
#endif
/*
 Integrating Unity IAP In Your Game 
 https://unity3d.com/learn/tutorials/topics/ads-analytics/integrating-unity-iap-your-game

*/

namespace Mkey
{


#if ((UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID) && ADDIAP)
    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class Purchaser : MonoBehaviour, IStoreListener
#else
    public class Purchaser : MonoBehaviour
#endif

    {
        [Header("Consumables: ", order = 1)]
        public ShopThingDataReal[] consumable;

        [Header("Non consumables: ", order = 1)]
        public ShopThingDataReal[] nonConsumable;

        [Header("Subscriptions: ", order = 1)]
        public ShopThingDataReal[] subscriptions;

        public static Purchaser Instance;
        private PurchaseEvents PEvent {get{ return GetComponent<PurchaseEvents>(); }}

#if ((UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID) && ADDIAP)

        private static IStoreController m_StoreController;          // Reference to the Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider;  // Reference to store-specific Purchasing subsystems.

        [Space(8, order = 0)]
        [Header("Store keys: ", order = 1)]
        public string appKey = "com.company.bubblegame"; 
        public string googleKey = "com.company.bubblegame";

        void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        void Start() // initialize purchaser
        {
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                InitializePurchasing(); // Begin to configure our connection to Purchasing
            }
        }

        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
    #region build consumables

            if (consumable != null && consumable.Length > 0)
            {
                for (int i = 0; i < consumable.Length; i++)
                {
                    if (consumable[i] != null && !string.IsNullOrEmpty(consumable[i].kProductID))
                    {
                        string prodID = consumable[i].kProductID;
                        builder.AddProduct(prodID, ProductType.Consumable, new IDs() {
                            { appKey + "." + prodID, AppleAppStore.Name },
                            { googleKey + "." + prodID, GooglePlay.Name } }); // com.company.slotgame.productID
                        consumable[i].clickEvent.RemoveAllListeners();
                        consumable[i].clickEvent.AddListener(() => { BuyProductID(prodID); });
                    }
                }
            }

            if (nonConsumable != null && nonConsumable.Length > 0)
            {
                for (int i = 0; i < nonConsumable.Length; i++)
                {
                    if (nonConsumable[i] != null && !string.IsNullOrEmpty(nonConsumable[i].kProductID))
                    {
                        string prodID = nonConsumable[i].kProductID;
                        builder.AddProduct(prodID, ProductType.NonConsumable, new IDs() {
                            { appKey + "." + prodID, AppleAppStore.Name },
                            { googleKey + "." + prodID, GooglePlay.Name }});

                        nonConsumable[i].clickEvent.RemoveAllListeners();
                        nonConsumable[i].clickEvent.AddListener(() => { BuyProductID(prodID); });
                    }
                }
            }
            if (subscriptions != null && subscriptions.Length > 0)
            {
                for (int i = 0; i < subscriptions.Length; i++)
                {
                    if (subscriptions[i] != null && !string.IsNullOrEmpty(subscriptions[i].kProductID))
                    {
                        string prodID = subscriptions[i].kProductID;

                        builder.AddProduct(prodID, ProductType.Subscription, new IDs() {
                            { appKey + "." + prodID, AppleAppStore.Name },
                            { googleKey + "." + prodID, GooglePlay.Name }, });// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.

                        nonConsumable[i].clickEvent.RemoveAllListeners();
                        nonConsumable[i].clickEvent.AddListener(() => { BuyProductID(prodID); });
                    }
                }
            }
    #endregion build consumables

            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        private void BuyProductID(string productId)
        {
            // If the stores throw an unexpected exception, use try..catch to protect my logic here.
            try
            {
                // If Purchasing has been initialized ...
                if (IsInitialized())
                {
                    // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                    Product product = m_StoreController.products.WithID(productId);

                    // If the look up found a product for this device's store and that product is ready to be sold ... 
                    if (product != null && product.availableToPurchase)
                    {
                        Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                        m_StoreController.InitiatePurchase(product);
                    }
                    // Otherwise ...
                    else
                    {
                        // ... report the product look-up failure situation  
                        Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                }
                // Otherwise ...
                else
                {
                    // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                    Debug.Log("BuyProductID FAIL. Not initialized.");
                }
            }
            // Complete the unexpected exception handling ...
            catch (Exception e)
            {
                // ... by reporting any unexpected exception for later diagnosis.
                Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
            }
        }

        /// <summary>
        /// Restore purchases previously made by this customer. Some platforms automatically restore purchases.
        /// Apple currently requires explicit purchase restoration for IAP.
        /// </summary>
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

    #region IStoreListener
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            ShopThingDataReal prod = GetProductById(args.purchasedProduct.definition.id);
            if (prod != null)
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", prod.kProductID));
                if (prod.PurchaseEvent != null)
                {
                    prod.PurchaseEvent.Invoke();
                }
                else
                {
                    Debug.Log("PurchaseEvent failed");
                }
                if (PEvent) PEvent.GoodPurchaseMessage(prod.kProductID, prod.name.ToString());
            }

            else // Or ... an unknown product has been purchased by this user. Fill in additional products here.
            {
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }// Return a flag indicating wither this product has completely been received, or if the application needs to be reminded of this purchase at next app launch. Is useful when saving purchased products to the cloud, and when that save is delayed.

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            ShopThingDataReal prod = GetProductById(product.definition.id);
            if (prod != null)
            {
               if(PEvent) PEvent.FailedPurchaseMessage(prod.kProductID, prod.name);
            }

            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    #endregion IStoreListener

        ShopThingDataReal GetProductById(string id)
        {
            if (consumable != null && consumable.Length > 0)
                for (int i = 0; i < consumable.Length; i++)
                {
                    if (consumable[i] != null)
                        if (String.Equals(id, consumable[i].kProductID, StringComparison.Ordinal))
                            return consumable[i];
                }

            if (nonConsumable != null && nonConsumable.Length > 0)
                for (int i = 0; i < nonConsumable.Length; i++)
                {
                    if (nonConsumable[i] != null)
                        if (String.Equals(id, nonConsumable[i].kProductID, StringComparison.Ordinal))
                            return nonConsumable[i];
                }

            if (subscriptions != null && subscriptions.Length > 0)
                for (int i = 0; i < subscriptions.Length; i++)
                {
                    if (subscriptions[i] != null)
                        if (String.Equals(id, subscriptions[i].kProductID, StringComparison.Ordinal))
                            return subscriptions[i];
                }
            return null;
        }

#else
 void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        void Start() 
        {
            InitializePurchasing();
        }

        public void InitializePurchasing()
        {

            // Create a builder, first passing in a suite of Unity provided stores.

            // Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
    #region build 

            if (consumable != null && consumable.Length > 0)
            {
                for (int i = 0; i < consumable.Length; i++)
                {
                    if (consumable[i] != null && !string.IsNullOrEmpty(consumable[i].kProductID))
                    {
                        string prodID = consumable[i].kProductID;
                        consumable[i].clickEvent.RemoveAllListeners();
                        consumable[i].clickEvent.AddListener(() => { BuyProductID(prodID); });
                    }
                }
            }

            if (nonConsumable != null && nonConsumable.Length > 0)
            {
                for (int i = 0; i < nonConsumable.Length; i++)
                {
                    if (nonConsumable[i] != null && !string.IsNullOrEmpty(nonConsumable[i].kProductID))
                    {
                        string prodID = nonConsumable[i].kProductID;
                        nonConsumable[i].clickEvent.RemoveAllListeners();
                        nonConsumable[i].clickEvent.AddListener(() => { BuyProductID(prodID); });
                    }
                }
            }
            if (subscriptions != null && subscriptions.Length > 0)
            {
                for (int i = 0; i < subscriptions.Length; i++)
                {
                    if (subscriptions[i] != null && !string.IsNullOrEmpty(subscriptions[i].kProductID))
                    {
                        string prodID = subscriptions[i].kProductID;
                        nonConsumable[i].clickEvent.RemoveAllListeners();
                        nonConsumable[i].clickEvent.AddListener(() => { BuyProductID(prodID); });
                    }
                }
            }
    #endregion build 
        }

        private void BuyProductID(string productId)
        {
            ShopThingDataReal prod = GetProductById(productId);
            if (prod != null)
            {
                prod.PurchaseEvent?.Invoke();
                if(PEvent) PEvent.GoodPurchaseMessage(productId, prod.name);
            }
            else
            {
                if(PEvent) PEvent.FailedPurchaseMessage(productId, prod.name);
            }
        }

        private ShopThingDataReal GetProductById(string id)
        {
            if (consumable != null && consumable.Length > 0)
                for (int i = 0; i < consumable.Length; i++)
                {
                    if (consumable[i] != null)
                        if (String.Equals(id, consumable[i].kProductID, StringComparison.Ordinal))
                            return consumable[i];
                }

            if (nonConsumable != null && nonConsumable.Length > 0)
                for (int i = 0; i < nonConsumable.Length; i++)
                {
                    if (nonConsumable[i] != null)
                        if (String.Equals(id, nonConsumable[i].kProductID, StringComparison.Ordinal))
                            return nonConsumable[i];
                }

            if (subscriptions != null && subscriptions.Length > 0)
                for (int i = 0; i < subscriptions.Length; i++)
                {
                    if (subscriptions[i] != null)
                        if (String.Equals(id, subscriptions[i].kProductID, StringComparison.Ordinal))
                            return subscriptions[i];
                }
            return null;
        }
#endif

    }
}
