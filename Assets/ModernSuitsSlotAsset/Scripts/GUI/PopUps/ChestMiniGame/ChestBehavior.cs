using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class ChestBehavior : MonoBehaviour
    {
        [SerializeField]
        private Image chestLight;
        [SerializeField]
        private Sprite closedChest;
        [SerializeField]
        private Sprite openedChest;
        [SerializeField]
        private CoinProcAnim coinsFountain;
        [SerializeField]
        private Transform coinsFountainPosition;
        [SerializeField]
        private Text coinsText;

        public int Coins { get; set; }
        #region temp vars
        private bool move = false;
        #endregion temp vars

        #region regular
        private void Start()
        {
            Button b = GetComponent<Button>();
            b.onClick.AddListener(Click);
        }

        private void OnDestroy()
        {
            SimpleTween.Cancel(gameObject, false);
        }
        #endregion regular

        public void ScaleOut(Action completeCallBack, float delay)
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt) rt.localScale = Vector3.zero;
            SimpleTween.Value(gameObject, Vector3.zero, Vector3.one, 0.5f).SetOnUpdate((Vector3 val) =>
           {
               if (rt) rt.localScale = val;
           })
            .SetDelay(delay)
            .SetEase(EaseAnim.EaseOutBounce)
            .AddCompleteCallBack(completeCallBack);
        }

        public void Open(Action completeCallBack)
        {
            TweenSeq ts = new TweenSeq();
            RectTransform rt = GetComponent<RectTransform>();
            Image im = GetComponent<Image>();

            if (rt) rt.localScale = Vector3.zero;

            ts.Add((callBack) =>
            {
                SimpleTween.Value(gameObject, Vector3.one, new Vector3(1.5f, 0.5f, 1f), 0.1f).SetOnUpdate((Vector3 val) =>
                {
                    if (rt) rt.localScale = val;
                })
          .AddCompleteCallBack(callBack);
            });

            ts.Add((callBack) =>
            {
                if(move)
                SimpleTween.Value(gameObject, 0, 5, 0.15f).SetOnUpdate((float val) =>
                {
                   if (rt) { rt.anchoredPosition -= new Vector2(0, val); }
                });
                SimpleTween.Value(gameObject, new Vector3(1.55f, 0.5f, 1f), new Vector3(1.00f, 1.00f, 1.00f), 0.25f).SetOnUpdate((Vector3 val) =>
                {
                    if (rt) rt.localScale = val;
                })
          .SetEase(EaseAnim.EaseOutBounce)
          .AddCompleteCallBack(callBack);
            });

            ts.Add((callBack) =>
            {
                if (openedChest && im)
                {
                    im.sprite = openedChest;
                }
                if (chestLight)
                {
                    chestLight.gameObject.SetActive(true);
                    SimpleTween.Value(gameObject, -Mathf.PI/4f, Mathf.PI/4f, 1f).SetOnUpdate((float val) =>
                      {
                          if (chestLight) chestLight.color = new Color(1, 1, 1, Mathf.Cos(val));
                      }).SetCycled();
                }
                if (coinsFountain)
                {
                    if (coinsFountainPosition) coinsFountain.transform.position = coinsFountainPosition.position;
                    coinsFountain.Jump();
                }
                if (coinsText)
                {
                    coinsText.gameObject.SetActive(true);
                    coinsText.text = Coins.ToString("# ### ### ### ###");
                }
            });

            ts.Add((callBack) =>
            {
                completeCallBack?.Invoke();
            });
            ts.Start();
        }

        public void Click()
        {
            Open(null);
        }
    }
}
