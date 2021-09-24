using UnityEngine;
using System;
using UnityEngine.Events;

#if UNITY_EDITOR
    using UnityEditor;
#endif
namespace Mkey
{
    public class SessionLocalTimer : MonoBehaviour
    {
        [Tooltip("Timespan in seconds for timer")]
        [SerializeField]
        private float seconds = 20;
        [SerializeField]
        private bool autoRestart = true;
        [Tooltip("Start timer automatically  with gameobject")]
        [SerializeField]
        private bool autoStart = true;
        [Tooltip("Don't destroy timer between scenes")]
        [SerializeField]
        private bool dontDestroy = true;
        [Tooltip("If singleton - only one timer can exist during the game")]
        [SerializeField]
        private bool singleTon = true;
        [Tooltip("Output data to console")]
        [SerializeField]
        private bool debugTime = true;
        private SessionTimer sT;
        private static SessionLocalTimer Instance;

        #region events
        public Action<float> TickPassedFullSecondsEvent;
        public Action<float> TickRestFullSecondsEvent;
        public Action<int, int, int, float> TickPassedDaysHourMinSecEvent;
        public Action<int, int, int, float> TickRestDaysHourMinSecEvent;
        public UnityEvent TimePassedEvent;
        #endregion events

        #region regular
        private void Awake()
        {
            if(Instance && singleTon)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            if(dontDestroy) DontDestroyOnLoad(gameObject);
            CreateTimer();
        }

        private void Start()
        {
           
            if (autoStart) sT.Start();
        }

        void Update()
        {
            sT.Update(Time.time);
        }

        private void OnDestroy()
        {
            sT.TickPassedFullSecondsEvent -= TickPassedFullSecondsHandler;
            sT.TickRestFullSecondsEvent -= TickRestFullSecondsHandler;
            sT.TickPassedDaysHourMinSecEvent -= TickPassedDaysHourMinSecHandler;
            sT.TickRestDaysHourMinSecEvent -= TickRestDaysHourMinSecHandler;
            sT.TimePassedEvent -= FullTimePassedHandler;
        }

        private void OnValidate()
        {
            seconds = Mathf.Max(0, seconds);
        }
        #endregion regular

        #region timer control
        /// <summary>
        /// Start created timer first time or after pause
        /// </summary>
        public void StartTimer()
        {
            if (sT != null) sT.Start();
        }

        /// <summary>
        /// Create new timer and set event handlers
        /// </summary>
        private void CreateTimer()
        {
            sT = new SessionTimer(seconds);
            sT.TickPassedFullSecondsEvent += TickPassedFullSecondsHandler;
            sT.TickRestFullSecondsEvent += TickRestFullSecondsHandler;
            sT.TickPassedDaysHourMinSecEvent += TickPassedDaysHourMinSecHandler;
            sT.TickRestDaysHourMinSecEvent += TickRestDaysHourMinSecHandler;
            sT.TimePassedEvent += FullTimePassedHandler;
        }

        /// <summary>
        /// Set timer to pause state
        /// </summary>
        public void PauseTimer()
        {
            if (sT != null) sT.Pause();
        }

        /// <summary>
        /// Start timer from zero
        /// </summary>
        public void RestartTimer()
        {
            if (sT != null) sT.Restart();
        }
        #endregion timer control

        #region timer handlers
        private void TickPassedFullSecondsHandler(float fullSeconds)
        {
            if(debugTime) Debug.Log("Passed full seconds: " + fullSeconds);
            TickPassedFullSecondsEvent?.Invoke(fullSeconds);
        }

        private void TickRestFullSecondsHandler(float fullSeconds)
        {
            if (debugTime) Debug.Log("Rest seconds: " + fullSeconds);
            TickRestFullSecondsEvent?.Invoke(fullSeconds);
        }

        private void TickPassedDaysHourMinSecHandler(int days, int hours, int minutes, float seconds)
        {
            if (debugTime)  Debug.Log("Passed days: " + days + " ;hours: " + hours + " ;minutes: " + minutes + " ;seconds: " + seconds);
            TickPassedDaysHourMinSecEvent?.Invoke(days, hours, minutes, seconds);
        }

        private void TickRestDaysHourMinSecHandler(int days, int hours, int minutes, float seconds)
        {
            if (debugTime) Debug.Log("Rest days: " + days + " ;hours: " + hours + " ;minutes: " + minutes + " ;seconds: " + seconds);
            TickRestDaysHourMinSecEvent?.Invoke(days, hours, minutes, seconds);
        }

        private void FullTimePassedHandler()
        {
            if (debugTime) Debug.Log("Time full passed");
            if (sT != null && autoRestart) sT.Restart();
            TimePassedEvent?.Invoke();
        }
        #endregion timer handlers
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SessionLocalTimer))]
    public class SessionSecondTimerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SessionLocalTimer ssT = (SessionLocalTimer) target;
            DrawDefaultInspector();
            if (EditorApplication.isPlaying)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Pause"))
                {
                    if (ssT != null) ssT.PauseTimer();
                }

                if (GUILayout.Button("Restart"))
                {
                    if (ssT != null) ssT.RestartTimer();
                }

                if (GUILayout.Button("Start"))
                {
                    if (ssT != null) ssT.StartTimer();
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Goto play mode for test");
            }
        }
    }
#endif
}