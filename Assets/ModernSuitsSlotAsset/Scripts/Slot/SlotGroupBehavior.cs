using UnityEngine;
using System;
using System.Collections.Generic;

namespace Mkey
{
    public class SlotGroupBehavior : MonoBehaviour

    {
        private SlotSoundController RSound { get { return SlotSoundController.Instance; } }
        public List<int> symbOrder;
        public List<Triple> triples;
        [SerializeField]
        [Tooltip("Symbol windows, from top to bottom")]
        private RayCaster[] rayCasters;
  

        [Space(16, order = 0)]
        [SerializeField]
        [Tooltip("sec, additional rotation time")]
        private float addRotateTime = 0f;
        [SerializeField]
        [Tooltip("sec, delay time for spin")]
        private float spinStartDelay = 0f;
        [Tooltip("min 0% - max 20%, change spinStartDelay")]
        [SerializeField]
        private int spinStartRandomize = 0;
        [SerializeField]
        private int spinSpeedMultiplier = 1;

        [Space(16, order = 0)]
        [SerializeField]
        [Tooltip("If true - reel set to random position at start")]
        private bool randomStartPosition = false;
        [Space(16, order = 0)]
        [SerializeField]
        [Tooltip("Tile size by Y")]
        private float tileSizeY = 3.13f;
        [SerializeField]
        [Tooltip("Additional space between tiles")]
        private float gapY = 0.35f; // additional space 
        [SerializeField]
        [Tooltip("Link to base (bottom raycaster)")]
        private bool baseLink = false;

        #region simulate
        [SerializeField]
        private bool simulate =false;
        [SerializeField]
        public  int simPos = 0;
        #endregion simulate

        [Tooltip("ReelSymbols source")]
        public SlotGroupBehavior CopyFrom;

        #region temp vars
        private float anglePerTileRad = 0;
        private float anglePerTileDeg = 0;
        private TweenSeq tS;
        private Transform TilesGroup;
        private SlotSymbol[] slotSymbols;
        private SlotIcon[] sprites;

        private int lastChanged = -1;
        [SerializeField]
        private bool debugreel=false;
        private int tileCount;
        private int windowSize;
        private int maxWindowSize=5;
        [SerializeField]
        private int topSector = 0;
        private int tempSectors = 0;
        #endregion temp vars

        #region properties 
        public int NextOrderPosition { get; private set; }
        public int CurrOrderPosition { get; private set; }
        public RayCaster[] RayCasters { get { return rayCasters; } }
        #endregion properties 

        #region regular
        private void OnValidate()
        {
            spinStartRandomize = (int)Mathf.Clamp(spinStartRandomize, 0, 20);
            spinStartDelay = Mathf.Max(0,spinStartDelay);
            spinSpeedMultiplier = Mathf.Max(0, spinSpeedMultiplier);
            addRotateTime = Mathf.Max(0, addRotateTime);
        }

        private void OnDestroy()
        {
            CancelRotation();
        }

        private void OnDisable()
        {
            CancelRotation();
        }
        #endregion regular

        public float[] SymbProbabilities
        {
            get; private set;
        }

        /// <summary>
        /// Instantiate slot tiles 
        /// </summary>
        internal void CreateSlotCylinder(SlotIcon[] sprites, int tileCount, GameObject tilePrefab)
        {
            CurrOrderPosition = 0;
            this.sprites = sprites;
            this.tileCount = tileCount;
            slotSymbols = new SlotSymbol[tileCount];

            // create Reel transform
            TilesGroup = (new GameObject()).transform;
            TilesGroup.localScale = transform.lossyScale;
            TilesGroup.parent = transform;
            TilesGroup.localPosition = Vector3.zero;
            TilesGroup.name = "Reel(" + name + ")"; 

            // calculate reel geometry
            float distTileY = tileSizeY + gapY; //old float distTileY = 3.48f;

            anglePerTileDeg = 360.0f / (float)tileCount;
            anglePerTileRad = anglePerTileDeg * Mathf.Deg2Rad;
            float radius = (distTileY / 2f) / Mathf.Tan(anglePerTileRad / 2.0f); //old float radius = ((tileCount + 1) * distTileY) / (2.0f * Mathf.PI);

            windowSize = rayCasters.Length;

            bool isEvenRayCastersCount = (windowSize % 2 == 0);
            int dCount = (isEvenRayCastersCount) ? windowSize / 2 - 1 : windowSize / 2;
            float addAnglePerTileDeg = (isEvenRayCastersCount) ? -anglePerTileDeg*dCount - anglePerTileDeg /2f : -anglePerTileDeg;
            float addAnglePerTileRad = (isEvenRayCastersCount) ? -anglePerTileRad*dCount - anglePerTileRad /2f : -anglePerTileRad;
            topSector = windowSize - 1;

            TilesGroup.localPosition = new Vector3(TilesGroup.localPosition.x, TilesGroup.localPosition.y, radius); // offset reel position by z-coordinat

            // orient to base rc
            RayCaster baseRC = rayCasters[rayCasters.Length - 1]; // bottom raycaster
            float brcY = baseRC.transform.localPosition.y;
            float dArad = 0f;
            if (brcY >-radius && brcY < radius && baseLink)
            {
                float dY = brcY - TilesGroup.localPosition.y;
                dArad = Mathf.Asin(dY/radius);
            //    Debug.Log("dY: "+ dY + " ;dArad: " + dArad  + " ;deg: " + dArad* Mathf.Rad2Deg);
                addAnglePerTileRad = dArad;
                addAnglePerTileDeg = dArad * Mathf.Rad2Deg;
            }
            else if(baseLink)
            {
                Debug.Log("Base Rc position out of reel radius" );
            }

            //create reel tiles
            for (int i = 0; i < tileCount; i++)
            {
                float n = (float)i;
                float tileAngleRad = n * anglePerTileRad + addAnglePerTileRad; // '- anglePerTileRad' -  symborder corresponds to visible symbols on reel before first spin 
                float tileAngleDeg = n * anglePerTileDeg + addAnglePerTileDeg;

                slotSymbols[i] = Instantiate(tilePrefab, transform.position, Quaternion.identity).GetComponent<SlotSymbol>();
                slotSymbols[i].transform.parent = TilesGroup;
                slotSymbols[i].transform.localPosition = new Vector3(0, radius * Mathf.Sin(tileAngleRad), -radius * Mathf.Cos(tileAngleRad));
                slotSymbols[i].transform.localScale = Vector3.one;
                slotSymbols[i].transform.localEulerAngles = new Vector3(tileAngleDeg, 0, 0);
                slotSymbols[i].name = "SlotSymbol: " + String.Format("{0:00}", i);
            }

            //set symbols
            for (int i = 0; i < tileCount; i++)
            {
                int symNumber = symbOrder[GetNextSymb()];
                slotSymbols[i].SetIcon(sprites[symNumber], symNumber);
            }
            lastChanged = tileCount - 1;

            SymbProbabilities = GetReelSymbHitPropabilities(sprites);
            CurrOrderPosition = 0; // offset  '- anglePerTileRad' - 

            // set random start position
            if (randomStartPosition)
            {
                NextOrderPosition = UnityEngine.Random.Range(0, symbOrder.Count);
                float angleX = GetAngleToNextSymb(NextOrderPosition);
                topSector += Mathf.Abs(Mathf.RoundToInt(angleX / anglePerTileDeg));
                topSector = (int)Mathf.Repeat(topSector, tileCount);
                TilesGroup.Rotate(-angleX, 0, 0);
                CurrOrderPosition = NextOrderPosition;
                WrapSymbolTape((-angleX));
            }
            if (debugreel) SignTopSymbol(topSector);
        }

        /// <summary>
        /// Async rotate cylinder
        /// </summary>
        internal void NextRotateCylinderEase(EaseAnim mainRotType, EaseAnim inRotType, EaseAnim outRotType,
                                        float mainRotTime, float mainRotateTimeRandomize,
                                        float inRotTime, float outRotTime,
                                        float inRotAngle, float outRotAngle,
                                        int nextOrderPosition,  Action rotCallBack)

        {
            NextOrderPosition =(!simulate)? nextOrderPosition : simPos;

            // start spin delay
            spinStartDelay = Mathf.Max(0, spinStartDelay);
            float spinStartRandomizeF = Mathf.Clamp(spinStartRandomize / 100f, 0f, 0.2f);
            float startDelay = UnityEngine.Random.Range(spinStartDelay * (1.0f - spinStartRandomizeF), spinStartDelay * (1.0f + spinStartRandomizeF));

            // check range before start
            inRotTime = Mathf.Clamp(inRotTime, 0, 1f);
            inRotAngle = Mathf.Clamp(inRotAngle, 0, 10);

            outRotTime = Mathf.Clamp(outRotTime, 0, 1f);
            outRotAngle = Mathf.Clamp(outRotAngle, 0, 10);

            // create reel rotation sequence - 4 parts  in - (continuous) - main - out
            float oldVal = 0f;
            tS = new TweenSeq();
            float angleX = 0;
            tempSectors = 0;

            tS.Add((callBack) => // in rotation part
            {
                SimpleTween.Value(gameObject, 0f, inRotAngle, inRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                     callBack();
                                  }).SetEase(inRotType).SetDelay(startDelay);
            });

            if (NextOrderPosition == -1)
                tS.Add((callBack) => // continuous rotation
                {
                    RecurRotation(mainRotTime / 1.0f, callBack);
                });

            tS.Add((callBack) =>  // main rotation part
            {
                oldVal = 0f;
                addRotateTime = Mathf.Max(0, addRotateTime);
                mainRotateTimeRandomize = Mathf.Clamp(mainRotateTimeRandomize, 0f, 0.2f);
                mainRotTime = addRotateTime + UnityEngine.Random.Range(mainRotTime * (1.0f - mainRotateTimeRandomize), mainRotTime * (1.0f + mainRotateTimeRandomize));

                spinSpeedMultiplier = Mathf.Max(0, spinSpeedMultiplier);
                angleX = GetAngleToNextSymb(NextOrderPosition) + anglePerTileDeg * symbOrder.Count * spinSpeedMultiplier;
               if(debugreel) Debug.Log(name + ", angleX : " + angleX);
                SimpleTween.Value(gameObject, 0, -(angleX + outRotAngle + inRotAngle), mainRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      // check rotation angle 
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                      if(val < -inRotAngle && val>= -(angleX + inRotAngle)) WrapSymbolTape(val + inRotAngle);
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      WrapSymbolTape(angleX);
                                      topSector += Mathf.Abs(Mathf.RoundToInt(angleX / anglePerTileDeg));
                                      topSector = (int)Mathf.Repeat(topSector, tileCount);
                                      if (debugreel) SignTopSymbol(topSector);
                               
                                      callBack();
                                  }).SetEase(mainRotType);
            });

            tS.Add((callBack) =>  // out rotation part
            {
                oldVal = 0f;
                SimpleTween.Value(gameObject, 0, outRotAngle, outRotTime)
                                  .SetOnUpdate((float val) =>
                                  {
                                      TilesGroup.Rotate(val - oldVal, 0, 0);
                                      oldVal = val;
                                  })
                                  .AddCompleteCallBack(() =>
                                  {
                                      CurrOrderPosition = NextOrderPosition;
                                      rotCallBack?.Invoke();
                                      callBack();
                                  }).SetEase(outRotType);
                RSound.SoundPlayReelStop(0, null);
            });

            tS.Start();
        }

        private void RecurRotation(float rotTime, Action completeCallBack)
        {
            float newAngle = -anglePerTileDeg * symbOrder.Count;
            tempSectors = 0;
            float oldVal = 0;
            SimpleTween.Value(gameObject, 0, newAngle, rotTime)
                                .SetOnUpdate((float val) =>
                                {
                                    if (this)
                                    {
                                        TilesGroup.Rotate(val - oldVal, 0, 0);
                                        oldVal = val;
                                        WrapSymbolTape(val); 
                                    }
                                })
                                .AddCompleteCallBack(() =>
                                {
                                    WrapSymbolTape(newAngle);
                                    tempSectors = 0;
                                    topSector += symbOrder.Count;
                                    topSector = (int)Mathf.Repeat(topSector, tileCount);
                                    if (NextOrderPosition == -1) RecurRotation(rotTime, completeCallBack);
                                    else {completeCallBack?.Invoke(); }
                                }).SetEase(EaseAnim.EaseLinear);
        }

        /// <summary>
        /// Change icon on reel appropriate to symbOrder
        /// </summary>
        private void WrapSymbolTape(float dA)
        {
            int sectors = Mathf.Abs(Mathf.RoundToInt(dA / anglePerTileDeg));
          //  if (sectors < tileCount-windowSize-2) return;

            bool found = false;

            for (int i = topSector + tempSectors; i < topSector + sectors + 3; i++)
            {
                int ip = (int)Mathf.Repeat(i, tileCount);
                tempSectors = i - topSector; // if (debugreel) Debug.Log("search sectors: " + sectors + ";  i: " + i);

                if (!found)
                {
                    found = (ip == lastChanged);
                }
                else // wrap tape at last changed
                {
                    if (debugreel) Debug.Log("found: " + found);
                    int symNumber = symbOrder[GetNextSymb()];
                    slotSymbols[ip].SetIcon(sprites[symNumber], symNumber);
                    lastChanged = ip; // if (debugreel) Debug.Log("set symbol in: " + ip + "; tempsectors: " + tempSectors);
                }
            }
        }

        /// <summary>
        /// Change icon on reel appropriate to symbOrder
        /// </summary>
        private void WrapSymbolTape1(float dA)
        {
            int sectors = Mathf.Abs(Mathf.RoundToInt(dA / anglePerTileDeg));
            bool found = false;
            for (int i = topSector + tempSectors; i < topSector + sectors + 3; i++)
            {
                int ip = (int)Mathf.Repeat(i, tileCount);
                tempSectors = i - topSector;
                if (debugreel) Debug.Log("search sectors: " + sectors + ";  i: " + i);

                found = (ip == lastChanged);
                if (found) //wrap tape at last changed
                {
                    if (debugreel) Debug.Log("found: " + found);
                    for (int ii = i + 1; ii < topSector + sectors + 3; ii++)
                    {
                        ip = (int)Mathf.Repeat(ii, tileCount);
                        int symNumber = symbOrder[GetNextSymb()];
                        slotSymbols[ip].SetIcon(sprites[symNumber], symNumber);
                        lastChanged = ip;
                        tempSectors++;
                        if (debugreel) Debug.Log("set symbol in: " + ip + "; tempsectors: " + tempSectors);
                    }
                    return;
                }
            }
        }

        int next = 0;
        /// <summary>
        /// Return next symb position  in symbOrder array
        /// </summary>
        /// <returns></returns>
        private int GetNextSymb()
        {
            return (int)Mathf.Repeat(next++, symbOrder.Count);
        }

        /// <summary>
        /// Return angle in degree to next symbol position in symbOrder array
        /// </summary>
        /// <param name="nextOrderPosition"></param>
        /// <returns></returns>
        private float GetAngleToNextSymb(int nextOrderPosition)
        {
            if (CurrOrderPosition < nextOrderPosition)
            {
                return (nextOrderPosition - CurrOrderPosition) * anglePerTileDeg;
            }
            return (symbOrder.Count - CurrOrderPosition + nextOrderPosition) * anglePerTileDeg;
        }

        /// <summary>
        /// Return probabilties for eac symbol according to symbOrder array 
        /// </summary>
        /// <returns></returns>
        internal float[] GetReelSymbHitPropabilities(SlotIcon[] symSprites)
        {
            if (symSprites == null || symSprites.Length == 0) return null;
            float[] probs = new float[symSprites.Length];
            int length = symbOrder.Count;
            for (int i = 0; i < length; i++)
            {
                int n = symbOrder[i];
                probs[n]++;
            }
            for (int i = 0; i < probs.Length; i++)
            {
                probs[i] = probs[i] / (float)length;
            }
            return probs;
        }

        /// <summary>
        /// Return true if top, middle or bottom raycaster has symbol with ID == symbID
        /// </summary>
        /// <param name="symbID"></param>
        /// <returns></returns>
        public bool HasSymbolInAnyRayCaster(int symbID, ref List<SlotSymbol> slotSymbols)
        {
            slotSymbols = new List<SlotSymbol>();
            bool res = false;
            SlotSymbol sS;

            for (int i = 0; i < rayCasters.Length; i++)
            {
                sS = rayCasters[i].GetSymbol();
                if (sS.IconID == symbID)
                {
                    res = true;
                    slotSymbols.Add(sS);
                }
            }

            return res;
        }

        /// <summary>
        /// Set next reel order while continuous rotation
        /// </summary>
        /// <param name="r"></param>
        internal void SetNextOrder(int r)
        {
            if (NextOrderPosition == -1)
                NextOrderPosition = r;
        }

        internal void CancelRotation()
        {
            SimpleTween.Cancel(gameObject, false);
            if (tS != null) tS.Break();
        }

        #region calculate
        public void CreateTriples()
        {
            triples = new List<Triple>();
            for (int i = 0; i < symbOrder.Count; i++)
            {
                int f = symbOrder[i];
                int s = symbOrder[(int)Mathf.Repeat(i + 1, symbOrder.Count)];
                int t = symbOrder[(int)Mathf.Repeat(i + 2, symbOrder.Count)];
                Triple triple = new Triple(new List<int> { f, s, t }, symbOrder.Count - 1);
                triples.Add(triple);
            }
        }

        public Triple GetRandomTriple()
        {
            int i = UnityEngine.Random.Range(0, symbOrder.Count);

            int f = symbOrder[i];
            int s = symbOrder[(int)Mathf.Repeat(i + 1, symbOrder.Count)];
            int t = symbOrder[(int)Mathf.Repeat(i + 2, symbOrder.Count)];

            return new Triple(new List<int> { f, s, t }, symbOrder.Count - 1);
        }
        #endregion calculate

        public void OrderTostring()
        {
            string res = "";
            for (int i = 0; i < symbOrder.Count; i++)
            {
                res += (i + ") ");
                res += symbOrder[i];
                if (i < symbOrder.Count - 1) res += "; ";
            }

            Debug.Log(res);
        }

        private void SignTopSymbol(int top)
        {
            for (int i = 0; i < slotSymbols.Length; i++)
            {
                if (slotSymbols[i].name.IndexOf("Top")!=-1) slotSymbols[i].name = "SlotSymbol: " + String.Format("{0:00}", i);
            }

            slotSymbols[top].name = "Top - " + slotSymbols[top].name;
        }
    }

    [Serializable]
    public class Triple
    {
        public List<int> ordering;
        public int number;

        public Triple(List<int> ordering, int number)
        {
            this.ordering = new List<int>(ordering);
            this.number = number;
        }

        public override string ToString()
        {
            string res = "";
            for (int i = 0; i < ordering.Count; i++)
            {
                res += ordering[i];
                if (i < ordering.Count - 1) res += ", ";
            }
            return res;
        }
    }
}
/*
  /// <summary>
        /// Change icon on reel appropriate to symbOrder
        /// </summary>
        private void WrapSymbolTape(float dA)
        {
            int sectors = Mathf.Abs(Mathf.RoundToInt(dA / anglePerTileDeg));
            bool found = false;
            for (int i = topSector + tempSectors; i < topSector + sectors + 3; i++)
            {
                int ip = (int)Mathf.Repeat(i, tileCount);
                tempSectors = i - topSector;
                if (debugreel) Debug.Log("search sectors: " + sectors + ";  i: " + i);

                if (!found)
                {
                    found = (ip == lastChanged);
                }
                else //if(found) wrap tape at last changed
                {
                    if (debugreel) Debug.Log("found: " + found);
                   // for (int ii = i+1; ii < topSector + sectors + 3; ii++)
                    {
                     //   ip = (int)Mathf.Repeat(ii, tileCount);
                        int symNumber = symbOrder[GetNextSymb()];
                        slotSymbols[ip].SetIcon(sprites[symNumber], symNumber, false);
                        lastChanged = ip;
                       // tempSectors++;
                        if (debugreel) Debug.Log("set symbol in: " + ip + "; tempsectors: " + tempSectors);
                    }
                   // return;
                }
            }
        }
 */
