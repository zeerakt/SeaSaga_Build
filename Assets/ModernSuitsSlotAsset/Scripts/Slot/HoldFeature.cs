using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class HoldFeature : MonoBehaviour
    {
        [SerializeField]
        private TextMesh[] multiplierText;
        [SerializeField]
        private SceneButton[] holdButtons;
        [SerializeField]
        private int maxHold = 2;

        private int betMultiplier_1 = 2;
        private int betMultiplier_2 = 4;
        private int betMultiplier_3 = 6;
        private int betMultiplier_4 = 8;
        #region events
        public Action<int> ChangeBetMultiplierEvent;
        #endregion events

        #region properties
        public int Length { get { return (holdButtons != null) ? holdButtons.Length : 0; } }
        public bool UseMultiplier { get; set; }

        #endregion properties

        #region temp vars
        private List<SceneButton> pressed;
        private int multiplier;
        private bool[] holdReels;
        #endregion temp vars

        #region regular
        private void Start()
		{
            foreach (var item in holdButtons)
            {
                item.clickEventAction += ClickEvenHandler;
            }

            pressed = new List<SceneButton>();
            holdReels = new bool[holdButtons.Length];
            if (multiplierText!=null)
            {
                foreach (var item in multiplierText)
                {
                    if (item) item.text = GetMultiplier().ToString();
                }
            }
		}
		#endregion regular

        public void HoldAllButOneReel(int buttonIndex)
        {
            ReleaseAllButtons();
            
            for (int i = 0; i < holdButtons.Length; i++)
            {
                if (i == buttonIndex)
                {
                    continue;
                }
                
                holdButtons[i].SetPressed();
                ClickEvenHandlerWithoutMultiplier(holdButtons[i]);
            }
        }

        private void ClickEvenHandlerWithoutMultiplier(SceneButton button)
        {
            if (!button.Pressed && pressed.Contains(button))
            {
                pressed.Remove(button);
            }

            else if(button.Pressed && pressed.Count < maxHold)
            {
                pressed.Add(button);
            }

            else if (button.Pressed && pressed.Count>0)
            {
                SceneButton b = pressed[0];
                b.Release();
                pressed.Remove(b);
                pressed.Add(button);
            }

            for (int i = 0; i < holdButtons.Length; i++)
            {
                holdReels[i] = holdButtons[i].Pressed;
            }
        }

        private void ClickEvenHandler(SceneButton button)
        {
            if (!button.Pressed && pressed.Contains(button))
            {
                pressed.Remove(button);
            }

            else if(button.Pressed && pressed.Count < maxHold)
            {
                pressed.Add(button);
            }

            else if (button.Pressed && pressed.Count>0)
            {
                SceneButton b = pressed[0];
                b.Release();
                pressed.Remove(b);
                pressed.Add(button);
            }

            multiplier = GetMultiplier();
            if (multiplierText != null)
            {
                foreach (var item in multiplierText)
                {
                    if (item) item.text = GetMultiplier().ToString();
                }
            }
            ChangeBetMultiplierEvent?.Invoke(multiplier);

            for (int i = 0; i < holdButtons.Length; i++)
            {
                holdReels[i] = holdButtons[i].Pressed;
            }
        }

        public int GetMultiplier()
        {
            if(pressed==null || pressed.Count == 0)
            {
                return 1;
            }
            else if (pressed.Count == 1)
            {
                return Mathf.Max(1, betMultiplier_1);
            }
            else if (pressed.Count == 2)
            {
                return Mathf.Max(1, betMultiplier_2);
            }
            else if (pressed.Count == 3)
            {
                return Mathf.Max(1, betMultiplier_3);
            }
            return Mathf.Max(1, betMultiplier_4);
        }

        public  bool [] GetHoldReels()
        {
            return holdReels;
        }

        public void ReleaseAllButtons()
        {
            foreach (var holdButton in holdButtons)
            {
                holdButton.Release();
                ClickEvenHandler(holdButton);
            }
        }
    }
}
/*
**********Description*****************
The hold feature is one of the mainstays of fruit machine play, and is one of a couple of ways in which a modicum of skill is introduced into these games.
While you are playing one of these games, you can be randomly granted a number of holds at any time. 
How exactly these rewards will work varies depending on what game you are playing.
It’s possible that you’ll have a number of holds that must be used on the next spin, while other machines could give you a few that you may choose to use over the course of your next several plays.
When you choose to hold a reel, that one will not move the next time you activate a spin. In almost all cases, you will have the ability to hold multiple reels if you have enough holds left.
In some cases, this means that you can lock in a guaranteed win just by keeping enough of the same symbols on the winning payline!
Even in games that allow you to hoard your holds for a while, they will typically eventually expire, and it may not be immediately obvious when this is going to happen.
However, this feature also tends to be awarded frequently and generously, so even if you miss out on using one or two extra at some point, you should soon gain a feel for when they have to be utilized.
 */
