using UnityEngine;
using System.Collections.Generic;
using System;

namespace Mkey
{
    public class PayTableController : PopUpsController
    {
        [SerializeField]
        private GameObject[] tabs;
        [Space(10)]
        [SerializeField]
        private PayLineHelper payLineHelperPrefab;
        [SerializeField]
        private RectTransform payLinesGridParent;
        [SerializeField]
        private bool createPayLinesAuto = true;

        #region temp vars
        private int currTabIndex = 0;
        private SlotController slot;
        #endregion temp vars

        public override void RefreshWindow()
        {
            if (createPayLinesAuto) CreatePayLinesInfo();
            SetActiveTab(currTabIndex);
            base.RefreshWindow();
        }

        private void CreatePayLinesInfo()
        {
            slot = SlotController.CurrentSlot;

            LineBehavior[] lines = FindObjectsOfType<LineBehavior>();

            Array.Sort(lines, delegate (LineBehavior x, LineBehavior y) // sort by name  x==y ->0; x>y ->1; x<y -1
            {
                if (x == null)
                {
                    if (y == null)
                    {
                        return 0;// If x is null and y is null, they'reequal. 
                    }
                    else
                    {
                        return -1;// If x is null and y is not null, y is greater.
                    }
                }
                else
                {
                    return x.name.CompareTo(y.name);
                }
            });

            foreach (var item in lines)
            {
                PayLineHelper pLH = Instantiate(payLineHelperPrefab); //Debug.Log(item.ToString());
                pLH.Create(item, slot.slotGroupsBeh);
                RectTransform pLHRT = pLH.GetComponent<RectTransform>();
                pLHRT.localScale = payLinesGridParent.lossyScale;
                pLHRT.SetParent(payLinesGridParent);
            }
        }

        private void GetChilds(GameObject g, ref List<GameObject> gList)
        {
            int childs = g.transform.childCount;
            if (childs > 0)//The condition that limites the method for calling itself
                for (int i = 0; i < childs; i++)
                {
                    Transform gT = g.transform.GetChild(i);
                    GameObject gC = gT.gameObject;
                    if (gC) gList.Add(gC);
                    GetChilds(gT.gameObject, ref gList);
                }
        }

        public void NextTab_Click()
        {
            currTabIndex = (int)Mathf.Repeat(++currTabIndex, tabs.Length);
            SetActiveTab(currTabIndex);
        }

        public void PrevTab_Click()
        {
            currTabIndex = (int)Mathf.Repeat(--currTabIndex, tabs.Length);
            SetActiveTab(currTabIndex);
        }

        private void SetActiveTab(int index)
        {
            if (tabs == null || tabs.Length == 0) return;
            if (index < 0) index = 0;
            if (index >= tabs.Length) index = tabs.Length - 1;
            for (int i = 0; i < tabs.Length; i++)
            {
                if (tabs[i]) tabs[i].SetActive(i == index);
            }
        }
    }
}