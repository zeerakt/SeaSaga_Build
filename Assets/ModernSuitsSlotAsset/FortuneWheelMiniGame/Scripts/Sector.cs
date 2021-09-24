using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using Mkey;

namespace MkeyFW
{
    [ExecuteInEditMode]
    public class Sector : MonoBehaviour
    {
        [SerializeField]
        private int coins;
        [SerializeField]
        private bool bigWin;
        [SerializeField]
        private List<GameObject> hitPrefabs;
        private float destroyTime = 3f;
        [SerializeField]
        private UnityEvent  hitEvent;

        [SerializeField]
        public AudioClip hitSound;

        private TextMesh textMesh;

        public int Coins
        {
            get { return coins; }
            set { coins = Mathf.Max(0, value); RefreshText(); }
        }

        public bool BigWin
        {
            get { return bigWin; }
        }

        #region regular
        void Start()
        {
            textMesh = GetComponent<TextMesh>();
            RefreshText();
        }

        void OnValidate()
        {
           coins = Mathf.Max(0, coins);
           RefreshText();
        }
        #endregion regular

        private void RefreshText()
        {
            if (!textMesh) textMesh = GetComponent<TextMesh>();
            if (!textMesh) return;
            var f = new NumberFormatInfo { NumberGroupSeparator = " " };
            textMesh.text = Coins.ToString("n0", f); // textMesh.text = coins.ToString("N0", CultureInfo.CreateSpecificCulture("en-US"));
        }

        /// <summary>
        /// Instantiate all prefabs and invoke hit event
        /// </summary>
        /// <param name="position"></param>
        public void PlayHit(Vector3 position)
        {
            if (hitPrefabs != null)
            {
                foreach (var item in hitPrefabs)
                {
                    if (item)
                    {
                        Transform partT = Instantiate(item).transform;
                        partT.position = position;
                        if (this && partT) Destroy(partT.gameObject, destroyTime);
                    }
                }
            }
            if (hitEvent != null) hitEvent.Invoke();
        }


    }
}