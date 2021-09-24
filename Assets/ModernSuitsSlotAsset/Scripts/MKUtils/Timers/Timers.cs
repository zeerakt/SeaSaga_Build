using System;
using System.Collections;
using UnityEngine;
using System.Globalization;
/*  how to use
  
    private GlobalTimer gTimer;
    private string lifeIncTimerName = "lifeinc";
    [Tooltip ("Time span to life increase")]
    [SerializeField]
    private int  lifeIncTime = 15; // 

    [Tooltip("Calc global time (between games)")]
    [SerializeField]
    private bool calcGlobalTime = true; // 
    private float currMinutes = 0;
    private float currSeconds = 0;

    void Start()
    {
        gTimer = new GlobalTimer(lifeIncTimerName, 0, 0, lifeIncTime, 0, !calcGlobalTime);
        gTimer.OnTickRestDaysHourMinSec += TickRestDaysHourMinSecHandler;
        gTimer.OnTimePassed += TimePassed;
    }

    void OnDestroy()
    {
        gTimer.OnTickRestDaysHourMinSec -= TickRestDaysHourMinSecHandler;
        gTimer.OnTimePassed -= TimePassed;
    }

    void Update()
    {
        gTimer.Update();
    }
 
#region timerhandlers
    private void TickRestDaysHourMinSecHandler(int d, int h, int m, float s)
    {
        currMinutes = m;
        currSeconds = s;
        RefresTimerText();
    }

    private void TimePassed()
    {
        BubblesPlayer.Instance.AddLifes(1);
        gTimer.Restart();
    }
#endregion timerhandlers

    private void RefresTimerText()
    {
        if (timerText) timerText.text = currMinutes.ToString("00") + ":" + currSeconds.ToString("00");
    }

*/

/* changes
  
    13.11.18
    add time span validation
        daySpan = Mathf.Max(0, daySpan);
        hoursSpan = Mathf.Max(0, hoursSpan);
        minutesSpan = Mathf.Max(0, minutesSpan);
        secondsSpan = Mathf.Max(0, secondsSpan);
    21.11.18
    add clamp   restTime = Mathf.Max(0, restTime);
        days = Mathf.Max(0, restTime.Days);
        hours = Mathf.Max(0, restTime.Hours);
        minutes = Mathf.Max(0, restTime.Minutes);
        seconds = Mathf.Max(0, restTime.Seconds + Mathf.RoundToInt(restTime.Milliseconds*0.001f));
    24.01.19  public static DateTime DTFromSring(string dtString)
              CustomProvider provider = new CustomProvider(CultureInfo.InvariantCulture);
    19.02.2019
        SessionTimer - only seconds ctor
        c# 6.0  ?.Invoke() events
	21.03.2019 (fix)
         TickRestDaysHourMinSecEvent?.Invoke(rest_days, rest_hours, rest_minutes, rest_seconds);
     24.04.2019
        - addtimspan to session timer
    25.06.2019
      -  if (!double.TryParse(PlayerPrefs.GetString(initTimeSaveKey), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out initTime))
	28.08.2019
		- GlobalTimer old    public Action TimePassedEvent; 
		- GlobalTimer new    public Action <double, double>TimePassedEvent; // <init time, realy full time>
    27.09.2019
		- GlobalTimer 
            - add Exist(string timerName)
            - add RemoveAllPrefs(string timerName) 
            - GetLastTick, GetStartTick, GetEndTick
            - remove Pause, Start, Restart 

 */

namespace Mkey
{
    /// <summary>
    /// Game timer, calc only ingame time
    /// </summary>
    public class SessionTimer
    {
        #region events
        public Action<float> TickPassedFullSecondsEvent;
        public Action<float> TickRestFullSecondsEvent;
        public Action<int, int, int, float> TickPassedDaysHourMinSecEvent;
        public Action<int, int, int, float> TickRestDaysHourMinSecEvent;
        public Action TimePassedEvent;
        #endregion events

        #region private variables
        private float lastTime = 0;
        private float passedTime = 0;
        private float passedTimeOld = 0;
        private bool pause = false;
        private bool firstUpdate = true;
        private int days = 0;
        private int hours = 0;
        private int minutes = 0;
        private float seconds = 0;
        private int rest_days = 0;
        private int rest_hours = 0;
        private int rest_minutes = 0;
        private float rest_seconds = 0;
        private float rest;
        private float restTime;
        #endregion private variables

        #region properties
        public bool IsTimePassed
        {
            get { return passedTime >= InitTime; }
        }

        public float InitTime
        {
            get; private set;
        }
        #endregion properties

        public void GetPassedTime(out int days, out int hours, out int minutes, out float seconds)
        {
            days = this.days;
            hours = this.hours;
            minutes = this.minutes;
            seconds = this.seconds;
        }

        public void GetRestTime(out int rest_days, out int rest_hours, out int rest_minutes, out float rest_seconds)
        {
            rest_days = this.rest_days;
            rest_hours = this.rest_hours;
            rest_minutes = this.rest_minutes;
            rest_seconds = this.rest_seconds;
        }

        public SessionTimer(float secondsSpan)
        {
            secondsSpan = Mathf.Max(0, secondsSpan);
            InitTime = secondsSpan;
            pause = true;
        }

        public void Start()
        {
            if (IsTimePassed) return;
            pause = false;
        }

        public void Pause()
        {
            pause = true;
        }

        public void Restart()
        {
            passedTime = 0;
            passedTimeOld = 0;
            pause = false;
            firstUpdate = true;
        }

        /// <summary>
        /// for timer update set Time.time param
        /// </summary>
        /// <param name="time"></param>
        public void Update(float gameTime)
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                lastTime = gameTime;

                // zero tick
                CalcTime();
                TickPassedFullSecondsEvent?.Invoke(passedTimeOld);
                TickRestFullSecondsEvent?.Invoke(InitTime - passedTimeOld);

                TickPassedDaysHourMinSecEvent?.Invoke(days, hours, minutes, seconds);
                TickRestDaysHourMinSecEvent?.Invoke(rest_days, rest_hours, rest_minutes, rest_seconds);
            }

            float dTime = gameTime - lastTime;
            lastTime = gameTime;
            if (pause) return;
            passedTime += dTime;

            // tick events
            if (passedTime - passedTimeOld >= 1.0f)
            {
                passedTimeOld = Mathf.Floor(passedTime);

                CalcTime();

                TickPassedFullSecondsEvent?.Invoke(passedTimeOld);
                TickRestFullSecondsEvent?.Invoke(InitTime - passedTimeOld);

                TickPassedDaysHourMinSecEvent?.Invoke(days, hours, minutes, seconds);
                TickRestDaysHourMinSecEvent?.Invoke(rest_days, rest_hours, rest_minutes, rest_seconds);
            }

            // time passed events
            if (IsTimePassed && !pause)
            {
                pause = true;
                TimePassedEvent?.Invoke();
            }
        }

        private void CalcTime()
        {
            days = (int)(passedTime / 86400f);
            rest = passedTime - days * 86400f;

            hours = (int)(rest / 3600f);
            rest = rest - hours * 3600f;

            minutes = (int)(rest / 60f);
            rest = rest - minutes * 60f;

            seconds = rest;

            restTime = InitTime - passedTime;
            restTime = Mathf.Max(0, restTime);

            rest_days = (int)(restTime / 86400f);
            rest = restTime - rest_days * 86400f;

            rest_hours = (int)(rest / 3600f);
            rest = rest - rest_hours * 3600f;

            rest_minutes = (int)(rest / 60f);
            rest = rest - rest_minutes * 60f;

            rest_seconds = rest;
        }
        
        public void AddTimeSpan(float secondsSpan)
        {
            if (IsTimePassed) return;
            secondsSpan = Mathf.Max(0, secondsSpan);
            InitTime += secondsSpan;
        }
    }

    /// <summary>
    /// Game timer, calc ingame time, sleep game time, time between games
    /// </summary>
    public class GlobalTimer
    {
        private string name = "timer_default";

        private DateTime startDT;
        private DateTime lastDT;
        private DateTime endDT;
        private DateTime currentDT;

        private string endTickSaveKey;
        private string startTickSaveKey;
        private string lastTickSaveKey;

        private static string startTickSavePrefix = "_startTick";
        private static string endTickSavePrefix = "_endTick";
        private static string lastTickSavePrefix = "_lastTick";

        private int days = 0;
        private int hours = 0;
        private int minutes = 0;
        private float seconds = 0;
        private int rest_days = 0;
        private int rest_hours = 0;
        private int rest_minutes = 0;
        private float rest_seconds = 0;
        private double dTimeSec = 0;

        #region events
        public Action<double> TickPassedSecondsEvent;
        public Action<double> TickRestSecondsEvent;
        public Action<int, int, int, float> TickPassedDaysHourMinSecEvent;
        public Action<int, int, int, float> TickRestDaysHourMinSecEvent;
        public Action <double, double>TimePassedEvent;
        #endregion events

        public bool IsTimePassed
        {
            get; private set;
        }

        /// <summary>
        /// Returns the elapsed time from the beginning
        /// </summary>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public void PassedTime(out int days, out int hours, out int minutes, out float seconds)
        {
            days = this.days;
            hours = this.hours;
            minutes = this.minutes;
            seconds = this.seconds;
        }

        /// <summary>
        /// Returns the remaining time to the end
        /// </summary>
        /// <param name="days"></param>
        /// <param name="hours"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        public void RestTime(out int rest_days, out int rest_hours, out int rest_minutes, out float rest_seconds)
        {
            rest_days = this.rest_days;
            rest_hours = this.rest_hours;
            rest_minutes = this.rest_minutes;
            rest_seconds = this.rest_seconds;
        }

        private void CalcTime()
        {
            TimeSpan passedTime = (!IsTimePassed) ? lastDT - startDT : endDT - startDT;
            days = passedTime.Days;
            hours = passedTime.Hours;
            minutes = passedTime.Minutes;
            seconds = passedTime.Seconds + Mathf.RoundToInt(passedTime.Milliseconds * 0.001f);

            TimeSpan restTime = (!IsTimePassed) ? endDT - lastDT : endDT - endDT;
            rest_days = Mathf.Max(0, restTime.Days);
            rest_hours = Mathf.Max(0, restTime.Hours);
            rest_minutes = Mathf.Max(0, restTime.Minutes);
            rest_seconds = Mathf.Max(0, restTime.Seconds + Mathf.RoundToInt(restTime.Milliseconds * 0.001f));
        }

        /// <summary>
        /// Create new timer
        /// </summary>
        /// <param name="timerName"></param>
        /// <param name="daySpan"> value > 0 </param>
        /// <param name="hoursSpan"> value 0 - 24 </param>
        /// <param name="minutesSpan"> value 0 - 60 </param>
        /// <param name="secondsSpan"> value 0 - 60 </param>
        /// <param name="removeOld">Remove old timer with timerName if exist</param>
        public GlobalTimer(string timerName, float daySpan, float hoursSpan, float minutesSpan, float secondsSpan)
        {
            name = timerName;
            startTickSaveKey = name + startTickSavePrefix;
            endTickSaveKey = name + endTickSavePrefix;
            lastTickSaveKey = name + lastTickSavePrefix;
            RemoveTimerPrefs();
            IsTimePassed = false;

            daySpan = Mathf.Max(daySpan, 0);
            hoursSpan = Mathf.Clamp(hoursSpan, 0, 24);
            minutesSpan = Mathf.Clamp(minutesSpan, 0, 60);
            secondsSpan = Mathf.Clamp(secondsSpan, 0, 60);
            double initTime = daySpan * 24.0 * 3600.0 + hoursSpan * 3600.0 + minutesSpan * 60.0 + secondsSpan;

            startDT = DateTime.Now;
            lastDT = DateTime.Now;
            endDT = startDT.AddSeconds(initTime);

            PlayerPrefs.SetString(startTickSaveKey, startDT.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString(endTickSaveKey, endDT.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString(lastTickSaveKey, lastDT.ToString(CultureInfo.InvariantCulture));

            Debug.Log("-------------------- new timer: " + name + " ; initTime: " + initTime + " ;startTD: " +startDT + " ;endTD: " + endDT + " -----------------------");
        }

        /// <summary>
        /// Continue existing timer
        /// </summary>
        /// <param name="timerName"></param>
        public GlobalTimer(string timerName)
        {
            name = timerName;
            if (!Exist(name)) return;

            startTickSaveKey = name + startTickSavePrefix;
            endTickSaveKey = name + endTickSavePrefix;
            lastTickSaveKey = name + lastTickSavePrefix;

            lastDT = DateTime.Now;
            startDT = DTFromSring(PlayerPrefs.GetString(startTickSaveKey));
            endDT = DTFromSring(PlayerPrefs.GetString(endTickSaveKey));
            Debug.Log("-------------------- continue timer: " + name + " ; end time: " + endDT + " ------------------------");
        }

        /// <summary>
        /// Timer update.
        /// </summary>
        /// <param name="time"></param>
        public void Update()
        {
            if (IsTimePassed) return;

            currentDT = DateTime.Now;

            dTimeSec = (currentDT - lastDT).TotalSeconds;
            if (dTimeSec>= 1.0)
            {
                // Debug.Log("dTime: " + dTime +" current: "+ currentDT.ToString() + " last: " + lastDT.ToString());
                lastDT = currentDT;
                PlayerPrefs.SetString(lastTickSaveKey, currentDT.ToString(CultureInfo.InvariantCulture));
                CalcTime();
                TickPassedSecondsEvent?.Invoke((currentDT - startDT).TotalSeconds);
                TickRestSecondsEvent?.Invoke((IsTimePassed) ? 0 : (endDT - currentDT).TotalSeconds);
                TickPassedDaysHourMinSecEvent?.Invoke(days, hours, minutes, seconds);
                TickRestDaysHourMinSecEvent?.Invoke(rest_days, rest_hours, rest_minutes, rest_seconds);
            }

            if (currentDT >= endDT)
            {
                RemoveTimerPrefs();
                IsTimePassed = true;
                TimePassedEvent?.Invoke((endDT - startDT).TotalSeconds, (currentDT - startDT).TotalSeconds);
            }
        }

        public static DateTime DTFromSring(string dtString)
        {
            if (string.IsNullOrEmpty(dtString)) return DateTime.Now;
            DateTime dateValue = DateTime.Now;

            CustomProvider provider = new CustomProvider(CultureInfo.InvariantCulture);
            {
                try
                {
                    dateValue = Convert.ToDateTime(dtString, provider);
                }
                catch (FormatException)
                {
                    Debug.Log(dtString + "--> Bad Format");
                }
                catch (InvalidCastException)
                {
                    Debug.Log(dtString + "--> Conversion Not Supported");
                }
            }
            return dateValue;
        }

        private double GetTimeSpanSeconds(DateTime dtStart, DateTime dtEnd)
        {
            return (dtEnd - dtStart).TotalSeconds;
        }

        private double GetTimeSpanSeconds(string dtStart, string dtEnd)
        {
            return (DTFromSring(dtEnd) - DTFromSring(dtStart)).TotalSeconds;
        }

        public void RemoveTimerPrefs()
        {
            if (PlayerPrefs.HasKey(startTickSaveKey))
            {
                PlayerPrefs.DeleteKey(startTickSaveKey);
            }
            if (PlayerPrefs.HasKey(endTickSaveKey))
            {
                PlayerPrefs.DeleteKey(endTickSaveKey);
            }
            if (PlayerPrefs.HasKey(lastTickSaveKey))
            {
                PlayerPrefs.DeleteKey(lastTickSaveKey);
            }
        }

        public static bool Exist(string timerName)
        {
            string endTickSaveKey = timerName + endTickSavePrefix;
            string startTickSaveKey = timerName + startTickSavePrefix;
            string lastTickSaveKey = timerName + lastTickSavePrefix;
            return PlayerPrefs.HasKey(endTickSaveKey) && PlayerPrefs.HasKey(startTickSaveKey) && PlayerPrefs.HasKey(lastTickSaveKey);
        }

        public static void RemoveTimerPrefs(string timerName)
        {
            string endTickSaveKey = timerName + endTickSavePrefix;
            string startTickSaveKey = timerName + startTickSavePrefix;
            string lastTickSaveKey = timerName + lastTickSavePrefix;

            if (PlayerPrefs.HasKey(startTickSaveKey))
            {
                PlayerPrefs.DeleteKey(startTickSaveKey);
            }
            if (PlayerPrefs.HasKey(endTickSaveKey))
            {
                PlayerPrefs.DeleteKey(endTickSaveKey);
            }
            if (PlayerPrefs.HasKey(lastTickSaveKey))
            {
                PlayerPrefs.DeleteKey(lastTickSaveKey);
            }
        }

        public static DateTime GetLastTick(string timerName)
        {
            if (!Exist(timerName)) return DateTime.MinValue;
            string lastTickSaveKey = timerName + lastTickSavePrefix;
            return DTFromSring(PlayerPrefs.GetString(lastTickSaveKey)); 
        }

        public static DateTime GetEndTick(string timerName)
        {
            if (!Exist(timerName)) return DateTime.MinValue;
            string endTickSaveKey = timerName + endTickSavePrefix;
            return DTFromSring(PlayerPrefs.GetString(endTickSaveKey));
        }

        public static DateTime GetStartTick(string timerName)
        {
            if (!Exist(timerName)) return DateTime.MinValue;
            string startTickSaveKey = timerName + startTickSavePrefix;
            return DTFromSring(PlayerPrefs.GetString(startTickSaveKey));
        }
    }

    public class CustomProvider : IFormatProvider
    {
        private string cultureName;

        public CustomProvider(string cultureName)
        {
            this.cultureName = cultureName;
        }

        public CustomProvider(CultureInfo cInfo)
        {
            cultureName = cInfo.Name;
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(DateTimeFormatInfo))
            {
                // Console.Write("(CustomProvider retrieved.) ");
                return new CultureInfo(cultureName).GetFormat(formatType);
            }
            else
            {
                return null;
            }
        }

    }
}
    

