using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
	public class LinesController : MonoBehaviour
    {
        [SerializeField]
        private SlotController slot;
        [SerializeField]
        private SlotControls controls;

        [SerializeField]
        [Tooltip("Destroy existing and create all possible lines at start using raycasters")]
        private bool createAllPossibleLines = false;
        [SerializeField]
        [Tooltip("Select all lines at start, or only first line")]
        private bool selectAllLines = true;
        [SerializeField]
        [Tooltip("Default line renderer prefab")]
        private SlotLineRenderer slotLineRendererPrefab;
        [SerializeField]
        [Tooltip("Burn all selected lines at scene start, if createAllPossibleLines == true - not work")]
        private bool burnLinesAtStart = true;

        [Header("Test")]
        [Space]
        public TextMesh winTextPrefab;
        public bool showWinText;

        #region properties
        public int LinesCount { get { return (Lines!=null)? Lines.Count : 0; } }
        public List<LineBehavior> Lines { get; private set; }
        #endregion properties

        #region temp vars
      //  private bool setLineVisible = false;
        private float burnDelay = 0.0f;
        private SlotPlayer SPlayer
        {
            get { return SlotPlayer.Instance; }
        }
        #endregion temp vars

        #region regular
        private void Start()
        {
            controls.ChangeSelectedLinesEvent += ChangeSelectedLinesHandler;

            if (createAllPossibleLines && slot)
            {
                // remove all existing lines
                Lines = new List<LineBehavior>(GetComponentsInChildren<LineBehavior>());
                foreach (var item in Lines)
                {
                    if (item)
                    {
                        DestroyImmediate(item.gameObject);
                    }
                }
                Lines = null;

                // create all possible lines
                List<int[]> rcCombos = new List<int[]>();
                SlotGroupBehavior[] sGB = slot.slotGroupsBeh;

                int[] rcCounts = new int[sGB.Length];// raycasters counts by reel
                for (int i = 0; i < sGB.Length; i++)
                {
                    rcCounts[i] = sGB[i].RayCasters.Length;
                 //   Debug.Log(" rcCounts[i]: " + rcCounts[i]);
                }

                rcCombos = CreateRCCombos(rcCounts);
               // Debug.Log("rcCombos: " + rcCombos.Count);
                RayCaster[] rcComb = new RayCaster[sGB.Length];
                for (int i = 0; i < rcCombos.Count; i++)
                {
                    int[] combo = rcCombos[i];
                    for (int j = 0; j < combo.Length; j++)
                    {
                        int rcNum = combo[j]-1;
                        rcComb[j] =(rcNum>=0) ? sGB[j].RayCasters[rcNum] : null;
                    }
                    CreateLine(rcComb, i+1);
                }
            }

            Lines = new List<LineBehavior>(GetComponentsInChildren<LineBehavior>());
            
            // sort lines by number
            Lines.Sort((LineBehavior a, LineBehavior b) =>
            {
                if (a == null & b == null) return 0;
                else if (a == null) return -1;
                else if (b == null) return 1;
                else
                {
                    return a.number.CompareTo(b.number);
                }
            });

            Lines.ForEach((l) => {l.InitStart(slot,this, slotLineRendererPrefab, createAllPossibleLines); });
            if (selectAllLines)
            {
                SelectAllLines(burnLinesAtStart);
            }
            else
            {
                SelectFirstLine(burnLinesAtStart);
            }
        }

        private void OnDestroy()
        {
           if(controls) controls.ChangeSelectedLinesEvent -= ChangeSelectedLinesHandler;
        }
        #endregion regular

        public void SetControlActivity(bool activity)
        {
            LineButtonBehavior[] lbs = GetComponentsInChildren<LineButtonBehavior>();

            foreach (var item in lbs)
            {
                item.interactable = activity;
            }
        }

        #region line select
        private void SelectFirstLine(bool burn)
        {
            if (controls) controls.SetSelectedLinesCount(1, burn);
        }

        public void SelectAllLines(bool burn)
        {
            if (createAllPossibleLines && controls.SelectedLinesCount > 0) return; //avoid max bet
            if (Lines == null || Lines.Count == 0) return;
			if(controls) controls.SetSelectedLinesCount(Lines.Count, burn);
        }

        internal void LineButton_Click(LineBehavior line)
        {
            if(!line) return;
            if (!line.LineButton) return;
            if (!line.LineButton.interactable) return;

            int count = 1;
            if (line.IsSelected)
            {
                count = line.number - 1;
            }
            else
            {
                count = line.number;
            }
            if (controls) controls.SetSelectedLinesCount(count, true);
        }
        #endregion line select

        private void CreateLine(RayCaster [] raycasters, int number)
        {
            GameObject l = new GameObject();
            LineBehavior lB = l.AddComponent<LineBehavior>();
            l.transform.parent = transform.transform;
            lB.rayCasters = new RayCaster[raycasters.Length];
            for (int i = 0; i < raycasters.Length; i++)
            {
                lB.rayCasters[i] = raycasters[i];
            }
            lB.name = "Line " + number;
            lB.number = number;
        }

        /// <summary>
        /// Return all possible rc combos by rc number (from 1 to rc.length)
        /// </summary>
        /// <param name="counts"></param>
        /// <returns></returns>
        private List<int[]> CreateRCCombos(int[] counts)
        {
            List<int[]> res = new List<int[]>();
            int length = counts.Length;
            int decLength = length-1;
            int[] counter = new int[length];
            for (int i = decLength; i >= 0; i--)
            {
                counter[i] = (counts[i] > 0) ? 1 : 0; // 0 - empty 
            }
            int[] copy = new int[length];//copy arr
            counter.CopyTo(copy, 0);
            res.Add(copy);

            bool result = true;
            while (result)
            {
                result = false;
                for (int i = decLength; i >= 0; i--)    // find new combo
                {
                    if (counter[i] < counts[i] && counter[i]>0) 
                    {
                        counter[i]++;
                        if (i != decLength) // reset low "bytes"
                        {
                            for (int j = i + 1; j < length; j++)
                            {
                               if(counter[j] > 0) counter[j] = 1;
                            }
                        }
                        result = true;
                        copy = new int[length];//copy arr
                        counter.CopyTo(copy, 0);
                        res.Add(copy);
                        break;
                    }
                }
            }
            return res;
        }

        #region event handlers
        private void ChangeSelectedLinesHandler(int newCount, bool burn)
        {
            newCount = Mathf.Min(newCount, Lines.Count);
            for (int i = 0; i < Lines.Count; i++)
            {
                if (i < newCount)
                {
                    if (!Lines[i].IsSelected)
                    {
                        Lines[i].Select(burn, burnDelay);
                    }
                }
                else
                {
                    if (Lines[i].IsSelected)
                    {
                        Lines[i].DeSelect();
                    }
                }
            }
        }
        #endregion event handlers
    }
}
