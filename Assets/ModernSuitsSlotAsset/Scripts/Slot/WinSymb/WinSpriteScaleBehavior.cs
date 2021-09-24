using System;
using UnityEngine;

namespace Mkey
{
    public class WinSpriteScaleBehavior : WinSymbolBehavior
    {
        [SerializeField]
        private int count = 3;

        private TweenSeq tweenSeq;
        private Vector3 localScale;

        #region override
        protected override void PlayWin()
        {
            if (!Symbol) return;

            Transform t = Symbol.transform;
            tweenSeq = new TweenSeq();
            localScale = t.localScale;
            count = (count < 1) ? 1 : count;

            for (int i = 0; i < count; i++)
            {
                // 1 scale  out
                tweenSeq.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, localScale.x, localScale.x * 1.1f, 0.15f).SetOnUpdate((float val) =>
                    {
                        if (this) t.localScale = new Vector3(val, val, 1);
                    }).AddCompleteCallBack(() => { callBack(); }).SetEase(EaseAnim.EaseInSine);
                });

                // 2 scale  in
                tweenSeq.Add((callBack) =>
                {
                    SimpleTween.Value(gameObject, localScale.x * 1.1f, localScale.x, 0.15f).SetOnUpdate((float val) =>
                    {
                        if (this) t.localScale = new Vector3(val, val, 1);
                    }).AddCompleteCallBack(() => { callBack(); }).SetEase(EaseAnim.EaseInSine);
                });
            }

            //3 
            tweenSeq.Add((callBack) =>
            {
                if (this && t) t.localScale = localScale;
                callBack();
            });

            tweenSeq.Start();
        }

        protected override void Cancel()
        {
            if (!this) return;
            if (tweenSeq != null) tweenSeq.Break();
            SimpleTween.Cancel(gameObject, false);
            if (Symbol) Symbol.transform.localScale = localScale;
        }
        #endregion override
    }
}