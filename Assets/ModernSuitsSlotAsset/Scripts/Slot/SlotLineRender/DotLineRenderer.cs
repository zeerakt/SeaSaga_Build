using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class DotLineRenderer : SlotLineRenderer
    {
        [SerializeField]
        private Sprite dotSprite;
        [SerializeField]
        private Material material;
        [SerializeField]
        private int sortingOrder;
        [SerializeField]
        private float dotDistance = 2f;
        private int sortingLayerID = 0; //next updates

        #region temp vars
        private bool burnCancel = false;
        private List<SpriteRenderer> rend;
        private WaitForEndOfFrame wfef;
        private List<Color> colors; 
        #endregion temp vars

        #region override
        public override void Create(LinesController linesController, LineBehavior lineBehavior)
        {
            base.Create(linesController, lineBehavior);

            wfef = new WaitForEndOfFrame();
            Material mat = (!material) ? new Material(Shader.Find("Sprites/Default")) : material;

            List<Vector3> positions = new List<Vector3>();
            if (lineCreator && lineCreator.enabled && lineCreator.handlesPositions != null && lineCreator.handlesPositions.Count > 1)
            {
                foreach (var item in lineCreator.handlesPositions)
                {
                    positions.Add(transform.TransformPoint(item));
                }
            }
            else
            {
                // create line using raycasters
                foreach (var item in rayCasters)
                {
                    if (item)
                    {
                        positions.Add(item.transform.position);
                    }
                }
            }
           
            rend = CreateDotLine(positions, dotSprite, mat, sortingLayerID, sortingOrder, dotDistance, false);

            //2) cache data 
            if (rend != null && rend.Count > 0)
            {
                colors = new List<Color>(rend.Count);
                for (int i = 0; i < rend.Count; i++)
                {
                    colors.Add(rend[i].color);
                }
            }
            SetLineVisible(false);
        }

        /// <summary>
        /// Enable or disable the flashing material
        /// </summary>
        internal override void LineFlashing(bool flashing)
        {
            if (!this) return;
            Color c;
            if (flashing)
            {
                Color nC;
                SimpleTween.Value(gameObject, 0, Mathf.PI * 2f, 1f).SetOnUpdate((float val) =>
                {
                    for (int i = 0; i < rend.Count; i++)
                    {
                        float k = 0.5f * (Mathf.Cos(val) + 1f);
                        c = colors[i];
                        nC = new Color(c.r, c.g, c.b, c.a * k);
                        if (rend[i]) rend[i].color = nC;
                    }
                }).SetCycled();
            }
            else
            {
                SimpleTween.Cancel(gameObject, false);
                for (int i = 0; i < rend.Count; i++)
                {
                    c = colors[i];
                    if (rend[i]) rend[i].color = c;
                }
            }
        }

        internal override void LineBurn(bool burn, float burnDelay, Action completeCallBack)
        {
            burnCancel = (!burn) ? true : false;
            StopCoroutine("LineBurnC");
            SetLineVisible(false);
            if (burn)
                StartCoroutine(LineBurnC(3, burnDelay, completeCallBack));
        }

        /// <summary>
        /// Enable or disable line elemnts.
        /// </summary>
        internal override void SetLineVisible(bool visible)
        {
            if (rend == null) return;
            foreach (var item in rend)
             if(item) item.gameObject.SetActive(visible);
        }
        #endregion override

        #region private
        /// <summary>
        /// Create dotline use raycasters
        /// </summary>
        private List<SpriteRenderer> CreateDotLine(List<Vector3> positions, Sprite sprite, Material material, int sortingLayerID, int sortingOrder, float distance, bool setActive)
        {
            if (positions == null || positions.Count < 2) return null;
            List<SpriteRenderer> dList = new List<SpriteRenderer>();
            int length = positions.Count;

            for (int i = 0; i < length - 2; i++)
            {
                CreateDotLine(ref dList, sprite, material, positions[i], positions[i + 1], 0, sortingOrder, distance, true, false);
            }
            CreateDotLine(ref dList, sprite, material, positions[length - 2], positions[length - 1], 0, sortingOrder, distance, true, true);
            if (dList != null)
                dList.ForEach((r) => { if (r != null) r.gameObject.SetActive(setActive); });
            return dList;
        }

        /// <summary>
        /// Create dotLine tile between two points, use world coordinats
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="dist"></param>
        /// <param name="createStartPoint"></param>
        /// <param name="createEndPoint"></param>
        private void CreateDotLine(ref List<SpriteRenderer> dList, Sprite sprite, Material material, Vector3 start, Vector3 end, int sortingLayerID, int sortingOrder, float dist, bool createStartPoint, bool createEndPoint)
        {
            Vector3 dir = end - start;
            float seLength = dir.magnitude;

            if (createStartPoint) dList.Add(Creator.CreateSprite(transform, sprite, material, start, sortingLayerID, sortingOrder));

            if (seLength == 0) return;

            Vector3 dirOne = dir / seLength;
            float countf = (dist < seLength) ? seLength / dist + 1f : 2f;
            float count = Mathf.RoundToInt(countf);

            for (int i = 1; i < count - 1; i++)
            {
                dList.Add(Creator.CreateSprite(transform, sprite, material, start + dirOne * ((float)i * seLength / (count - 1f)), sortingLayerID, sortingOrder));
            }

            if (createEndPoint)
            {
                dList.Add(Creator.CreateSprite(transform, sprite, material, end, sortingLayerID, sortingOrder));
            }
        }

        private IEnumerator LineBurnC(int dotCount, float burnDelay, Action completeCallBack)
        {
            yield return burnDelay;
            if (lineBehavior.IsSelected && rend != null)
            {
                int p = 0;
                bool a;
                for (int c = 0; c < 2; c++)
                {
                    if (!lineBehavior.IsSelected) break;
                    if (burnCancel) break;
                    for (int i = 0; i < rend.Count + dotCount; i += dotCount)
                    {
                        if (burnCancel) break;

                        if (!lineBehavior.IsSelected) break;
                        for (int j = 0; j < dotCount; j++)
                        {
                            if ((p = i + j) >= rend.Count) break;
                            a = rend[p].gameObject.activeSelf;
                            rend[p].gameObject.SetActive(!a);
                        }
                        if (p >= rend.Count) break;
                        yield return wfef;
                    }
                    yield return new WaitForSeconds(1.5f);
                }
            }
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Set Order for line spite rendrer.
        /// </summary>
        private void SetLineRenderOrder(int order)
        {
            foreach (var item in rend)
                if (item) item.sortingOrder = order;
        }
        #endregion private
    }
}

       // private float burnTime = 1f; // next updates 
       // private int burnSpeed = 4;   // next updates