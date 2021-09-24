using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
    public enum OrderIAP { RANDOM, SEQUENCED}
    public class DealSaleController : MonoBehaviour
    {
        [Header("Deal time spans: ", order = 1)]
        [SerializeField]
        private TimeSpanHolder workingTimeSpan;
        [SerializeField]
        private TimeSpanHolder pausedTimeSpan;
        [SerializeField]
        private OrderIAP IAP;

        #region events
        public Action<int, int, int, float> WorkingDealTickRestDaysHourMinSecEvent;
        public Action<int, int, int, float> PausedDealTickRestDaysHourMinSecEvent;
        public Action<double, double> WorkingDealTimePassedEvent;
        public Action<double, double> PausedDealTimePassedEvent;
        public Action WorkingDealStartEvent;
        public Action PausedDealStartEvent;
        #endregion events

        #region temp vars
        private bool debug = false;
        private string dealWorkingTimerName = "dealWorking_Timer";
        private string dealPausedTimerName = "dealPaused_Timer";
        private string dealIAPsaveKey = "dealiapkey";
        private GlobalTimer gTimer;
        private Purchaser MPurchaser {get { return Purchaser.Instance; } }
        #endregion temp vars

        #region properties
        public float RestDays { get; private set; }
        public float RestHours { get; private set; }
        public float RestMinutes { get; private set; }
        public float RestSeconds { get; private set; }
        public bool IsWork { get; private set; }
        public static DealSaleController Instance;
        public bool IsDealTime { get; private set; }
        #endregion properties

        #region regular
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
		
		private void Start()
		{
            if (GlobalTimer.Exist(dealWorkingTimerName) && GlobalTimer.Exist(dealPausedTimerName))
            {
                GlobalTimer.RemoveTimerPrefs(dealWorkingTimerName);
                GlobalTimer.RemoveTimerPrefs(dealPausedTimerName);
            }

            if (!GlobalTimer.Exist(dealWorkingTimerName) && !GlobalTimer.Exist(dealPausedTimerName))
            {
                StartNewWorkingTimer();
            }
            else if (GlobalTimer.Exist(dealWorkingTimerName))
            {
                StartExistingWorkingTimer();
            }
            else if (GlobalTimer.Exist(dealPausedTimerName))
            {
                StartExistingPausedTimer();
            }
        }

		private void Update()
		{
            if (IsWork)
            {
                gTimer.Update();
            }
        }
        #endregion regular

        #region timerhandlers
        private void WorkingDealTickRestDaysHourMinSecHandler(int d, int h, int m, float s)
        {
            RestDays = d;
            RestHours = h;
            RestMinutes = m;
            RestSeconds = s;
            WorkingDealTickRestDaysHourMinSecEvent?.Invoke(d,h,m,s);
        }

        private void WorkingDealTimePassedHandler(double initTime, double realyTime)
        {
            if (debug) Debug.Log("working deal time passed");
            IsWork = false;
            WorkingDealTimePassedEvent?.Invoke(initTime, realyTime);
            StartNewPausedTimer();
        }

        private void PausedDealTickRestDaysHourMinSecHandler(int d, int h, int m, float s)
        {
            RestDays = d;
            RestHours = h;
            RestMinutes = m;
            RestSeconds = s;
            PausedDealTickRestDaysHourMinSecEvent?.Invoke(d, h, m, s);
        }

        private void PausedDealTimePassedHandler(double initTime, double realyTime)
        {
            if (debug) Debug.Log("stop deal time passed");
            IsWork = false;
            PausedDealTimePassedEvent?.Invoke(initTime, realyTime);
            StartNewWorkingTimer();
        }
        #endregion timerhandlers

        #region timers start
        private void StartNewWorkingTimer()
        {
            IsDealTime = true;
            SetIAP();
            StartNewTimer(dealWorkingTimerName, workingTimeSpan.GetTimeSpan(), WorkingDealTickRestDaysHourMinSecHandler, WorkingDealTimePassedHandler, WorkingDealStartEvent);
        }

        private void StartExistingWorkingTimer()
        {
            IsDealTime = true;
            StartExistingTimer(dealWorkingTimerName, WorkingDealTickRestDaysHourMinSecHandler, WorkingDealTimePassedHandler, WorkingDealStartEvent);
        }

        private void StartNewPausedTimer()
        {
            IsDealTime = false;
            StartNewTimer(dealPausedTimerName, pausedTimeSpan.GetTimeSpan(), PausedDealTickRestDaysHourMinSecHandler, PausedDealTimePassedHandler, PausedDealStartEvent);
        }

        private void StartExistingPausedTimer()
        {
            IsDealTime = false;
            StartExistingTimer(dealPausedTimerName, PausedDealTickRestDaysHourMinSecHandler, PausedDealTimePassedHandler, PausedDealStartEvent);
        }

        private void StartNewTimer(string timerName, TimeSpan ts, Action<int, int, int, float> TickRestDaysHourMinSecAction, Action<double, double> TimePassedAction, Action StartAction)
        {
            if (debug) Debug.Log("start new timer: " + timerName);
            gTimer = new GlobalTimer(timerName, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            if (TickRestDaysHourMinSecAction != null) gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecAction;
            if (TimePassedAction != null) gTimer.TimePassedEvent += TimePassedAction;
            StartAction?.Invoke();
            IsWork = true;
        }

        private void StartExistingTimer(string timerName, Action<int, int, int, float> TickRestDaysHourMinSecAction, Action<double, double> TimePassedAction, Action StartAction)
        {
            if (debug) Debug.Log("start existing timer: " + timerName);
            gTimer = new GlobalTimer(timerName);
            if (TickRestDaysHourMinSecAction != null) gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecAction;
            if (TimePassedAction != null) gTimer.TimePassedEvent += TimePassedAction;
            StartAction?.Invoke();
            IsWork = true;
        }
        #endregion timers start

        #region order
        private void SetIAP()
        {
            int order = PlayerPrefs.GetInt(dealIAPsaveKey, 0);

            if (!MPurchaser) return;
            List<ShopThingDataReal> products = GetAllDealIAPs();
            order = Mathf.Clamp(order, 0, products.Count - 1);

            if (IAP == OrderIAP.SEQUENCED)
            {
                order++;
                order =(int) Mathf.Repeat(order, products.Count);
            }
            else if(IAP== OrderIAP.RANDOM)
            {
                order = UnityEngine.Random.Range(0, products.Count);
            }
            PlayerPrefs.SetInt(dealIAPsaveKey, order);
        }

        public ShopThingDataReal GetIAP ()
        {
            if (!MPurchaser) return null;
            int order = PlayerPrefs.GetInt(dealIAPsaveKey, 0);

            List<ShopThingDataReal> products = GetAllDealIAPs();

            if (products.Count == 0)
            {
                return null;               
            }

            order = Mathf.Clamp(order, 0, products.Count-1);
            return products[order];
        }

        private List<ShopThingDataReal> GetAllDealIAPs()
        {
            List<ShopThingDataReal> products = new List<ShopThingDataReal>();
            if (MPurchaser.consumable != null && MPurchaser.consumable.Length > 0) products.AddRange(MPurchaser.consumable);
            if (MPurchaser.nonConsumable != null && MPurchaser.nonConsumable.Length > 0) products.AddRange(MPurchaser.nonConsumable);
            if (MPurchaser.subscriptions != null && MPurchaser.subscriptions.Length > 0) products.AddRange(MPurchaser.subscriptions);

            if (products.Count == 0) return  new List<ShopThingDataReal>();

            products.RemoveAll((thing) => { return (thing == null || thing.shopType != RealShopType.DealCoins); });

            Debug.Log("deal sale products count: " + products.Count);
            return products;
        }
        #endregion order

        public void ResetData()
        {
            IsDealTime = true;
            PlayerPrefs.DeleteKey(dealIAPsaveKey);
            GlobalTimer.RemoveTimerPrefs(dealWorkingTimerName);
            GlobalTimer.RemoveTimerPrefs(dealPausedTimerName);
        }
    }

    [Serializable]
    public class TimeSpanHolder
    {
        public int days = 0;
        public int hours = 3;
        public int minutes = 0;
        public int seconds = 0;

        public TimeSpan GetTimeSpan()
        {
            return new TimeSpan(days, hours, minutes, seconds);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DealSaleController))]
    public class DealSaleControllerEditor : Editor
    {
        private bool test = true;
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (!EditorApplication.isPlaying)
            {
                if (test = EditorGUILayout.Foldout(test, "Test"))
                {
                    EditorGUILayout.BeginHorizontal("box");
                    if (GUILayout.Button("Reset Data"))
                    {
                        DealSaleController t = (DealSaleController)target;
                        t.ResetData();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
#endif
}
