using UnityEngine;
using System;
using Mkey;

namespace MkeyFW
{
    public class ArrowBeviour : MonoBehaviour
    {
        [SerializeField]
        private bool showArrow = true;
        [SerializeField]
        private EaseAnim ease = EaseAnim.EaseInElastic;
        private float fadeTime = 0.25f;
        private float stayTime = 0.8f;
        private SpriteRenderer sR;
        private TweenSeq tS;

        #region regular
        void OnDestroy()
        {
            CancelTween();
        }
        #endregion regular

        /// <summary>
        /// Show arrow 2s and hide
        /// </summary>
        public void Show(int count, float delay)
        {
            if(!sR)  sR = GetComponent<SpriteRenderer>();

            if (!showArrow) return;
            if (tS != null) return;
            if (sR)
            {
                sR.color = new Color(1, 1, 1, 0);
                ArrowTween(count, delay, null);
            }
        }

        private void ArrowTween(int count, float delay, Action completeCallBack)
        {
            tS = new TweenSeq();
            if (count < 0) count = 0;
            for (int i = 0; i < count; i++)
            {
                tS.Add((callBack) =>  //fadein
                {

                    SimpleTween.Value(gameObject, 0, 1, fadeTime)
                        .SetOnUpdate((float val) => { if (this) sR.color = new Color(1, 1, 1, val); })
                        .SetDelay(delay)
                        .SetEase(ease)
                        .AddCompleteCallBack(callBack);
                });

                tS.Add((callBack) => //fadeout
                {
                    SimpleTween.Value(gameObject, 1, 0, fadeTime)
                        .SetOnUpdate((float val) => { if (this) sR.color = new Color(1, 1, 1, val); })
                        .SetDelay(stayTime)
                        .SetEase(ease)
                        .AddCompleteCallBack(callBack);
                });
            }


            tS.Add((callBack) =>
            {
                sR.color = new Color(1, 1, 1, 0);
                tS = null;
                completeCallBack?.Invoke();
                callBack?.Invoke();
            });
            tS.Start();
        }

        public void CancelTween()
        {
            if (this)
            {
                if (tS != null) tS.Break();
                tS = null;
                SimpleTween.Cancel(gameObject, false);
                if (sR) sR.color = new Color(1, 1, 1, 0);
            }
        }
    }
}