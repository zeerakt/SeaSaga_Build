using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class SlotControls : MonoBehaviour
    {
        #region main references
        [SerializeField]
        private WarningMessController megaJackPotWinPuPrefab;
        [SerializeField]
        private WarningMessController miniJackPotWinPuPrefab;
        [SerializeField]
        private WarningMessController maxiJackPotWinPuPrefab;

        [SerializeField]
        private GameObject megaJackPotWinPrefab;
        [SerializeField]
        private GameObject maxiJackPotWinPrefab;
        [SerializeField]
        private GameObject miniJackPotWinPrefab;
        [SerializeField]
        private JackPotController jackPotController;
        #endregion main references

        #region default
        [Space(8)]
        [Header("Jackpot coins")]
        [Tooltip("Mini jackpot sum start value")]
        [SerializeField]
        private int miniStart = 10;

        [Tooltip("Maxi jackpot sum start value")]
        [SerializeField]
        private int maxiStart = 20;

        [Tooltip("Mega jackpot sum start value")]
        [SerializeField]
        private int megaStart = 1000;

        [Space(8)]
        [Tooltip("Check if you want to save coins, level, progress, facebook gift flag, sound settings")]
        [SerializeField]
        private bool saveData = false;

        [Tooltip("Default max line bet, min =1")]
        [SerializeField]
        private int maxLineBet = 20;

        [Tooltip("Default line bet at start, min = 1")]
        [SerializeField]
        private int defLineBet = 1;

        [Tooltip("Check if you want to play auto all free spins")]
        [SerializeField]
        private bool autoPlayFreeSpins = true;

        [Tooltip("Default auto spins count, min = 1")]
        [SerializeField]
        private int defAutoSpins = 1;

        [Tooltip("Max value of auto spins, min = 1")]
        [SerializeField]
        private int maxAutoSpins = int.MaxValue;
        #endregion default

        #region output
        [Space(16, order = 0)]
        [SerializeField]
        private Text LineBetSumText;
        [SerializeField]
        private Text TotalBetSumText;
        [SerializeField]
        private Text LinesCountText;
        [SerializeField]
        private Text FreeSpinText;
        [SerializeField]
        private Text FreeSpinCountText;
        [SerializeField]
        private Text AutoSpinsCountText;
        [SerializeField]
        private Text InfoText;
     //   [SerializeField]
     //   private Text balanceAmountText;
        [SerializeField]
        private Text WinAmountText;
        [SerializeField]
        private Text MegaJackpotAmountText;
        [SerializeField]
        private Text MiniJackpotAmountText;
        [SerializeField]
        private Text MaxiJackpotAmountText;
        #endregion output

        [SerializeField]
        private Button spinButton;
        [SerializeField]
        private SceneButton autoSpinButton;

        #region features
        [SerializeField]
        private HoldFeature hold;
        #endregion features

        #region keys
        private static string Prefix { get { return SceneLoader.GetCurrentSceneName(); } }
        private static string SaveMiniJackPotKey { get { return Prefix + "_mk_slot_minijackpot"; } }// current  mini jackpot
        private static string SaveMaxiJackPotKey { get { return Prefix + "_mk_slot_maxijackpot"; } } // current  maxi jackpot
        private static string SaveMegaJackPotKey { get { return Prefix + "_mk_slot_megajackpot"; } } // current  mega jackpot
        private static string SaveAutoSpinsKey {get { return Prefix + "_mk_slot_autospins"; } } // current auto spins
        #endregion keys

        #region temp vars
        private float levelxp;
        private float oldLevelxp;
        private int levelTweenId;
        private SceneButton[] buttons;
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGUI { get { return SlotGuiController.Instance; } }
        private TweenIntValue balanceTween;
        private TweenIntValue winCoinsTween;
        private TweenIntValue infoCoinsTween;
        private WarningMessController megaJackPotWinPu;
        private WarningMessController miniJackPotWinPu;
        private WarningMessController maxiJackPotWinPu;
        private GameObject megaJackPotWinGO;
        private GameObject maxiJackPotWinGO;
        private GameObject miniJackPotWinGO;
        private string coinsFormat = "# ### ### ### ###";
        private SpinButtonBehavior spinButtonBehavior;
        #endregion temp vars

        #region references
        [SerializeField]
        private SlotController slot;
        [SerializeField]
		private LinesController linesController;
        #endregion references

        #region events
        public Action<int> ChangeMiniJackPotEvent;
        public Action<int> ChangeMaxiJackPotEvent;
        public Action<int> ChangeMegaJackPotEvent;
        public Action<int> LoadMiniJackPotEvent;
        public Action<int> LoadMaxiJackPotEvent;
        public Action<int> LoadMegaJackPotEvent;

        public Action<int> ChangeTotalBetEvent;
        public Action<int> ChangeLineBetEvent;
        public Action<int, bool> ChangeSelectedLinesEvent;

        public Action<int> ChangeFreeSpinsEvent;
        public Action<int> ChangeAutoSpinsEvent;
        public Action<int, int> ChangeAutoSpinsCounterEvent; // 
        public Action<bool> ChangeAutoStateEvent;
        #endregion events

        #region properties
        public bool SaveData
        {
            get { return saveData; }
        }

        public int MiniJackPotStart
        {
            get { return miniStart; }
        }

        public int MaxiJackPotStart
        {
            get { return maxiStart; }
        }

        public int MegaJackPotStart
        {
            get { return megaStart; }
        }

        public int LineBet
        {
            get; private set;
        }

        public int TotalBet
        {
            get { return LineBet * SelectedLinesCount * HoldMultipler; }
        }

        public int HoldMultipler
        {
            get { return (hold && hold.enabled && hold.gameObject.activeSelf && hold.UseMultiplier) ? hold.GetMultiplier
            () :1; }
        }

        public int SelectedLinesCount
        {
            get; private set;
        }

        public bool AnyLineSelected
        {
            get { return SelectedLinesCount > 0; }
        }

        public int FreeSpins
        {
            get; private set;
        }

        public bool HasFreeSpin
        {
            get { return FreeSpins > 0; }
        }

        public bool AutoPlayFreeSpins
        {
            get { return autoPlayFreeSpins; }
        }

        public bool Auto { get; private set; }

        public int AutoSpinsCounter { get; private set; }

        public HoldFeature Hold { get { return hold; } }

        public bool UseHold
        {
            get { return (hold && hold.enabled && hold.gameObject.activeSelf); }
        }
        #endregion properties

        #region saved properties
        public int MiniJackPot
        {
            get; private set;
        }

        public int MaxiJackPot
        {
            get; private set;
        }

        public int MegaJackPot
        {
            get; private set;
        }

        public int AutoSpinCount
        {
            get; private set;
        }
        #endregion saved properties

        #region regular
        private IEnumerator Start()
        {
            while (!MPlayer)
            {
                yield return new WaitForEndOfFrame();
            }

            if (spinButton) spinButtonBehavior = spinButton.GetComponent<SpinButtonBehavior>();
            if (spinButtonBehavior)
            {
                spinButtonBehavior.TrySetAutoEvent+=((auto)=>
                {
                    if (Auto) { ResetAutoSpinsMode(); return; }

                    if (auto)
                    {
                        SetAutoSpinsCounter(0);
                        Auto = true;
                        slot.SpinPress();
                        ChangeAutoStateEvent?.Invoke(true);
                    }
                });

                spinButtonBehavior.ClickEvent += () => { Spin_Click(); };
            }
            buttons = GetComponentsInChildren<SceneButton>();

            // set player event handlers
            ChangeFreeSpinsEvent += ChangeFreeSpinsHandler;
            ChangeAutoSpinsEvent += ChangeAutoSpinsHandler;
            ChangeTotalBetEvent += ChangeTotalBetHandler;
            ChangeLineBetEvent += ChangeLineBetHandler;
            ChangeSelectedLinesEvent += ChangeSelectedLinesHandler;

            MPlayer.ChangeWinCoinsEvent += ChangeWinCoinsHandler;

            ChangeMiniJackPotEvent += ChangeMiniJackPotHandler;
            ChangeMaxiJackPotEvent += ChangeMaxiJackPotHandler;
            ChangeMegaJackPotEvent += ChangeMegaJackPotHandler;

            LoadMiniJackPot();
            LoadMaxiJackPot();
            LoadMegaJackPot();

            LoadLineBet();
            if (hold) hold.ChangeBetMultiplierEvent += (hm) => { RefreshBetLines(); };
            if (WinAmountText) winCoinsTween = new TweenIntValue(WinAmountText.gameObject, 0, 0.5f, 2, true, (w) => { if (this && WinAmountText) WinAmountText.text = (w > 0) ? w.ToString(coinsFormat) : "0"; });
            if (InfoText) infoCoinsTween = new TweenIntValue(InfoText.gameObject, 0, 0.5f, 2, true, (w) => { if (this) SetTextString (InfoText, (w > 0) ? w.ToString(coinsFormat) : "0"); });

            AutoSpinsCounter = 0;
            ChangeAutoSpinsCounterEvent += (r, i) => { if (this && AutoSpinsCountText) AutoSpinsCountText.text = i.ToString(); };
            LoadFreeSpins();
            LoadAutoSpins();
            SetTextString(InfoText, (SelectedLinesCount > 0) ? "Click to SPIN to start!" : "Select any slot line!");
            ChangeSelectedLinesEvent += (l, b) => { SetTextString(InfoText, (l > 0) ? "Click to SPIN to start!" : "Select any slot line!"); };
            Refresh();
        }

        void OnDestroy()
        {
            ChangeTotalBetEvent -= ChangeTotalBetHandler;
            ChangeLineBetEvent -= ChangeLineBetHandler;
            ChangeSelectedLinesEvent -= ChangeSelectedLinesHandler;

            // remove player event handlers
            if (MPlayer)
            {
                MPlayer.ChangeWinCoinsEvent -= ChangeWinCoinsHandler;
            }
        }

        private void OnValidate()
        {
            miniStart = Math.Max(0, miniStart);
            maxiStart = Math.Max(0, maxiStart);
            megaStart = Math.Max(0, megaStart);

            maxLineBet = Math.Max(1, maxLineBet);
            defLineBet = Math.Max(1, defLineBet);
            defLineBet = Mathf.Min(defLineBet, maxLineBet);

            maxAutoSpins = Math.Max(1, maxAutoSpins);
            defAutoSpins = Math.Max(1, defAutoSpins);
            defAutoSpins = Math.Min(defAutoSpins, maxAutoSpins);
        }
        #endregion regular

        /// <summary>
        /// Set all buttons interactble = activity, but startButton = startButtonAcivity
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="startButtonAcivity"></param>
        public void SetControlActivity(bool activity, bool startButtonAcivity)
        {
            if (buttons != null)
            {
                foreach (SceneButton b in buttons)
                {
                    if (b) b.interactable = activity;
                }
            }
            if (spinButton) spinButton.interactable = startButtonAcivity;
            if (linesController) { linesController.SetControlActivity(activity); }

        }

        #region refresh
        /// <summary>
        /// Refresh gui data : Balance,  BetCount, freeSpin
        /// </summary>
        private void Refresh()
        {
            RefreshJackPots();
            RefreshBetLines();
            RefreshSpins();
            if ( WinAmountText) WinAmountText.text = 0.ToString();
        }

        /// <summary>
        /// Refresh gui lines, bet
        /// </summary>
        private void RefreshBetLines()
        {
            if (MPlayer)
            {
                if (LineBetSumText) LineBetSumText.text = LineBet.ToString();
                if (TotalBetSumText) TotalBetSumText.text = TotalBet.ToString();
                if (LinesCountText) LinesCountText.text = SelectedLinesCount.ToString();
            }
        }

        /// <summary>
        /// Refresh gui spins
        /// </summary>
        private void RefreshSpins()
        {
            if (AutoSpinsCountText) AutoSpinsCountText.text =  AutoSpinCount.ToString();
            if (FreeSpinText) FreeSpinText.text = (FreeSpins > 0)? "Free" : "";
            if (FreeSpinCountText) FreeSpinCountText.text = (FreeSpins > 0) ? FreeSpins.ToString() : "";
        }

        private void RefreshJackPots()
        {
            if (this && MiniJackpotAmountText) MiniJackpotAmountText.text = MiniJackPot.ToString(coinsFormat);
            if (this && MaxiJackpotAmountText) MaxiJackpotAmountText.text = MaxiJackPot.ToString(coinsFormat);
            if (this && MegaJackpotAmountText) MegaJackpotAmountText.text = MegaJackPot.ToString(coinsFormat);
        }
        #endregion refresh

        #region control buttons
        public void LinesPlus_Click()
        {
           AddSelectedLinesCount(1, true);
        }

        public void LinesMinus_Click()
        {
            AddSelectedLinesCount(-1, false);
        }

        public void LineBetPlus_Click()
        {
           AddLineBet(1);
        }

        public void LineBetMinus_Click()
        {
           AddLineBet(-1);
        }

        public void AutoSpinPlus_Click()
        {
            AddAutoSpins(1);
        }

        public void AutoSpinMinus_Click()
        {
            AddAutoSpins(-1);
        }

        public void MaxBet_Click()
        {
            linesController.SelectAllLines(true);
            SetMaxLineBet();
        }

        public void Spin_Click()
        {
            if (Auto) { ResetAutoSpinsMode(); return; }
            slot.SpinPress();
        }

        public void AutoSpin_Click()
        {
            if (Auto) { ResetAutoSpinsMode(); return; }
            SetAutoSpinsCounter(0);
            Auto = true;
            slot.SpinPress();
            ChangeAutoStateEvent?.Invoke(true);
        }
        #endregion control buttons
		
		#region event handlers
		private void ChangeFreeSpinsHandler(int newFreeSpinsCount)
		{
			if (this)
			{
                if (FreeSpinText) FreeSpinText.text = (FreeSpins > 0) ? "Free" : "";
                if (FreeSpinCountText) FreeSpinCountText.text = (newFreeSpinsCount > 0) ? newFreeSpinsCount.ToString() : "";
			}
		}
		
		private void ChangeAutoSpinsHandler(int newAutoSpinsCount)
		{
			if (this && AutoSpinsCountText) AutoSpinsCountText.text = newAutoSpinsCount.ToString();
		}
		
		private void ChangeTotalBetHandler(int newTotalBet)
		{
			if (this && TotalBetSumText) TotalBetSumText.text = TotalBet.ToString();
		}
		
		private void ChangeLineBetHandler(int newLineBet)
		{
			if (this && LineBetSumText) LineBetSumText.text = newLineBet.ToString();
              
		}
		
		private void ChangeSelectedLinesHandler(int newCount, bool burn)
		{
			if (this && LinesCountText) LinesCountText.text = newCount.ToString();
		}

        private void ChangeMiniJackPotHandler(int newCount)
        {
            if (this && MiniJackpotAmountText) MiniJackpotAmountText.text = newCount.ToString();
        }

        private void ChangeMaxiJackPotHandler(int newCount)
        {
            if (this && MaxiJackpotAmountText) MaxiJackpotAmountText.text = newCount.ToString();
        }

        private void ChangeMegaJackPotHandler(int newCount)
        {
            if (this && MegaJackpotAmountText) MegaJackpotAmountText.text = newCount.ToString(coinsFormat);
        }

        private void ChangeWinCoinsHandler(int newCount)
        {
            if (winCoinsTween != null) winCoinsTween.Tween(newCount, 100);
            if (infoCoinsTween != null) infoCoinsTween.Tween(newCount, 100);
        }
        #endregion event handlers

        #region mini jackpot
        /// <summary>
        /// Add mini jack pot and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddMiniJackPot(int count)
        {
            SetMiniJackPotCount(MiniJackPot + count);
        }

        /// <summary>
        /// Set mini jackpot and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetMiniJackPotCount(int count)
        {
            count = Mathf.Max(miniStart, count);
            bool changed = (MiniJackPot != count);
            MiniJackPot = count;
            if (SaveData && changed)
            {
                string key = SaveMiniJackPotKey;
                PlayerPrefs.SetInt(key, MiniJackPot);
            }
            if (changed) ChangeMiniJackPotEvent?.Invoke(MiniJackPot);
        }

        /// <summary>
        /// Load serialized mini jackpot or set defaults
        /// </summary>
        private void LoadMiniJackPot()
        {
            if (SaveData)
            {
                string key = SaveMiniJackPotKey;
                if (PlayerPrefs.HasKey(key)) SetMiniJackPotCount(PlayerPrefs.GetInt(key));
                else SetMiniJackPotCount(miniStart);
            }
            else
            {
                SetMiniJackPotCount(miniStart);
            }
        }
        #endregion mini jackpot

        #region maxi jackpot
        /// <summary>
        /// Add maxi jack pot and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddMaxiJackPot(int count)
        {
            SetMaxiJackPotCount(MaxiJackPot + count);
        }

        /// <summary>
        /// Set maxi jackpot and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetMaxiJackPotCount(int count)
        {
            count = Mathf.Max(maxiStart, count);
            bool changed = (MaxiJackPot != count);
            MaxiJackPot = count;
            if (SaveData && changed)
            {
                string key = SaveMaxiJackPotKey;
                PlayerPrefs.SetInt(key, MaxiJackPot);
            }
            if (changed) ChangeMaxiJackPotEvent?.Invoke(MaxiJackPot);
        }

        /// <summary>
        /// Load serialized maxi jackpot or set defaults
        /// </summary>
        private void LoadMaxiJackPot()
        {
            if (SaveData)
            {
                string key = SaveMaxiJackPotKey;
                if (PlayerPrefs.HasKey(key)) SetMaxiJackPotCount(PlayerPrefs.GetInt(key));
                else SetMaxiJackPotCount(maxiStart);
            }
            else
            {
                SetMaxiJackPotCount(maxiStart);
            }
        }
        #endregion maxi jackpot

        #region mega jackpot
        /// <summary>
        /// Add mega jack pot and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddMegaJackPot(int count)
        {
            SetMegaJackPotCount(MegaJackPot + count);
        }

        /// <summary>
        /// Set mega jackpot and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetMegaJackPotCount(int count)
        {
            count = Mathf.Max(megaStart, count);
            bool changed = (MegaJackPot != count);
            MegaJackPot = count;
            if (SaveData && changed)
            {
                string key = SaveMegaJackPotKey;
                PlayerPrefs.SetInt(key, MegaJackPot);
            }
            if (changed) ChangeMegaJackPotEvent?.Invoke(MegaJackPot);
        }

        /// <summary>
        /// Load serialized mega jackpot or set defaults
        /// </summary>
        private void LoadMegaJackPot()
        {
            if (SaveData)
            {
                string key = SaveMegaJackPotKey;
                if (PlayerPrefs.HasKey(key)) SetMegaJackPotCount(PlayerPrefs.GetInt(key));
                else SetMegaJackPotCount(megaStart);
            }
            else
            {
                SetMegaJackPotCount(megaStart);
            }
        }
        #endregion mega jackpot

        #region common jackpot
        public void SetJackPotCount(int count, JackPotType jackPotType)
        {
            switch (jackPotType)
            {
                case JackPotType.Mini:
                    SetMiniJackPotCount(count);
                    break;
                case JackPotType.Maxi:
                    SetMaxiJackPotCount(count);
                    break;
                case JackPotType.Mega:
                    SetMegaJackPotCount(count);
                    break;
            }
        }

        public int GetJackPotCoins(JackPotType jackPotType)
        {
            int jackPotCoins = 0;
            switch (jackPotType)
            {
                case JackPotType.Mini:
                    jackPotCoins = MiniJackPot; 
                    break;
                case JackPotType.Maxi:
                    jackPotCoins = MaxiJackPot; 
                    break;
                case JackPotType.Mega:
                    jackPotCoins = MegaJackPot;
                    break;
            }
            return jackPotCoins;
        }

        internal void JPWinCancel()
        {
            if (megaJackPotWinPu) megaJackPotWinPu.CloseWindow();
            if (maxiJackPotWinPu) maxiJackPotWinPu.CloseWindow();
            if (miniJackPotWinPu) miniJackPotWinPu.CloseWindow();

            if (megaJackPotWinGO) Destroy(megaJackPotWinGO);
            if (maxiJackPotWinGO) Destroy(maxiJackPotWinGO);
            if (miniJackPotWinGO) Destroy(miniJackPotWinGO);
        }

        internal void JPWinShow(int jackPotCoins, JackPotType jackPotType)
        {
            switch (jackPotType)
            {
                case JackPotType.None:
                    break;
                case JackPotType.Mini:
                    if (miniJackPotWinPrefab && jackPotController) miniJackPotWinGO = Instantiate(miniJackPotWinPrefab, jackPotController.transform);
                    if (miniJackPotWinPuPrefab) miniJackPotWinPu = MGUI.ShowMessage(miniJackPotWinPuPrefab, "", jackPotCoins.ToString(), 5f, null);
                    break;
                case JackPotType.Maxi:
                    if (maxiJackPotWinPrefab && jackPotController) maxiJackPotWinGO = Instantiate(maxiJackPotWinPrefab, jackPotController.transform);
                    if (maxiJackPotWinPuPrefab) maxiJackPotWinPu = MGUI.ShowMessage(maxiJackPotWinPuPrefab, "", jackPotCoins.ToString(), 5f, null);
                    break;
                case JackPotType.Mega:
                    if (megaJackPotWinPrefab && jackPotController) megaJackPotWinGO = Instantiate(megaJackPotWinPrefab, jackPotController.transform);
                    if (megaJackPotWinPuPrefab) megaJackPotWinPu = MGUI.ShowMessage(megaJackPotWinPuPrefab, "", jackPotCoins.ToString(), 5f, null);
                    break;
                default:
                    break;
            }
        }
        #endregion common jackpot

        #region LineBet
        /// <summary>
        /// Change line bet and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddLineBet(int count)
        {
            SetLineBet(LineBet + count);
        }

        /// <summary>
        /// Set line bet and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetLineBet(int count)
        {
            count = Mathf.Max(1, count);
            count = Mathf.Min(count, maxLineBet);
            bool changed = (LineBet != count);
            LineBet = count;
            if (changed)
            {
                ChangeLineBetEvent?.Invoke(LineBet);
                ChangeTotalBetEvent?.Invoke(TotalBet);
            }
        }

        /// <summary>
        /// Load default line bet
        /// </summary>
        private void LoadLineBet()
        {
            SetLineBet(defLineBet);
        }

        internal void SetMaxLineBet()
        {
            SetLineBet(maxLineBet);
        }

        /// <summary>
        /// If has money for bet, dec money, and return true
        /// </summary>
        /// <returns></returns>
        internal bool ApplyBet()
        {
            if (MPlayer.HasMoneyForBet(TotalBet))
            {
                MPlayer.AddCoins(-TotalBet);
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion LineBet

        #region lines
        internal void AddSelectedLinesCount(int count, bool burn)
        {
            SetSelectedLinesCount(SelectedLinesCount + count, burn);
        }

        internal void SetSelectedLinesCount(int count, bool burn)
        {
            count = Mathf.Max(1, count);
            count = Mathf.Min(linesController.LinesCount, count);

            bool changed = (SelectedLinesCount != count);
            SelectedLinesCount = count;
            if (changed)
            {
                ChangeSelectedLinesEvent?.Invoke(count, burn);
                ChangeTotalBetEvent?.Invoke(TotalBet);
            }
        }
        #endregion lines

        #region FreeSpins
        /// <summary>
        /// Change free spins count and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddFreeSpins(int count)
        {
            SetFreeSpinsCount(FreeSpins + count);
        }

        /// <summary>
        /// Set Free spins count and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetFreeSpinsCount(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (FreeSpins != count);
            FreeSpins = count;
            if (changed) ChangeFreeSpinsEvent?.Invoke(FreeSpins);
        }

        /// <summary>
        /// Load default free spins count
        /// </summary>
        private void LoadFreeSpins()
        {
            SetFreeSpinsCount(0);
        }

        /// <summary>
        /// If has free spins, dec free spin and return true.
        /// </summary>
        /// <returns></returns>
        internal bool ApllyFreeSpin()
        {
            if (HasFreeSpin)
            {
                AddFreeSpins(-1);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion FreeSpins

        #region AutoSpins
        /// <summary>
        /// Change auto spins cout and save result
        /// </summary>
        /// <param name="count"></param>
        public void AddAutoSpins(int count)
        {
            SetAutoSpinsCount(AutoSpinCount + count);
        }

        /// <summary>
        /// Set level and save result
        /// </summary>
        /// <param name="count"></param>
        public void SetAutoSpinsCount(int count)
        {
            count = Mathf.Max(1, count);
            count = Mathf.Min(count, maxAutoSpins);
            bool changed = (AutoSpinCount != count);
            AutoSpinCount = count;
            if (SaveData && changed)
            {
                string key = SaveAutoSpinsKey;
                PlayerPrefs.SetInt(key, AutoSpinCount);
            }
            if (changed) ChangeAutoSpinsEvent?.Invoke(AutoSpinCount);
        }

        /// <summary>
        /// Load serialized auto spins count or set default auto spins count
        /// </summary>
        private void LoadAutoSpins()
        {
            //if (SaveData)
            //{
            //    string key = saveAutoSpinsKey;
            //    if (PlayerPrefs.HasKey(key)) SetAutoSpinsCount(PlayerPrefs.GetInt(key));
            //    else SetAutoSpinsCount(defAutoSpins);
            //}
            //else
            {
                SetAutoSpinsCount(defAutoSpins);
            }
        }

        public void IncAutoSpinsCounter()
        {
            SetAutoSpinsCounter(AutoSpinsCounter+1);
        }

        public void SetAutoSpinsCounter(int count)
        {
            count = Mathf.Max(0, count);
            bool changed = (count != AutoSpinsCounter);
            AutoSpinsCounter = count;
            if(changed) ChangeAutoSpinsCounterEvent?.Invoke(AutoSpinsCounter, AutoSpinCount);
        }

        public void ResetAutoSpinsMode()
        {
            Auto = false;
            if (autoSpinButton) autoSpinButton.Release();
            ChangeAutoStateEvent?.Invoke(false);
        }
        #endregion AutoSpins

        public void SetDefaultData()
        {
            SetMiniJackPotCount(miniStart);
            SetMaxiJackPotCount(maxiStart);
            SetMegaJackPotCount(megaStart);
            SetLineBet(defLineBet);
            SetAutoSpinsCount(defAutoSpins);
        }

        #region utils
        public void SetTextString(Text text, string textString)
        {
            if (text)
            {
                text.text = textString;
            }
        }

        public void SetImageSprite(Image image, Sprite sprite)
        {
            if (image)
            {
                image.sprite = sprite;
            }
        }

        private string GetMoneyName(int count)
        {
            if (count > 1) return "coins";
            else return "coin";
        }
        #endregion utils
    }
}