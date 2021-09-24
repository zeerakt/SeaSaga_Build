
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if ADDGADS
    using GoogleMobileAds.Api;
#endif

#if UNITY_EDITOR
    using UnityEditor;
#endif

/*
  test https://developers.google.com/admob/unity/test-ads
  Banner 	        ca-app-pub-3940256099942544/6300978111
  Interstitial 	    ca-app-pub-3940256099942544/1033173712
  Rewarded Video 	ca-app-pub-3940256099942544/5224354917
  Native Advanced 	ca-app-pub-3940256099942544/2247696110

  21.01.2020 
    - first release
    09.03.2020 
        - change  NOADS -> ADDGADS – symbol (from project settings)
    08.04.2020
        -destroy banner
    15.04.2020
        show banner, hide banner
    23.05.2020
        serialize ads ids
    14.07.2020 
        - fix showrewardedad (noads), add editor
 */

namespace Mkey
{
    public class AdsControl : MonoBehaviour
    {
        private bool customBannerRequest = false;
        private bool customInterstitialRequest = false;
        private bool customRewardedAdRequest = false;

        [Header("Banner")]
#if ADDGADS
        [SerializeField]
        private bool requestBanner = true;
        [SerializeField]
        private AdPosition bannerPosition = AdPosition.Bottom;
#endif
        [SerializeField]
        private string bannerAdUnitIdAndroid = "ca-app-pub-3940256099942544/6300978111"; // test
        [SerializeField]
        private string bannerAdUnitIdIos = "ca-app-pub-3940256099942544/2934735716"; // test

        [Header("Interstitial")]
#if ADDGADS
        [SerializeField]
        private bool requestInterstitial = true;
#endif
        [SerializeField]
        private string interstitialAdUnitIdAndroid = "ca-app-pub-3940256099942544/1033173712";
        [SerializeField]
        private string interstitialAdUnitIdIos = "ca-app-pub-3940256099942544/4411468910";

        [Header("Rewarded ads")]
#if ADDGADS
        [SerializeField]
        private bool requestRewardedAds = true;
#endif
        [SerializeField]
        private List<RewardAd> rewardAds;

#if ADDGADS
        [Space(16)]
        [SerializeField]
        private bool showTestGui = false;

#region temp vars
        private InterstitialAd interstitial;
        private BannerView bannerView;
        private RewardedAd rewardedAd;
        private float deltaTime = 0.0f;
        private Action<bool, string, double> rewardCallBack; // <well, args.type or message, args.amount>
        private Action rewardedOpenedCallBack; // MSound.SetSound(false); 
        private Action rewardedClosedCallBack; //  MSound.SetSound(true);
        private Action interstitialOpenedCallBack; // MSound.SetSound(false); 
        private Action interstitialClosedCallBack; // MSound.SetSound(true);
        private static string outputMessage = string.Empty;
#endregion temp vars

        public static AdsControl Instance;

#region regular
        private void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }

        private void Start()
        {
            MobileAds.SetiOSAppPauseOnBackground(true);

            MobileAds.Initialize(initStatus => { }); // new

            if (requestRewardedAds) CreateAndLoadRewardedAd();

            if (requestInterstitial) RequestInterstitial();

            if (requestBanner) RequestBanner();
        }

        private void Update()
        {
            // Calculate simple moving average for time to render screen. 0.1 factor used as smoothing value.
            deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
        }

        private void OnGUI() // https://github.com/googleads/googleads-mobile-unity/blob/master/samples/HelloWorld/Assets/Scripts/GoogleMobileAdsDemoScript.cs
        {
            if (!showTestGui) return;
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, Screen.width, Screen.height);
            style.alignment = TextAnchor.LowerRight;
            style.fontSize = (int)(Screen.height * 0.06);
            style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
            float fps = 1.0f / this.deltaTime;
            string text = string.Format("{0:0.} fps", fps);
            GUI.Label(rect, text, style);

            // Puts some basic buttons onto the screen.
            GUI.skin.button.fontSize = (int)(0.035f * Screen.width);
            float buttonWidth = 0.35f * Screen.width;
            float buttonHeight = 0.15f * Screen.height;
            float columnOnePosition = 0.1f * Screen.width;
            float columnTwoPosition = 0.55f * Screen.width;

            Rect requestBannerRect = new Rect(
                columnOnePosition,
                0.05f * Screen.height,
                buttonWidth,
                buttonHeight);
            if (GUI.Button(requestBannerRect, "Request\nBanner"))
            {
                this.RequestBanner();
            }

            Rect destroyBannerRect = new Rect(
                columnOnePosition,
                0.225f * Screen.height,
                buttonWidth,
                buttonHeight);
            if (GUI.Button(destroyBannerRect, "Destroy\nBanner"))
            {
                this.bannerView.Destroy();
            }

            Rect requestInterstitialRect = new Rect(
                columnOnePosition,
                0.4f * Screen.height,
                buttonWidth,
                buttonHeight);
            if (GUI.Button(requestInterstitialRect, "Request\nInterstitial"))
            {
                this.RequestInterstitial();
            }

            Rect showInterstitialRect = new Rect(
                columnOnePosition,
                0.575f * Screen.height,
                buttonWidth,
                buttonHeight);
            if (GUI.Button(showInterstitialRect, "Show\nInterstitial"))
            {
                this.ShowInterstitial(null, null);
            }

            Rect destroyInterstitialRect = new Rect(
                columnOnePosition,
                0.75f * Screen.height,
                buttonWidth,
                buttonHeight);
            if (GUI.Button(destroyInterstitialRect, "Destroy\nInterstitial"))
            {
                this.interstitial.Destroy();
            }

            Rect requestRewardedRect = new Rect(
                columnTwoPosition,
                0.05f * Screen.height,
                buttonWidth,
                buttonHeight);
            if (GUI.Button(requestRewardedRect, "Request\nRewarded Ad"))
            {
                this.CreateAndLoadRewardedAd();
            }

            Rect showRewardedRect = new Rect(
                columnTwoPosition,
                0.225f * Screen.height,
                buttonWidth,
                buttonHeight);
            if (GUI.Button(showRewardedRect, "Show\nRewarded Ad"))
            {
                if (rewardAds != null && rewardAds.Count > 0 && rewardAds[0] != null)
                {
                    this.ShowRewardedAd(rewardAds[0].Name, null, null, null);
                }
            }

            Rect textOutputRect = new Rect(
                columnTwoPosition,
                0.925f * Screen.height,
                buttonWidth,
                0.05f * Screen.height);
            GUI.Label(textOutputRect, outputMessage);
        }

        private void OnDestroy()
        {
            if (interstitial != null)
            {
                interstitial.Destroy();
            }

            if (bannerView != null)
            {
                bannerView.Destroy();
            }
        }
#endregion regular

#region banner
        public void RequestBanner(AdPosition bannerPosition)
        {
            this.bannerPosition = bannerPosition;
            RequestBanner();
        }

        public void RequestBanner()
        {
#if UNITY_ANDROID
            string adUnitId = bannerAdUnitIdAndroid;
#elif UNITY_IPHONE
        string adUnitId = bannerAdUnitIdIos;
#else
        string adUnitId ="unexpected_platform";
#endif
            // Clean up banner ad before creating a new one.
            if (bannerView != null)
            {
                bannerView.Destroy();
            }

            bannerView = new BannerView(adUnitId, AdSize.Banner, bannerPosition);

            // Called when an ad request has successfully loaded.
            bannerView.OnAdLoaded += BannerOnAdLoadedHandler;
            // Called when an ad request failed to load.
            bannerView.OnAdFailedToLoad += BannerOnAdFailedToLoadHandler;
            // Called when an ad is clicked.
            bannerView.OnAdOpening += BannerOnAdOpenedHandler;
            // Called when the user returned from the app after an ad click.
            bannerView.OnAdClosed += BannerOnAdClosedHandler;
            // Called when the ad click caused the user to leave the application.
            bannerView.OnAdLeavingApplication += BannerOnAdLeavingApplicationHandler;

            // Load the banner with the request.
            if (customBannerRequest)
            {
                bannerView.LoadAd(this.CreateAdRequest());
            }
            else
            {
                // Create an empty ad request.
                AdRequest request = new AdRequest.Builder().Build();
                bannerView.LoadAd(request);
            }
        }

        private void BannerOnAdLoadedHandler(object sender, EventArgs args)
        {
            Debug.Log("Banner OnAdLoaded event received.");
        }

        private void BannerOnAdFailedToLoadHandler(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("Banner failed to load: " + args.Message);
        }

        private void BannerOnAdOpenedHandler(object sender, EventArgs args)
        {
            Debug.Log("Banner opened");
        }

        private void BannerOnAdClosedHandler(object sender, EventArgs args)
        {
            Debug.Log("Banner OnAdClosed event received.");
        }

        private void BannerOnAdLeavingApplicationHandler(object sender, EventArgs args)
        {
            Debug.Log("Banner OnAdLeavingApplication event received.");
        }

        public void HideBanner()
        {
            if (bannerView != null) bannerView.Hide();
        }

        public void ShowBanner()
        {
            if (bannerView != null) bannerView.Show();
        }
#endregion banner

#region interstitial
        private void RequestInterstitial()
        {
#if UNITY_ANDROID
            string adUnitId = interstitialAdUnitIdAndroid;
#elif UNITY_IPHONE
            string adUnitId = interstitialAdUnitIdIos;
#else
            string adUnitId = "unexpected_platform";
#endif
            if (interstitial != null)
            {
                interstitial.Destroy();
            }

            Debug.Log("RequestIntVideo: " + adUnitId);
            // Initialize an InterstitialAd.
            interstitial = new InterstitialAd(adUnitId);


            // Called when an ad request has successfully loaded.
            interstitial.OnAdLoaded += InterstitialAdLoadedHandler;
            // Called when an ad request failed to load.
            interstitial.OnAdFailedToLoad += InterstitialAdFailedToLoadHandler;
            // Called when an ad is shown.
            interstitial.OnAdOpening += InterstitialAdOpenedHandler;
            // Called when the ad is closed.
            interstitial.OnAdClosed += InterstitialAdClosedHandler;
            // Called when the ad click caused the user to leave the application.
            interstitial.OnAdLeavingApplication += InterstitialAdLeavingApplicationHandler;

            if (customInterstitialRequest)
            {
                interstitial.LoadAd(CreateAdRequest());
            }
            else
            {
                // Create an empty ad request.
                AdRequest request = new AdRequest.Builder().Build();
                // Load the interstitial with the request.
                interstitial.LoadAd(request);
            }
        }

        private void InterstitialAdLoadedHandler(object sender, EventArgs args)
        {
            Debug.Log("Interstitial loaded event received.");
        }

        private void InterstitialAdFailedToLoadHandler(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("Interstitial failed to load event received: " + args.Message);
            StartCoroutine(NewInterstitialRequest());
        }

        private void InterstitialAdOpenedHandler(object sender, EventArgs args)
        {
            interstitialOpenedCallBack?.Invoke();
            Debug.Log("Interstitial opened event received.");
        }

        private void InterstitialAdClosedHandler(object sender, EventArgs args)
        {
            interstitialClosedCallBack?.Invoke();
            Debug.Log("Interstitial closed event received.");
            StartCoroutine(NewInterstitialRequest());
        }

        private void InterstitialAdLeavingApplicationHandler(object sender, EventArgs args)
        {
            Debug.Log("Interstitial OnAdLeavingApplication event received.");
        }

        private IEnumerator NewInterstitialRequest()
        {
            yield return new WaitForSeconds(3f);
            RequestInterstitial();
        }

        public void ShowInterstitial(Action interstitialOpenedCallBack, Action interstitialClosedCallBack)
        {
            this.interstitialOpenedCallBack = interstitialOpenedCallBack;
            this.interstitialClosedCallBack = interstitialClosedCallBack;
            if (interstitial.IsLoaded())
            {
                Debug.Log("show loaded video");
                interstitial.Show();
            }
            else
            {
                Debug.Log("not loaded");
            }
        }
#endregion interstitial

#region rewarded ad
        private void CreateAndLoadRewardedAd()
        {
            if (rewardAds == null || rewardAds.Count == 0) return;
            foreach (var item in rewardAds)
            {
                if (item != null) item.CreateAndLoadRewardedAd();
            }
        }

        public void ShowRewardedAd(string adName, Action rewardedOpenedCallBack, Action rewardedClosedCallBack, Action<bool, string, double> rewardCallBack)
        {
            if (rewardAds == null || rewardAds.Count == 0) return;

            string defaultAdName = "default";
            if (string.IsNullOrEmpty(adName)) adName = defaultAdName;

            RewardAd rA = null;
            foreach (var item in rewardAds)
            {
                if (String.Equals(item.Name, adName, StringComparison.Ordinal))
                    rA = item;
            }

            if (rA == null && String.Equals(adName, defaultAdName, StringComparison.Ordinal))
            {
                rA = rewardAds[0];
            }

            if (rA == null)
            {
                Debug.Log("Rearded ad: " + adName + " not exist");
                return;
            }
            rA.ShowRewardedAd(rewardedOpenedCallBack, rewardedClosedCallBack, rewardCallBack);
        }
#endregion revarded ad

#region custom ad request
        // Returns an ad request with custom ad targeting.
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
                .AddTestDevice(AdRequest.TestDeviceSimulator)
                .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
                .AddKeyword("game")
                .SetGender(Gender.Male)
                .SetBirthday(new DateTime(1985, 1, 1))
                .TagForChildDirectedTreatment(false)
                .AddExtra("color_bg", "9B30FF")
                .Build();
        }
#endregion custom ad request
#else
        public static AdsControl Instance;

        #region regular
        private void Awake()
        {
            if (Instance) Destroy(gameObject);
            else
            {
                Instance = this;
            }
        }
        #endregion regular

        public void ShowInterstitial(Action interstitialOpenedCallBack, Action interstitialClosedCallBack)
        {
            Debug.Log("ADD ADDGADS sripting symbol in project settings");
        }

        public void ShowRewardedAd(string adName, Action rewardedOpenedCallBack, Action rewardedClosedCallBack, Action<bool, string, double> rewardCallBack)
        {
            Debug.Log("ADD ADDGADS sripting symbol in project settings");
        }

        public void HideBanner()
        {
            Debug.Log("ADD ADDGADS sripting symbol in project settings");
        }

        public void ShowBanner()
        {
            Debug.Log("ADD ADDGADS sripting symbol in project settings");
        }
#endif
    }

    [Serializable]
    public class RewardAd
    {
        [SerializeField]
        private string name = "rewardedad";
        [SerializeField]
        private string adUnitIdAndroid = "ca-app-pub-3940256099942544/5224354917";  // test
        [SerializeField]
        private string adUnitIdIOS = "ca-app-pub-3940256099942544/1712485313";      // test

#if ADDGADS
        public string Name { get { return name; } }
        private Action<bool, string, double> rewardCallBack;    //  <well, args.type or message, args.amount>
        private Action rewardedOpenedCallBack;                  //  MSound.SetSound(false); 
        private Action rewardedClosedCallBack;                  //  MSound.SetSound(true);

        private RewardedAd rewardedAd;
        private bool customRewardedAdRequest = false;

        private RewardAd()
        {
            name = "rewardedad";
            adUnitIdAndroid = "ca-app-pub-3940256099942544/5224354917";
            adUnitIdIOS = "ca-app-pub-3940256099942544/5224354917";
        }

        private string adUnitId = "";

        public void CreateAndLoadRewardedAd() //https://developers.google.com/admob/unity/rewarded-ads
        {
#if UNITY_ANDROID
            adUnitId = adUnitIdAndroid;
#elif UNITY_IPHONE
             adUnitId = adUnitIdIOS;
#else
             adUnitId = "unexpected_platform";
#endif
            Debug.Log("RequestRewardBasedVideo (adUnitId): " + adUnitId);

            // Create new rewarded ad instance.
            rewardedAd = new RewardedAd(adUnitId);

            // Called when an ad request has successfully loaded.
            rewardedAd.OnAdLoaded += RewardedAdLoadedHandler;
            // Called when an ad request failed to load.
            rewardedAd.OnAdFailedToLoad += RewardedAdFailedToLoadHandler;
            // Called when an ad is shown.
            rewardedAd.OnAdOpening += HandleRewardedAdOpened;
            // Called when an ad request failed to show.
            rewardedAd.OnAdFailedToShow += RewardedAdFailedToShowHandler;
            // Called when the user should be rewarded for interacting with the ad.
            rewardedAd.OnUserEarnedReward += RewardedAdUserEarnedRewardHandler;
            // Called when the ad is closed.
            rewardedAd.OnAdClosed += RewardedAdClosedHandler;

            if (customRewardedAdRequest)
            {
                rewardedAd.LoadAd(CreateAdRequest());
            }
            else
            {
                AdRequest request = new AdRequest.Builder().Build();
                rewardedAd.LoadAd(request);
            }
        }

        private void RewardedAdLoadedHandler(object sender, EventArgs args)
        {
            Debug.Log("Rewarded ad: " + ToString() + " - loaded event received");
        }

        private void RewardedAdFailedToLoadHandler(object sender, AdErrorEventArgs args)
        {
            Debug.Log("Reward ad :" + ToString() + " -  failed to load event received: " + args.Message);
            CreateAndLoadRewardedAd();
        }

        private void RewardedAdFailedToShowHandler(object sender, AdErrorEventArgs args)
        {
            Debug.Log("Rewarded ad: " + ToString() + " - failed to show event received with message: " + args.Message);
        }

        private void RewardedAdClosedHandler(object sender, EventArgs args)
        {
            Debug.Log("Rewarded ad: " + ToString() + " -  closed event received.");
            rewardedClosedCallBack?.Invoke();
            CreateAndLoadRewardedAd();
        }

        private void RewardedAdUserEarnedRewardHandler(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            rewardCallBack?.Invoke(true, type, amount);
            Debug.Log("Rewarded ad: " + ToString() + " -  earned reward event received for (amount): " + amount.ToString() + " ;type : " + type);
        }

        private void HandleRewardedAdOpened(object sender, EventArgs args)
        {
            rewardedOpenedCallBack?.Invoke();
            Debug.Log("Rewarded ad: " + ToString() + " -  opened event received");
        }

        public void ShowRewardedAd(Action rewardedOpenedCallBack, Action rewardedClosedCallBack, Action<bool, string, double> rewardCallBack)
        {
            this.rewardedOpenedCallBack = rewardedOpenedCallBack;
            this.rewardedClosedCallBack = rewardedClosedCallBack;
            this.rewardCallBack = rewardCallBack;

            if (rewardedAd.IsLoaded())
            {
                Debug.Log("show loaded video");
                rewardedAd.Show();
            }
            else
            {
                Debug.Log("Rewarded ad: " + ToString() + " -  not loaded");
                rewardCallBack?.Invoke(false, "Rewarded ad: " + ToString() + " -  not loaded", 0);
            }
        }

        #region custom ad request
        // Returns an ad request with custom ad targeting.
        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder()
                .AddTestDevice(AdRequest.TestDeviceSimulator)
                .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
                .AddKeyword("game")
                .SetGender(Gender.Male)
                .SetBirthday(new DateTime(1985, 1, 1))
                .TagForChildDirectedTreatment(false)
                .AddExtra("color_bg", "9B30FF")
                .Build();
        }

        public override string ToString()
        {
            return "rewarde ad name: " + name + ", (adUnitId): " + adUnitId;
        }
        #endregion custom ad request

#else
        public string Name { get { return ""; } }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(AdsControl))]
    public class AdsControlEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            GUIStyle sl = new GUIStyle(EditorStyles.label);
            sl.margin = new RectOffset(0,0,-15,-15);


            GUILayout.Space(16);
            GUILayout.Label("Android test ad units IDs ", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel("Banner:         ca-app-pub-3940256099942544/6300978111", sl);
            EditorGUILayout.SelectableLabel("Interstitial:   ca-app-pub-3940256099942544/1033173712", sl);
            EditorGUILayout.SelectableLabel("Rewarded Video: ca-app-pub-3940256099942544/5224354917", sl);
            GUILayout.Space(8);
            GUILayout.Label("IOS test ad units IDs ", EditorStyles.boldLabel);
            EditorGUILayout.SelectableLabel("Banner:         ca-app-pub-3940256099942544/2934735716", sl);
            EditorGUILayout.SelectableLabel("Interstitial:   ca-app-pub-3940256099942544/4411468910", sl);
            EditorGUILayout.SelectableLabel("Rewarded Video: ca-app-pub-3940256099942544/1712485313", sl);


            GUILayout.Space(8);
            GUILayout.Label("Admob links:", EditorStyles.boldLabel);
            if (LinkLabel(new GUIContent("Get Started")))
            {
                Application.OpenURL("https://developers.google.com/admob/unity/quick-start");
            }
            if (LinkLabel(new GUIContent("Banner Ads")))
            {
                Application.OpenURL("https://developers.google.com/admob/unity/banner");
            }
            if (LinkLabel(new GUIContent("Interstitial Ads")))
            {
                Application.OpenURL("https://developers.google.com/admob/unity/interstitial");
            }
            if (LinkLabel(new GUIContent("Rewarded Ads")))
            {
                Application.OpenURL("https://developers.google.com/admob/unity/rewarded-ads");
            }
            if (LinkLabel(new GUIContent("Test Ads")))
            {
                Application.OpenURL("https://developers.google.com/admob/unity/test-ads");
            }
        }

       private bool LinkLabel(GUIContent label, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(label, EditorStyles.linkLabel, options);

            Handles.BeginGUI();
            Handles.color = EditorStyles.linkLabel.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin, position.yMax), new Vector3(position.xMax, position.yMax));
            Handles.color = Color.white;
            Handles.EndGUI();

            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, label, EditorStyles.linkLabel);
        }
    }
#endif
}
/*
 * https://github.com/googleads/googleads-mobile-unity/blob/master/samples/HelloWorld/Assets/Scripts/GoogleMobileAdsDemoScript.cs
 * https://developers.google.com/admob/unity/interstitial
 * https://developers.google.com/admob/unity/banner
 * https://developers.google.com/admob/unity/rewarded-video
 */
