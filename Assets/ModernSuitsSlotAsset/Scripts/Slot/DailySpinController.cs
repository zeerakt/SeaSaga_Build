using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace Mkey
{
	public class DailySpinController : MonoBehaviour
	{
        [SerializeField]
        private PopUpsController screenPrefab;
        [SerializeField]
        private TextMesh timerText;
        [HideInInspector]
        public UnityEvent TimePassEvent;
        [SerializeField]
        private MkeyFW.FortuneWheelInstantiator fwInstantiator;

        #region temp vars
        private int hours = 24;
        private int minutes = 0; // for test
        private GlobalTimer gTimer;
        private PopUpsController screen;
        private string timerName = "dailySpinTimer";
        private bool debug = false;
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        private SlotSoundController MSound { get { return SlotSoundController.Instance; } }
        #endregion temp vars

        #region properties
        public float RestDays { get; private set; }
        public float RestHours { get; private set; }
        public float RestMinutes { get; private set; }
        public float RestSeconds { get; private set; }
        public bool IsWork { get; private set; }
        public static DailySpinController Instance { get; private set; }
        public static bool HaveDailySpin { get; private set; }
        #endregion properties

        #region regular
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            Debug.Log("Awake: " + name);
        }

        private void Start()
        {
            if (timerText) timerText.text = "";
            IsWork = false;

            // set fortune wheel event handlers
            fwInstantiator.SpinResultEvent += (coins, isBigWin) =>
            {
                MPlayer.AddCoins(coins);
                HaveDailySpin = false;
                StartNewTimer();
                if (fwInstantiator.MiniGame) fwInstantiator.MiniGame.SetBlocked(!HaveDailySpin, true);
            }; 

            fwInstantiator.CreateEvent +=(MkeyFW.WheelController wc)=>
            {
                if (screenPrefab) screen = MGui.ShowPopUp(screenPrefab);
                wc.SetBlocked(!HaveDailySpin, false);
            };

            fwInstantiator.CloseEvent += () => 
            {
                if (screen) screen.CloseWindow();
                if (timerText) timerText.text = "";
            };


            if (!HaveDailySpin)
            {
                // check existing timer and  last tick
                if (GlobalTimer.Exist(timerName))
                {
                    StartExistingTimer();
                }
                else
                {
                    if (debug) Debug.Log("timer not exist: " + timerName);
                    StartNewTimer();
                }
            }
        }

        private void Update()
        {
            if (IsWork)
                gTimer.Update();
        }
        #endregion regular

        #region timerhandlers
        private void TickRestDaysHourMinSecHandler(int d, int h, int m, float s)
        {
            RestDays = d;
            RestHours = h;
            RestMinutes = m;
            RestSeconds = s;
            if(timerText && fwInstantiator.MiniGame) timerText.text = String.Format("{0:00}:{1:00}:{2:00}", h, m, s);
        }

        private void TimePassedHandler(double initTime, double realyTime)
        {
            IsWork = false;
            if (timerText) timerText.text = "";
            HaveDailySpin = true;
            Debug.Log("time passed daily spin");
            if (fwInstantiator.MiniGame)
            {
                Debug.Log("time passed daily spin - > start mini game");
                fwInstantiator.MiniGame.SetBlocked(!HaveDailySpin, false);
            }

            if (debug) Debug.Log("daily spin timer time passed, have daily spin");
            TimePassEvent?.Invoke();
        }
        #endregion timerhandlers

        #region timers
        private void StartNewTimer()
        {
            if (debug) Debug.Log("start new daily spin timer");
            TimeSpan ts = new TimeSpan(hours, minutes, 0);
            gTimer = new GlobalTimer(timerName, ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
            gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
            gTimer.TimePassedEvent += TimePassedHandler;
            IsWork = true;
        }

        private void StartExistingTimer()
        {
            if (debug) Debug.Log("start existing daily spin timer");
            gTimer = new GlobalTimer(timerName);
            gTimer.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
            gTimer.TimePassedEvent += TimePassedHandler;
            IsWork = true;
        }
        #endregion timers

        public void OpenSpinGame()
        {
            fwInstantiator.Create(false);
            if (fwInstantiator.MiniGame)
            {
                fwInstantiator.MiniGame.BackGroundButton.clickEvent.AddListener(()=> { fwInstantiator.ForceClose(); });
            }
        }

        public void CloseSpinGame()
        {
            fwInstantiator.Close();
        }

        public void ResetData()
        {
            GlobalTimer.RemoveTimerPrefs(timerName);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DailySpinController))]
    public class DailySpinControllerEditor : Editor
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
                        DailySpinController t = (DailySpinController)target;
                        t.ResetData();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
#endif
}
