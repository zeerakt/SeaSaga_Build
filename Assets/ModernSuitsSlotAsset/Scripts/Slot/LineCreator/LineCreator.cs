using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    [ExecuteInEditMode]
    public class LineCreator : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private bool curveCreated;
        [SerializeField]
        public List<Vector3> handlesPositions;
        [SerializeField]
        private bool useLineInfoColor = true;
        [SerializeField]
        private bool extendLine = false;

        #region properties
        /// <summary>
        /// Return handles count
        /// </summary>
        public int HandlesCount
        {
            get
            {
                if (handlesPositions == null) return 0;
                return handlesPositions.Count;
            }
        }
        #endregion properties

        #region regular
        private void Start()
        {
            CreateInitialHandles();
        }
        #endregion regular

        public void OnChangeLine()
        {
            
        }

        public void RemovePoint(int selectedIndex)
        {
            Debug.Log("Remove point: " + selectedIndex);
            handlesPositions.RemoveAt(selectedIndex);
        }

        public void AddPoint(int selectedIndex)
        {
            Debug.Log("Add point: " + selectedIndex);
            Vector3 p0 = handlesPositions[selectedIndex];
            Vector3 p1 = handlesPositions[selectedIndex + 1];
            Vector3 pn = (p1 + p0) / 2.0f;
            handlesPositions.Insert(selectedIndex + 1, pn);
        }

        public void SetInitial()
        {
            curveCreated = false;
            CreateInitialHandles();
        }

        #region private methods
        private void CreateInitialHandles()
        {
            if (curveCreated) return;
            Debug.Log("create initial curve");
            LineBehavior lineBehavior = GetComponent<LineBehavior>();
            int rCount = 0;
            handlesPositions = new List<Vector3>();
            if (lineBehavior && lineBehavior.rayCasters!=null)
            {
                foreach (var item in lineBehavior.rayCasters)
                {
                    if (item)
                    {
                        handlesPositions.Add(transform.InverseTransformPoint(item.transform.position));
                        rCount++;
                    }
                }

                if (rCount >= 2 && extendLine)
                {
                    Vector3 cPoint = handlesPositions[0];
                    float dx = cPoint.x - handlesPositions[1].x;
                    handlesPositions.Insert(0, new Vector3(cPoint.x + 0.5f* dx, cPoint.y, cPoint.z));

                    cPoint = handlesPositions[handlesPositions.Count - 1];
                    handlesPositions.Add(new Vector3(cPoint.x - 0.5f * dx, cPoint.y, cPoint.z));
                }
                curveCreated = true;
            }
           // Debug.Log("Initial curve created:" + p0 + ":" + p1 + ":" + p2 + ":" + p3);
        }

        public void Display()
        {
            if (HandlesCount < 2) return;
            for (int i = 0; i < HandlesCount-1; i++)
            {
                Debug.DrawLine(transform.TransformPoint(handlesPositions[i]), transform.TransformPoint(handlesPositions[i+1]), Color.white);
            }
            Debug.Log("draw" + name);
        }

        public Color GetLineColor()
        {
            if (useLineInfoColor && GetComponent<LineBehavior>())
            {
                return GetComponent<LineBehavior>().lineInfoColor;
            }
            return Color.white;
        }
        #endregion private methods

    }

}