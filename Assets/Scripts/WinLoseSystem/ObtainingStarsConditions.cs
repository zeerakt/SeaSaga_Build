using System;
using UnityEngine;

namespace WinLoseSystem
{
    [Serializable]
    public class ObtainingStarsConditions
    {
        public long FirstStarCondition => _firstStarCondition;
        public long SecondStarCondition => _secondStarCondition;
        public long WinStarCondition => _winStarCondition;

        [SerializeField]
        private long _firstStarCondition;

        [SerializeField]
        private long _secondStarCondition;

        [SerializeField]
        private long _winStarCondition;
    }
}
