using System;
using UnityEngine;

namespace Mkey
{
    public class WinSpriteBlinkBehavior : WinSymbolBehavior
    {
        [SerializeField]
        private int count = 3;

        private TweenSeq tweenSeq;

        #region override
        protected override void PlayWin()
        {
            SpriteRenderer sR = GetComponent<SpriteRenderer>();
            if (!sR)
            {
                return;
            }

            sR.sortingOrder = SymbolSortingOrder + GetNextAddSortingOrder();
            tweenSeq = new TweenSeq();
            Color c = sR.color;
            count = (count < 1) ? 1 : count;

            for (int i = 0; i < count; i++)
            {
                // 1 fade  out
                tweenSeq.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 1.0f, 0, 0.25f).SetOnUpdate((float val) =>
                    {
                        if (sR) sR.color = new Color(c.r, c.g, c.b, val);
                    }).AddCompleteCallBack(() => { callBack(); }).SetEase(EaseAnim.EaseInSine);
                });

                // 2 fade  in
                tweenSeq.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, 0, 1f, 0.25f).SetOnUpdate((float val) =>
                    {
                        if (sR) sR.color = new Color(c.r, c.g, c.b, val);
                    }).AddCompleteCallBack(() => { callBack(); }).SetEase(EaseAnim.EaseInSine);
                });
            }

            //3 
            tweenSeq.Add((callBack) =>
            {
                callBack();
            });

            tweenSeq.Start();
        }

        protected override void Cancel()
        {
            if (!this) return;
            if (tweenSeq != null) tweenSeq.Break();
            SimpleTween.Cancel(gameObject, false);
        }
        #endregion override
    }
}