using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class WinJumpBehavior : WinSymbolBehavior
    {
        private GameObject tweenClone;
        private TweenSeq tweenSeq;

        #region override
        protected override void PlayWin()
        {
            if (!Slot || !Slot.topJumpTarget || !Slot.bottomJumpTarget)
            {
                return;
            }

            Transform firstPos = Slot.topJumpTarget;
            Transform secPos = Slot.bottomJumpTarget;

            tweenSeq = new TweenSeq();
            // 0 create clone
            tweenClone = CreateJumpClone();

            // 1 scale clone
            tweenSeq.Add((callBack) =>
            {
                SimpleTween.Value(tweenClone, transform.localScale.x, transform.localScale.x * 2f, 0.2f).SetOnUpdate((float val) =>
                {
                    if (!tweenClone.activeSelf)
                    {
                        tweenClone.SetActive(true);
                    }
                    tweenClone.transform.localScale = new Vector3(val, val, val);
                }).AddCompleteCallBack(() => { callBack(); });
                //   
            });

            // 2 jump to first position  
            tweenSeq.Add((callBack) =>
            {
                SimpleTween.Move(tweenClone, tweenClone.transform.position, firstPos.position, 0.5f).AddCompleteCallBack(() => { callBack(); }).SetEase(EaseAnim.EaseOutBounce);
            });

            //3 jump to second position 
            tweenSeq.Add((callBack) =>
            {
                SimpleTween.Move(tweenClone, tweenClone.transform.position, secPos.position, 0.5f).SetEase(EaseAnim.EaseInCirc).AddCompleteCallBack(() => { callBack(); });

                SimpleTween.Value(tweenClone, tweenClone.transform.localScale.x, 0, 0.25f).SetOnUpdate((float val) => { tweenClone.transform.localScale = new Vector3(val, val, val); }).SetDelay(0.26f).
                AddCompleteCallBack(() =>
                {
                    Destroy(tweenClone);
                });
            });

            tweenSeq.Start();
        }

        protected override void Cancel()
        {
            if (!this) return;
            if (tweenSeq != null) tweenSeq.Break();
            if (tweenClone) SimpleTween.Cancel(tweenClone, false);
            if (tweenClone) Destroy(tweenClone);
        }
        #endregion override

        #region private
        private GameObject CreateJumpClone()
        {
            if (!SymbolSprite) return null;
            return Creator.CreateSprite(transform, SymbolSprite, transform.position, 0, SymbolSortingOrder + GetNextAddSortingOrder()).gameObject;
        }
        #endregion private
    }
}