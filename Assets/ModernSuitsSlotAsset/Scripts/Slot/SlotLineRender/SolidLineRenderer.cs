using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class SolidLineRenderer : SlotLineRenderer
    {
        [SerializeField]
        private float width = 0.15f;
        [SerializeField]
        private Material material;
        [SerializeField]
        private bool useBehColor = true;
        [SerializeField]
        private int sortingOrder = 0;
        [SerializeField]
        private LineRenderer addLineRenderer;

        private int sortingLayerID = 0; //next updates
        #region temp vars
        private LineRenderer lineRenderer;
        private bool burnCancel = false;
        private WaitForEndOfFrame wfef;
        private List<LineRenderer> rend;
        private List<Color> colors;
        #endregion temp vars

        #region override
        public override void Create (LinesController linesController, LineBehavior lineBehavior)
        {
            base.Create(linesController, lineBehavior);

            wfef = new WaitForEndOfFrame();
            Material mat = (!material) ? new Material(Shader.Find("Sprites/Default")) : material;

            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.material = mat;
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
            if (useBehColor)
            {
                lineRenderer.startColor = lineBehavior.lineInfoColor;
                lineRenderer.endColor = lineBehavior.lineInfoColor;
            }
            lineRenderer.sortingOrder = sortingOrder + GetNextAddSortingOrder();
            lineRenderer.sortingLayerID = sortingLayerID;

            List<Vector3> positions = new List<Vector3>(); // world pos
            List<Vector3> hP = new List<Vector3>(); // local pos

            if (lineCreator && lineCreator.enabled && lineCreator.handlesPositions != null && lineCreator.handlesPositions.Count > 1)
            {
                foreach (var item in lineCreator.handlesPositions)
                {
                    positions.Add(transform.TransformPoint(item));
                    hP.Add(item);
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
                        hP.Add(transform.InverseTransformPoint(item.transform.position));
                    }
                }
            }


            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());

            // setup add linerenderer
            if (addLineRenderer)
            {
                addLineRenderer.material = mat;
                addLineRenderer.startWidth = width + 0.1f;
                addLineRenderer.endWidth = width + 0.1f;
                Color c = lineRenderer.startColor;
                c = new Color(c.r, c.g, c.b, 0.3f);
                addLineRenderer.startColor = c;
                addLineRenderer.endColor = c;

                addLineRenderer.sortingOrder = lineRenderer.sortingOrder - 1;
                addLineRenderer.sortingLayerID = sortingLayerID;

                addLineRenderer.positionCount = positions.Count;
                addLineRenderer.SetPositions(positions.ToArray());
            }

            //2) cache color source
            rend = new List<LineRenderer>();
            colors = new List<Color>();
            if (lineRenderer)
                if (lineRenderer) rend.Add(lineRenderer);
            if (addLineRenderer)
                if (addLineRenderer) rend.Add(addLineRenderer);

            foreach (var item in rend)
            {
                colors.Add(item.startColor);
                colors.Add(item.endColor);
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
                SimpleTween.Value(gameObject, 0, Mathf.PI*2f, 1f).SetOnUpdate((float val) =>
                {
                    
                    for (int i = 0; i < rend.Count; i++)
                    {
                        float k = 0.5f * (Mathf.Cos(val) + 1f);
                        c = colors[2*i];
                        nC = new Color(c.r, c.g, c.b, c.a * k);
                        if (rend[i]) rend[i].startColor = nC;
                        c = colors[2 * i + 1];
                        nC = new Color(c.r, c.g, c.b, c.a * k);
                        if (rend[i]) rend[i].endColor = nC;
                    }
                }).SetCycled();
            }
            else
            {
                SimpleTween.Cancel(gameObject, false);
                for (int i = 0; i < rend.Count; i++)
                {
                    c = colors[2 * i];
                    if (rend[i]) rend[i].startColor = c;
                    c = colors[2 * i + 1];
                    if (rend[i])  rend[i].endColor = c;
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
            if (lineRenderer) lineRenderer.enabled = visible;
            if (addLineRenderer) addLineRenderer.enabled = visible;
        }
        #endregion override

        #region private
        private IEnumerator LineBurnC(int dotCount, float burnDelay, Action completeCallBack)
        {
            yield return burnDelay;
            SetLineVisible(true);

            for (int i = 0; i < 15; i++)
            {
                if (burnCancel) break;
                if (!lineBehavior.IsSelected) break;
                yield return wfef;
            }
            SetLineVisible(false);
            completeCallBack?.Invoke();
        }

        /// <summary>
        /// Set Order for line spite rendrer.
        /// </summary>
        private void SetLineRenderOrder(int order)
        {
            if (lineRenderer) lineRenderer.sortingOrder = order;
            if (addLineRenderer) addLineRenderer.sortingOrder = order-1;
        }
        #endregion private
    }
}