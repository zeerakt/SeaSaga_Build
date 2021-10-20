using System;
using UnityEngine;

namespace EnergySystem
{
    public class EnergyManager : MonoBehaviour
    {
        private const string SAVE_TIME_KEY = "saveTime";
        private const string SAVE_AMOUNT_KEY = "savedAmount";
        private const string SUBTRACT_ENERGY_TINE = "subtractTime";

        public int CurrentEnergyAmount { get; private set; }

        [SerializeField]
        private EnergySystemUIController _uiController;
        [SerializeField]
        private int _maxEnergyAmount;
        [SerializeField]
        private float _timeToRestoreOneEnergy;

        private bool _energyRestored = true;

        private float _timeToNextEnergy;

        private void Start()
        {

            
            LoadEnergy();
        }

        private void Update()
        {
            if (!_energyRestored)
            {
                if (Time.time >= _timeToNextEnergy)
                {
                    AddEnergy();
                }
            }
            UpdateEnergyInfo();
        }

        private int GetRestoredEnergy(DateTime saveTime)
        {
            var saveSeconds = GetTotalSecondDifference(saveTime);
            var restoredEnergy = Mathf.FloorToInt(saveSeconds / _timeToRestoreOneEnergy);
        
            return restoredEnergy;
        }
    
        private int ClampEnergyToMaxAmount(float value)
        {
            var clampedValue = Mathf.Clamp(value, 0, _maxEnergyAmount);
            return (int)clampedValue;
        }
    
        private static float GetTotalSecondDifference(DateTime time)
        {
            return (float)DateTime.UtcNow.Subtract(time).TotalSeconds;
        }
    
        private static string GetUtcNowAsString()
        {
            return DateTime.UtcNow.ToString();
        }
    
        private void SaveEnergy()
        {
            var dateTimeNow = GetUtcNowAsString();
            PlayerPrefs.SetString(SAVE_TIME_KEY, dateTimeNow);
            PlayerPrefs.SetInt(SAVE_AMOUNT_KEY, CurrentEnergyAmount);
        }
    
        private void LoadEnergy()
        {
            var savedAmount = ClampEnergyToMaxAmount(PlayerPrefs.GetInt(SAVE_AMOUNT_KEY));
            var saveTime = PlayerPrefs.GetString(SAVE_TIME_KEY, GetUtcNowAsString());
            var subtractTime = PlayerPrefs.GetString(SUBTRACT_ENERGY_TINE, GetUtcNowAsString());

            var restoredAmount = savedAmount + GetRestoredEnergy(DateTime.Parse(saveTime));
            CurrentEnergyAmount = ClampEnergyToMaxAmount(restoredAmount);
        
            _energyRestored = CurrentEnergyAmount == _maxEnergyAmount;
            if (!_energyRestored)
            {
                SetTimeToNextEnergy(DateTime.Parse(subtractTime));
            }
        }
    
        private void AddEnergy(int value = 1)
        {
            CurrentEnergyAmount = ClampEnergyToMaxAmount(CurrentEnergyAmount + value);
        
            if (CurrentEnergyAmount == _maxEnergyAmount)
            {
                _energyRestored = true;
            }
            else
            {
                StartEnergyRestoration();
            }
            SaveEnergy();
        }
    
        private void UpdateEnergyInfo()
        {
            var livesAmountLabel = _uiController.LivesAmountLabel;
            var restoreEstimateTime = _uiController.RestoreTimeEstimate;
        
            var restoreTimeEstimate = Mathf.FloorToInt(_timeToNextEnergy - Time.time);
            var estimateTimeSpan = new TimeSpan(0, 0, 0, restoreTimeEstimate);

            restoreEstimateTime.text = CurrentEnergyAmount == _maxEnergyAmount ? "MAX" : estimateTimeSpan.ToString(@"mm\:ss");
            livesAmountLabel.text = CurrentEnergyAmount.ToString();
        }
    
        public void SubtractEnergy(int value = 1)
        {
            CurrentEnergyAmount -= value;

            if (_energyRestored)
            {
                StartEnergyRestoration();
                PlayerPrefs.SetString(SUBTRACT_ENERGY_TINE, GetUtcNowAsString());
            }
            SaveEnergy();
        }
    
        private void StartEnergyRestoration()
        {
            _timeToNextEnergy = Time.time + _timeToRestoreOneEnergy;
            _energyRestored = false;
        }

        private void SetTimeToNextEnergy(DateTime value)
        {
            var timeFromPreviousSubtract = GetTotalSecondDifference(value);
            var timeToNextEnergy = _timeToRestoreOneEnergy - timeFromPreviousSubtract;
            _timeToNextEnergy = Mathf.Clamp(timeToNextEnergy, 0, _timeToRestoreOneEnergy);
        }
        
    }

}
