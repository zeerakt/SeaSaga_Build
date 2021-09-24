using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class WinController : MonoBehaviour
    {
        [SerializeField]
        private LinesController linesController;
        [Tooltip("Win prefab tag")]
        [SerializeField]
        private string winTag = "spritescale";
        [Tooltip("Time in sec for each winning line to show winsymbols")]
        [SerializeField]
        private float lineWinShowTime = 5f;

        public WinData scatterWin { get; private set; }

        #region temp vars
        private List<PayLine> payTable;
        private List<ScatterPay> scatterPayTable;
        private List<SlotSymbol> scatterWinSymbols;
        private List<SlotSymbol> jackPotWinSymbols;
        private SlotGroupBehavior[] slotGroupsBeh;
        private int scatter_id;
        private bool useScatter;
        private GameObject particlesPrefab;
        private Transform topJumpTarget;
        private Transform bottomJumpTarget;
        private int megaJackPotCount;
        private int maxiJackPotCount;
        private int miniJackPotCount;
        private bool useMegaJackPot;
        private bool useMaxiJackPot;
        private bool useMiniJackPot;
        private int jp_symb_id;
        private bool winLineFlashing = false;
        private SlotController slot;
        private int contID;
        private TweenSeq contTS;
        private TweenSeq onceTS;
        #endregion temp vars

        #region regular 
        void Start()
        {
            InitCalc();
        }

        private void OnDestroy()
        {
            WinShowCancel();
        }

        private void OnValidate()
        {
            lineWinShowTime = (lineWinShowTime < 3) ? 3 : lineWinShowTime;
        }

        internal void InitCalc()
        {
            slot = GetComponent<SlotController>();
            payTable = slot.payTableFull;
            slotGroupsBeh = slot.slotGroupsBeh;
            scatter_id = slot.scatter_id;
            useScatter = slot.useScatter;
            particlesPrefab = slot.particlesStars;
            topJumpTarget = slot.topJumpTarget;
            bottomJumpTarget = slot.bottomJumpTarget;
            megaJackPotCount = slot.megaJackPotCount;
            maxiJackPotCount = slot.maxiJackPotCount;
            miniJackPotCount = slot.miniJackPotCount;
            scatterPayTable = slot.scatterPayTable;
            useMegaJackPot = slot.useMegaJacPot;
            useMaxiJackPot = slot.useMaxiJacPot;
            useMiniJackPot = slot.useMiniJacPot;
            jp_symb_id = slot.jp_symbol_id;
        }
        #endregion regular 

        #region win animation
        /// <summary>
        /// Show symbols particles and lines glowing
        /// </summary>
        internal void WinEffectsShow(bool flashingLines, bool showSymbolParticles)
        {
            HideAllLines();

            foreach (var lb in linesController.Lines)
            {
                if (lb.IsWinningLine)
                {
                    lb.SetLineVisible(flashingLines);
                    lb.LineFlashing(flashingLines);
                }
                lb.ShowWinSymbolsParticles(showSymbolParticles);
            }

            if (useScatter && scatterWinSymbols != null && scatterWinSymbols.Count > 0)
            {
                foreach (var item in scatterWinSymbols)
                {
                    item.ShowParticles(showSymbolParticles, slot.particlesStars);
                }
            }

            if (jackPotWinSymbols != null && jackPotWinSymbols.Count > 0)
            {
                foreach (var item in jackPotWinSymbols)
                {
                    item.ShowParticles(showSymbolParticles, slot.particlesStars);
                }
            }
        }

        /// <summary>
        /// Show win symbols 
        /// </summary>
        internal void WinSymbolShow(bool flashLine, Action<WinData> lineWinCallBack, Action scatterWinCallBack, Action jackPotWinCallBack, Action completeCallBack)
        {
            winLineFlashing = flashLine;
            WinSymbolShowContinuous(lineWinCallBack, scatterWinCallBack, jackPotWinCallBack, completeCallBack);
        }

        /// <summary>
        /// Show selected lines with flashing or without
        /// </summary>
        internal void ShowSelectedLines(bool flashing)
        {
            foreach (var lB in linesController.Lines)
            {
                if (lB.IsSelected)
                {
                    lB.SetLineVisible(true);
                }
                lB.LineFlashing(flashing);
            }
        }

        /// <summary>
        /// Hide selected lines
        /// </summary>
        internal void HideAllLines()
        {
            foreach (var lb in linesController.Lines)
            {
                lb.LineFlashing(false);
                lb.LineBurn(false, 0, null);
            }
        }

        /// <summary>
        /// Reset winning line data
        /// </summary>
        internal void ResetLineWinning()
        {
            foreach (LineBehavior lb in linesController.Lines)
            {
                lb.ResetLineWinning();
            }

            scatterWinSymbols = null;
            jackPotWinSymbols = null;
            scatterWin = null;
        }

        internal void WinShowCancel()
        {
            if (onceTS!=null) onceTS.Break();
            if (contTS != null) contTS.Break();
            SimpleTween.Cancel(contID, false);

            if (linesController != null && linesController.Lines != null)
            {
                foreach (LineBehavior lb in linesController.Lines)
                {
                    lb.LineWinCancel();
                }
            }
            if (useScatter && scatterWinSymbols != null)
                foreach (var item in scatterWinSymbols)
                {
                    item.DestroyWinObject();
                }
            if (jackPotWinSymbols != null)
                foreach (var item in jackPotWinSymbols)
                {
                    item.DestroyWinObject();
                }
        }

        /// <summary>
        /// Show won symbols once
        /// </summary>
        private void WinSymbolShowOnce(Action<WinData> lineWinCallBack, Action scatterWinCallBack, Action jackPotWinCallBack, Action completeCallBack)
        {
            if (onceTS != null) onceTS.Break();

            onceTS = new TweenSeq();
            Action<GameObject, float, Action> waitAction = (g, time, callBack) => { SimpleTween.Value(g, 0, 1, time).AddCompleteCallBack(callBack); };

            lineWinShowTime = (lineWinShowTime < 3) ? 3 : lineWinShowTime;

            //show linewins
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLine)

                    onceTS.Add((callBack) =>
                    {
                        if (winLineFlashing)
                        {
                            lB.LineFlashing(true);
                            lB.SetLineVisible(true);
                        }

                        lB.LineWinPlay(winTag, lineWinShowTime,
                                (windata) =>
                                {
                                    if (winLineFlashing)
                                    {
                                        lB.LineFlashing(false);
                                        lB.SetLineVisible(false);
                                    }
                                    lineWinCallBack?.Invoke(windata);
                                    callBack();
                                });
                    });
            }

            //show jackPot win
            if (jackPotWinSymbols != null && jackPotWinSymbols.Count > 0)
            {
                ParallelTween pT = new ParallelTween();
                foreach (var item in jackPotWinSymbols)
                {
                    pT.Add((callBack) =>
                    {
                        item.ShowWinPrefab(winTag);
                        waitAction(item.gameObject, lineWinShowTime, callBack);
                    });
                }
                onceTS.Add((callBack) =>
                {
                    pT.Start(() =>
                    {
                        jackPotWinCallBack?.Invoke();
                        callBack();
                    });
                });
            }

            //show scatterwin
            if (useScatter && scatterWinSymbols != null && scatterWinSymbols.Count > 0)
            {
                ParallelTween pT = new ParallelTween();
                foreach (var item in scatterWinSymbols)
                {
                    pT.Add((callBack) =>
                    {
                        item.ShowWinPrefab(winTag);
                        waitAction(item.gameObject, lineWinShowTime, callBack);
                    });
                }
                onceTS.Add((callBack) =>
                {
                    pT.Start(() =>
                    {
                        scatterWinCallBack?.Invoke();
                        callBack();
                    });
                });
            }

            onceTS.Add((callBack) =>
            {
                completeCallBack?.Invoke();
                callBack();
            });

            onceTS.Start();
        }

        /// <summary>
        /// Show won symbols continuous
        /// </summary>
        private void WinSymbolShowContinuous(Action<WinData> lineWinCallBack, Action scatterWinCallBack, Action jackPotWinCallBack, Action completeCallBack)
        {
            contTS = new TweenSeq();
            //    int length = linesController.LinesCount;
            //    contTS.Add((callBack) =>
            //    {
            //        WinSymbolShowOnce(lineWinCallBack, scatterWinCallBack, jackPotWinCallBack, () =>
            //        {
            //            completeCallBack?.Invoke();
            //            callBack?.Invoke();
            //        });
            //    });

            //    contTS.Add((callBack) =>
            //    {
            //        SimpleTween.SimpleTweenObject cont = SimpleTween.Value(slot.gameObject, 0, 1, 10f).SetCycled().AddCompleteCallBack( // use as timer
            //            () =>
            //            {
            //                //foreach (LineBehavior lb in linesController.Lines)
            //                //{
            //                //    lb.LineWinCancel();
            //                //    if (winLineFlashing)
            //                //    {
            //                //        lb.LineFlashing(false);
            //                //        lb.SetLineVisible(false);
            //                //    }
            //                //}
            //                WinShowCancel();
            //                WinSymbolShowOnce(null, null, null, null);
            //            });
            //        contID = cont.ID;
            //    });
            //    contTS.Start();

            contTS.Add((callBack) =>
            {
                foreach (LineBehavior lb in linesController.Lines)
                {
                    lb.LineWinCancel();
                }
                if (useScatter && scatterWinSymbols != null)
                    foreach (var item in scatterWinSymbols)
                    {
                        item.DestroyWinObject();
                    }
                if (jackPotWinSymbols != null)
                    foreach (var item in jackPotWinSymbols)
                    {
                        item.DestroyWinObject();
                    }

                WinSymbolShowOnce(null, null, null, callBack);
            });

            WinSymbolShowOnce(lineWinCallBack, scatterWinCallBack, jackPotWinCallBack, () =>
                  {
                      completeCallBack?.Invoke();
                      contTS.StartCycle();
                  });
        }
        #endregion win animation

        #region get win
        /// <summary>
        /// Return true if slot has any winning
        /// </summary>
        internal bool HasAnyWinn(ref bool hasLineWin, ref bool hasScatterWin, ref JackPotType jackPotType)
        {
            hasLineWin = false;
            hasScatterWin = false;

            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLine)
                {
                    hasLineWin = true;
                    break;
                }
            }
            if (useScatter && HasScatterWin())
            {
                hasScatterWin = true;
            }

            jackPotType = GetJackPotWin();
            return (hasLineWin || hasScatterWin || jackPotType != JackPotType.None);
        }

        /// <summary>
        /// Search win symbols (paylines, scatter)
        /// </summary>
        internal void SearchWinSymbols()
        {
            foreach (var lb in linesController.Lines)
            {
                if (lb.IsSelected)
                {
                    lb.FindWin(payTable);
                }
            }

            // search scatters
            scatterWinSymbols = new List<SlotSymbol>();
            List<SlotSymbol> scatterSymbolsTemp = new List<SlotSymbol>();
            scatterWin = null;
            foreach (var item in slotGroupsBeh)
            {
                if (!item.HasSymbolInAnyRayCaster(scatter_id, ref scatterSymbolsTemp))
                {

                }
                else
                {
                    scatterWinSymbols.AddRange(scatterSymbolsTemp);
                }
            }

            if (useScatter)
                foreach (var item in scatterPayTable)
                {
                    if (item.scattersCount > 0 && item.scattersCount == scatterWinSymbols.Count)
                    {
                        scatterWin = new WinData(scatterWinSymbols, item.freeSpins, item.pay, item.payMult, item.freeSpinsMult, item.WinEvent);
                    }
                }
            if (scatterWin == null) scatterWinSymbols = new List<SlotSymbol>();
        }

        private bool HasScatterWin()
        {
            return scatterWin != null;
        }

        private JackPotType GetJackPotWin()
        {
            if (!useMiniJackPot && !useMaxiJackPot && !useMegaJackPot) return JackPotType.None;

            jackPotWinSymbols = null;
            Dictionary<int, List<SlotSymbol>> sD = new Dictionary<int, List<SlotSymbol>>();

            // create symbols dictionary
            foreach (var item in slotGroupsBeh)
            {
                RayCaster[] rCs = item.RayCasters;
                foreach (var rc in rCs)
                {
                    SlotSymbol s = rc.GetSymbol();
                    int sID = s.IconID;
                    if (sD.ContainsKey(sID))
                    {
                        sD[sID].Add(s);
                    }
                    else
                    {
                        sD.Add(sID, new List<SlotSymbol> { s });
                    }
                }
            }

            // search jackPot id if symbol is any
            if (jp_symb_id == -1)
            {
                int sCount = 0;
                int id = -1;
                foreach (var item in sD)
                {
                    if (item.Value.Count > sCount)
                    {
                        sCount = item.Value.Count;
                        id = item.Key;
                    }
                }

                if (sD.ContainsKey(id))
                {
                    if (useMegaJackPot && sCount >= megaJackPotCount && megaJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Mega;
                    }
                    if (useMaxiJackPot && sCount >= maxiJackPotCount && maxiJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Maxi;
                    }
                    if (useMiniJackPot && sCount >= miniJackPotCount && miniJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Mini;
                    }
                }
            }
            else
            {
                int id = jp_symb_id;
                if (sD.ContainsKey(id))
                {
                    if (useMegaJackPot && sD[id].Count >= megaJackPotCount && megaJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Mega;
                    }
                    if (useMaxiJackPot && sD[id].Count >= maxiJackPotCount && maxiJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Maxi;
                    }
                    if (useMiniJackPot && sD[id].Count >= miniJackPotCount && miniJackPotCount > 0)
                    {
                        jackPotWinSymbols = sD[id];
                        return JackPotType.Mini;
                    }
                }
            }
            return JackPotType.None;
        }

        /// <summary>
        /// Return line win coins + sctater win coins, without jackpot
        /// </summary>
        /// <returns></returns>
        public int GetWinCoins()
        {
            int res = 0;
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLine)
                {
                    res += lB.win.Pay;
                }
            }
            if (scatterWin != null) res += scatterWin.Pay;
            return res;
        }

        /// <summary>
        /// Return line win spins + sctater win spins
        /// </summary>
        /// <returns></returns>
        public int GetWinSpins()
        {
            int res = 0;
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLine)
                {
                    res += lB.win.FreeSpins;
                }
            }

            if (scatterWin != null) res += scatterWin.FreeSpins;
            return res;
        }

        /// <summary>
        /// Return product of lines payMultiplier, sctater payMultiplier
        /// </summary>
        /// <returns></returns>
        public int GetPayMultiplier()
        {
            int res = 1;
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLine && lB.win.PayMult > 0)
                {
                    res *= lB.win.PayMult;
                }
            }

            if (scatterWin != null && scatterWin.PayMult > 0) res *= scatterWin.PayMult;
            return res;
        }

        /// <summary>
        /// Return product of lines free spins multipliers, scatter free spins multiplier
        /// </summary>
        /// <returns></returns>
        public int GetFreeSpinsMultiplier()
        {
            int res = 1;
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLine && lB.win.FreeSpinsMult != 0)
                {
                    res *= lB.win.FreeSpinsMult;
                }
            }

            if (scatterWin != null && scatterWin.FreeSpinsMult > 0) res *= scatterWin.FreeSpinsMult;
            return res;
        }

        public int GetWinLinesCount()
        {
            int res = 0;
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLine)
                {
                    res++;
                }
            }
            return res;
        }

        public void StartLineEvents()
        {
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLine)
                {
                    lB.win?.WinEvent?.Invoke();
                }
            }
        }
        #endregion get win

        #region calc
        public WinDataCalc scatterWinCalc;

        /// <summary>
        /// calc line wins and scatter wins
        /// </summary>
        /// 
        internal void SearchWinCalc()
        {
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.gameObject.activeSelf)
                {
                    lB.FindWinCalc(payTable);
                }
            }

            // search scatters
            int scatterWinS = 0;
            int scatterSymbolsTemp = 0;
            scatterWinCalc = null;
            foreach (var item in slotGroupsBeh)
            {
                scatterSymbolsTemp = 0;
                for (int i = 0; i < item.RayCasters.Length; i++)
                {
                    if (item.RayCasters[i].ID == scatter_id)
                    {
                        scatterSymbolsTemp++;
                    }
                }
                scatterWinS += scatterSymbolsTemp;
            }

            if (useScatter)
                foreach (var item in scatterPayTable)
                {
                    if (item.scattersCount > 0 && item.scattersCount == scatterWinS)
                    {
                        scatterWinCalc = new WinDataCalc(scatterWinS, item.freeSpins, item.pay, item.payMult);
                        //Debug.Log("scatters: " + item.scattersCount);
                    }
                }
        }

        /// <summary>
        /// Return line win coins + sctater win coins, without jackpot
        /// </summary>
        /// <returns></returns>
        public int GetLineWinCoinsCalc()
        {
            int res = 0;
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLineCalc)
                {
                    res += lB.winCalc.Pay;
                }
            }
            return res;
        }

        /// <summary>
        /// Return line win coins + sctater win coins, without jackpot
        /// </summary>
        /// <returns></returns>
        public int GetScatterWinCoinsCalc()
        {
            int res = 0;
            if (scatterWinCalc != null) res += scatterWinCalc.Pay;
            return res;
        }

        /// <summary>
        /// Return line win spins + sctater win spins
        /// </summary>
        /// <returns></returns>
        public int GetLineWinSpinsCalc()
        {
            int res = 0;
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLineCalc)
                {
                    res += lB.winCalc.FreeSpins;
                }
            }
            return res;
        }

        /// <summary>
        /// Return line win spins + sctater win spins
        /// </summary>
        /// <returns></returns>
        public int GetScatterWinSpinsCalc()
        {
            int res = 0;
            if (scatterWinCalc != null) res += scatterWinCalc.FreeSpins;
            return res;
        }

        /// <summary>
        /// Return product of lines payMultiplier, sctater payMultiplier
        /// </summary>
        /// <returns></returns>
        public int GetLinePayMultiplierCalc()
        {
            int res = 1;
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLineCalc && lB.winCalc.PayMult != 0)
                {
                    res *= lB.winCalc.PayMult;
                }
            }
            return res;
        }

        /// <summary>
        /// Return product of lines payMultiplier, sctater payMultiplier
        /// </summary>
        /// <returns></returns>
        public int GetScatterPayMultiplierCalc()
        {
            int res = 1;
            if (scatterWinCalc != null && scatterWinCalc.PayMult != 0) res *= scatterWinCalc.PayMult;
            return res;
        }

        public int GetWinLinesCountCalc()
        {
            int res = 0;
            foreach (LineBehavior lB in linesController.Lines)
            {
                if (lB.IsWinningLineCalc)
                {
                    res++;
                }
            }
            return res;
        }
        #endregion calc
    }
}
