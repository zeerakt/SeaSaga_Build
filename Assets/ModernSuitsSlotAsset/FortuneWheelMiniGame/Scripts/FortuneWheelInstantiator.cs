using System;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
using UnityEngine.UI;
using UnityEngine.Events;

namespace MkeyFW
{
    public class FortuneWheelInstantiator : MonoBehaviour
    {
        [SerializeField]
        private WheelController fortuneWheelPrefab;
        [SerializeField]
        private EaseAnim ease = EaseAnim.EaseOutBack;
        [SerializeField]
        private bool autoClose = false;
        [SerializeField]
        private float autoCloseTime = 5f;

        #region temp vars

        #endregion temp vars

        #region properties
        public WheelController MiniGame { get; private set; }
        private SlotPlayer MPlayer { get { return SlotPlayer.Instance; } }
        private SlotGuiController MGui { get { return SlotGuiController.Instance; } }
        #endregion properties

        #region events
        public Action<int, bool> SpinResultEvent;
        public Action <WheelController> CreateEvent;
        public Action CloseEvent;
        #endregion events

        internal void Create(bool autoStart)
        {
            if (fortuneWheelPrefab == null) return;

            if (fortuneWheelPrefab)
            {
                if (MiniGame)
                {
                    WheelController t = MiniGame;
                    MiniGame = null;
                    Destroy(t);
                }
                MiniGame = Instantiate(fortuneWheelPrefab, transform);
                MiniGame.transform.position = transform.position;
                MiniGame.transform.localScale = Vector3.zero;
                MiniGame.SpinResultEvent += ResultEventHandler;
                MiniGame.SetBlocked(true, true);

                SimpleTween.Value(gameObject, 0, 1, 0.25f)
                    .SetOnUpdate((float val) =>
                    {
                        MiniGame.transform.localScale = new Vector3(val, val, val);
                    })
                    .AddCompleteCallBack(() => 
                    {
                        CreateEvent?.Invoke(MiniGame);
                    })
                    .SetEase(ease);

                SimpleTween.Value(gameObject, 0, 1, 0.5f).AddCompleteCallBack(() =>
                {
                    if (autoStart) MiniGame.StartSpin(() => 
                    {

                    });
                });
            }
        }

        internal void Close(float delay, Action completeCallBack)
        {
            if (MiniGame)
            {
                MiniGame.SetBlocked(true, true);
                SimpleTween.Value(gameObject, 1, 0, 0.25f)
                   .SetOnUpdate((float val) =>
                   {
                       MiniGame.transform.localScale = new Vector3(val, val, val);
                   })
                   .AddCompleteCallBack(() =>
                   {
                       if (MiniGame) Destroy(MiniGame.gameObject);
                       CloseEvent?.Invoke();
                       completeCallBack?.Invoke();
                   })
                   .SetDelay(delay);
            }
        }

        internal void Close()
        {
            Close(autoCloseTime, null);
        }

        internal void ForceClose()
        {
            Close(0, null);
        }

        private void ResultEventHandler(int coins, bool isBigWin)
        {
            SpinResultEvent?.Invoke(coins, isBigWin);
            if (autoClose)
            {
                Close();
            }
           
        }
    }
}