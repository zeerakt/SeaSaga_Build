using System;
using System.Collections.Generic;
using Mkey;
using UnityEngine;

namespace WinLoseSystem
{
    public class StarsController : MonoBehaviour
    {
        public event Action<int> OnPlayerEarnsStar;

        public int StarsCount => _starsCount;

        [SerializeField]
        private ObtainingStarsConditions _starsConditions;
        [SerializeField]
        private List<GameObject> _stars;

        private int _starsCount;

        private void Update()
        {
            UpdateStarsCount();
        }

        private void UpdateStarsCount()
        {
            var points = SlotPlayer.Instance.Coins;

            if (points < _starsConditions.FirstStarCondition && _starsCount != 0)
            {
                ShowStars(0);
            }
            else
            {
                if (points >= _starsConditions.WinStarCondition && _starsCount != 3)
                {
                    ShowStars(3);
                    return;
                }

                if (points >= _starsConditions.SecondStarCondition &&
                    points < _starsConditions.WinStarCondition &&
                    _starsCount != 2)
                {
                    ShowStars(2);
                    return;
                }

                if (points >= _starsConditions.FirstStarCondition &&
                    points < _starsConditions.SecondStarCondition &&
                    _starsCount != 1)
                {
                    ShowStars(1);
                }
            }
        }

        private void ShowStars(int count)
        {
            if (count < 0 || count > 3)
            {
                throw new Exception("You are trying to show wrong stars amount");
            }

            var starsChanged = 0;

            for(int i = 0; i < count; i++)
            {
                _stars[i].SetActive(true);
                starsChanged++;
            }

            for(int i = starsChanged; i < _stars.Count; i++)
            {
                _stars[i].SetActive(false);
            }

            _starsCount = count;
            OnPlayerEarnsStar?.Invoke(_starsCount);
        }
    }
}
