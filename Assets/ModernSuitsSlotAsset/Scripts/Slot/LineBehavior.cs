using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.Events;

namespace Mkey
{
    public class LineBehavior : MonoBehaviour
    {
        public int number;
        public RayCaster[] rayCasters;

        public WinData win;

        [Tooltip("Line color in line info panel")]
        public Color lineInfoColor = Color.white;
        [Tooltip("BackGround color in line info panel")]
        public Color lineInfoBGColor = Color.blue;

        public Action<bool> ChangeSelectionEvent;

        #region temp vars
        private LinesController linesController;
        private TextMesh winText;
        private SlotLineRenderer sLR;
        private SlotController slot;
        #endregion temp vars

        #region properties
        public LineButtonBehavior LineButton { get; private set;}

        public bool IsSelected { get;  private set; }

        public bool IsWinningLine
        {
            get { return win!=null; }
        }

        /// <summary>
        /// Get spins won
        /// </summary>
        public int WonSpins
        {
            get
            {
                return (win == null) ? 0 : win.FreeSpins;
            }
        }

        /// <summary>
        /// Get coins won
        /// </summary>
        internal int WonCoins
        {
            get
            {
                return (win == null) ? 0 : win.Pay;
            }
        }

        /// <summary>
        /// Return true if is won tween complete
        /// </summary>
        internal bool IsWinTweenComplete
        {
            get;private set;
        }
        #endregion properties

        #region regular
        /// <summary>
        /// Start from linesController
        /// </summary>
        /// <param name="linesController"></param>
        internal void InitStart(SlotController slot, LinesController linesController, SlotLineRenderer lineRendererPrefab, bool useLinesControllerMaterial)
        {
            this.linesController = linesController;
            this.slot = slot;

            sLR = Instantiate(lineRendererPrefab);
            sLR.transform.parent = transform;
            sLR.transform.localScale = Vector3.one;
            sLR.transform.localPosition = Vector3.zero;
            sLR.Create(linesController, this);


            win = null;
          
            LineButton = GetComponentInChildren<LineButtonBehavior>();
            if (LineButton) // set event handlers
            {
                LineButton.PointerDownEvent += ButtonClickHandler;
                ChangeSelectionEvent += LineButton.Refresh;
            }
        }

        void OnDrawGizmosSelected()
        {

        }

        void OnDrawGizmos()
        {

        }

        private void OnDestroy()
        {
            if (LineButton)
            {
                LineButton.PointerDownEvent -= ButtonClickHandler;
                ChangeSelectionEvent -= LineButton.Refresh;
            }
        }
        #endregion regular

        void ButtonClickHandler(TouchPadEventArgs tpea)
        {
            linesController.LineButton_Click(this);
        }

        /// <summary>
        /// Select line
        /// </summary>
        public void Select(bool burn, float burnDelay)
        {
            IsSelected = true;
            LineBurn(burn, burnDelay, null);
            ChangeSelectionEvent?.Invoke(IsSelected);
        }

        /// <summary>
        /// Deselect line
        /// </summary>
        public void DeSelect()
        {
            IsSelected = false;
            LineBurn(false, 0, null);
            ChangeSelectionEvent?.Invoke(IsSelected);
        }

        #region linerender
        /// <summary>
        /// Enable or disable the flashing material
        /// </summary>
        internal void LineFlashing(bool flashing)
        {
            sLR.LineFlashing(flashing);

            if(linesController && flashing && IsWinningLine && linesController.winTextPrefab && linesController.showWinText && WonCoins > 0)
            {
                if (!winText)
                {
                    bool isEven = (rayCasters.Length % 2 == 0);
                    int rc = rayCasters.Length / 2;
                    Vector3 position = (!isEven) ? rayCasters[rc].transform.position : (rayCasters[rc].transform.position + rayCasters[rc-1].transform.position) / 2f;
                    winText = Instantiate(linesController.winTextPrefab);
                    winText.transform.position = position;
                    winText.transform.parent = transform;
                    winText.transform.localScale = Vector3.one;
                }
                winText.gameObject.SetActive(true);
                winText.text = WonCoins.ToString();
            }
            else
            {
                if (winText) winText.gameObject.SetActive(false);
            }
        }

        internal void LineBurn(bool burn, float burnDelay, Action completeCallBack)
        {
            sLR.LineBurn(burn, burnDelay, completeCallBack);
        }

        /// <summary>
        /// Enable or disable line elemnts.
        /// </summary>
        internal void SetLineVisible(bool visible)
        {
            sLR.SetLineVisible(visible);
        }
        #endregion linerender

        /// <summary>
        /// Find  and fill winning symbols list  from left to right, according pay lines
        /// </summary>
        internal void FindWin(List<PayLine> payTable)
        {
            win = null;
            WinData winTemp = null;
            foreach (var item in payTable)
            {
                // find max win
                winTemp = GetPayLineWin(item);
                if (winTemp != null)
                {
                    if(win==null)
                    {
                        win = winTemp;
                    }
                    else
                    {
                        if(win.Pay < winTemp.Pay || win.FreeSpins < winTemp.FreeSpins)
                        {
                            win = winTemp;
                        }
                    }
                        
                }
            }
        }

        /// <summary>
        /// Check if line is wonn, according payline
        /// </summary>
        /// <param name="payLine"></param>
        /// <returns></returns>
        private WinData GetPayLineWin(PayLine payLine)
        {
            if (payLine == null || payLine.line.Length < rayCasters.Length) return null;
            List<SlotSymbol> winnSymbols = new List<SlotSymbol>();
            SlotSymbol s;
            for (int i = 0; i < rayCasters.Length; i++)
            {
                s = rayCasters[i].GetSymbol();
                //Debug.Log(s.iconID);
                if (payLine.line[i] >= 0 && s.IconID != payLine.line[i])
                {
                    return null;
                }
                else if (payLine.line[i] >= 0 && s.IconID == payLine.line[i])
                {
                    winnSymbols.Add(s);
                }
            }
            return new WinData(winnSymbols, payLine.freeSpins, payLine.pay, payLine.payMult, payLine.freeSpinsMult, payLine.LineEvent);
        }

        /// <summary>
        /// Reset old winnig data 
        /// </summary>
        internal void ResetLineWinning()
        {
            win = null;
        }

        #region win animation
        /// <summary>
        /// Instantiate particles for each winning symbol
        /// </summary>
        internal void ShowWinSymbolsParticles(bool activate)
        {
            if (IsWinningLine)
            {
                win.Symbols.ForEach((wS) => { wS.ShowParticles(activate, slot.particlesStars); });
            }
        }

        /// <summary>
        /// Instantiate jump clone for each symbol
        /// </summary>
        internal void LineWinPlay(string tag, float playTime, Action<WinData> comleteCallBack)
        {
            IsWinTweenComplete = false;
            if (win == null || win.Symbols == null)
            {
                comleteCallBack?.Invoke(null);
                return;
            }

            Action <float, Action> waitAction = (time, callBack) => { SimpleTween.Value(gameObject, 0,1, time).AddCompleteCallBack(callBack); };

            ParallelTween pt = new ParallelTween();
            foreach (SlotSymbol s in win.Symbols)
            {
                pt.Add((callBack) =>
                {
                    s.ShowWinPrefab(tag);
                    waitAction(playTime, callBack);
                });
            }
            pt.Start(() =>
            {
                IsWinTweenComplete = true;
                LineWinCancel();
                comleteCallBack?.Invoke(win);
            });
        }

        internal void LineWinCancel()
        {
            if (win != null && win.Symbols != null)
                win.Symbols.ForEach((ws) => { if (ws != null) ws.DestroyWinObject(); });
        }
        #endregion win animation

        #region calc
        public bool IsWinningLineCalc
        {
            get { return winCalc != null; }
        }

        public WinDataCalc winCalc;
        /// <summary>
        /// Find  and fill winning symbols list  from left to right, according pay lines
        /// </summary>
        internal void FindWinCalc(List<PayLine> payTable)
        {
            winCalc = null;
            WinDataCalc winTemp = null;
            foreach (var item in payTable)
            {
                // find max win
                winTemp = GetPayLineWinCalc(item);
                if (winTemp != null)
                {
                    if (winCalc == null)
                    {
                        winCalc = winTemp;
                    }
                    else
                    {
                        if (winCalc.Pay < winTemp.Pay || winCalc.FreeSpins < winTemp.FreeSpins)
                        {
                            winCalc = winTemp;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if line is wonn, according payline
        /// </summary>
        /// <param name="payLine"></param>
        /// <returns></returns>
        private WinDataCalc GetPayLineWinCalc(PayLine payLine)
        {
            if (payLine == null || payLine.line.Length < rayCasters.Length) return null;
            int winnSymbols = 0;

            for (int i = 0; i < rayCasters.Length; i++)
            {
                int s = rayCasters[i].ID;
                int ps = payLine.line[i];
                //Debug.Log(s.iconID);
                if (payLine.line[i] >= 0)
                {
                    if (s != ps)
                        return null;
                    else
                        winnSymbols++;
                }
            }
            return new WinDataCalc(winnSymbols, payLine.freeSpins, payLine.pay, payLine.payMult);
        }
        #endregion calc
    }

    public class WinData
    {
        List<SlotSymbol> symbols;
        private int freeSpins = 0;
        private int pay = 0;
        private int payMult = 1;
        private int freeSpinsMult = 1;
        private UnityEvent winEvent;

        public int Pay
        {
            get { return pay; }
        }

        public int FreeSpins
        {
            get { return freeSpins; }
        }

        public int PayMult
        {
            get { return payMult; }
        }

        public int FreeSpinsMult
        {
            get { return freeSpinsMult; }
        }

        public UnityEvent WinEvent
        {
            get { return winEvent; }
        }

        public List<SlotSymbol> Symbols
        {
            get { return symbols; }
        }

        public WinData(List<SlotSymbol> symbols, int freeSpins, int pay, int payMult, int freeSpinsMult, UnityEvent lineEvent)
        {
            this.symbols = symbols;
            this.freeSpins = freeSpins;
            this.pay = pay;
            this.payMult = payMult;
            this.freeSpinsMult = freeSpinsMult;
            this.winEvent = lineEvent;
        }

        public override string ToString()
        {
            return "Pay: " + pay + " ; FreeSpin: " + freeSpins + " ; PayMult: " + payMult+ " ; FreeSpinsMult: " + freeSpinsMult;
        }

    }

    public class WinDataCalc
    {
        int symbols;
        private int freeSpins = 0;
        private int pay = 0;
        private int payMult = 1;

        public int Pay
        {
            get { return pay; }
        }

        public int FreeSpins
        {
            get { return freeSpins; }
        }

        public int PayMult
        {
            get { return payMult; }
        }

        public int Symbols { get { return symbols; } }

        public WinDataCalc(int symbols, int freeSpins, int pay, int payMult)
        {
            this.symbols = symbols;
            this.freeSpins = freeSpins;
            this.pay = pay;
            this.payMult = payMult;
        }

        public override string ToString()
        {
            return "Pay: " + pay + " ; FreeSpin: " + freeSpins + " ; PayMult: " + payMult;
        }

    }
}
