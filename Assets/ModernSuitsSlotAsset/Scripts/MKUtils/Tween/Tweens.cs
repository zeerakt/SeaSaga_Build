using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class TweenIntValue
    {
        private int value;
        private int tweenId;
        private float maxTweenTime;
        private float minTweenTime;
        private Action<int> onUpdate;
        private GameObject g;
        private bool onlyPositive;

        public TweenIntValue(GameObject g, int initValue, float minTweenTime, float maxTweenTime, bool onlyPositive, Action<int> onUpdate)
        {
            value = initValue;
            this.maxTweenTime = Mathf.Max(0, maxTweenTime);
            this.minTweenTime = Mathf.Clamp(minTweenTime, 0, maxTweenTime);
            this.g = g;
            this.onlyPositive = onlyPositive;
            this.onUpdate = onUpdate;
        }

        public void Tween(int newValue, int valuePerSecond)
        {
            SimpleTween.Cancel(tweenId, false);
            float add = newValue - value;

            if ((add > 0 && onlyPositive) || !onlyPositive)
            {
                valuePerSecond = Mathf.Max(1, valuePerSecond);
                float tT = Mathf.Abs((float)add / (float)valuePerSecond);
                tT = Mathf.Clamp(tT, minTweenTime, maxTweenTime);
                tweenId = SimpleTween.Value(g, value, newValue, tT).SetOnUpdate((float val) =>
                {
                    if (this != null)
                    {
                        value = (int)val;
                        onUpdate?.Invoke(value);
                    }
                }).ID;
            }
            else if (onlyPositive)
            {
                if (this != null)
                {
                    value = newValue;
                    onUpdate?.Invoke(value);
                }
            }
        }
    }
}
