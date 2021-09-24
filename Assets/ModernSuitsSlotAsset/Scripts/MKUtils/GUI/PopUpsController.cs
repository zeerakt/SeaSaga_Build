using UnityEngine;
using UnityEngine.UI;
using System;
/*
230119
		old

	        Action<GameObject> openDelelegate;
			Action<GameObject> closeDelegate;
		new  
			Action<PopUpsController> openDelelegate;
			Action<PopUpsController> closeDelegate;
200219
    Invoke
    public void SetControlActivity(bool activity)

16.04.2019
    - c#6.0
    - add description

290419
     public EaseAnim inEase;
     public EaseAnim outEase;

16052019
     old - toggles[i].interactable = activity;
     new - inputFields[i].interactable = activity;
26.06.2019
     add close, open - flag

28.01.2020
    - add utils SetTextString(Text text, string textString), SetImageSprite(Image image, Sprite sprite
*/
namespace Mkey
{
    public enum WinAnimType {AlphaFade, Move, Scale}

    public enum Position {LeftMiddleOut, RightMiddleOut, MiddleBottomOut, MiddleTopOut, LeftMiddleIn, RightMiddleIn, MiddleBottomIn, MiddleTopIn, CustomPosition, AsIs, Center}

    public enum ScaleType {CenterXY, CenterX, CenterY, Top, Bottom, Left, Right}

    [RequireComponent(typeof(GuiFader_v2))]
    public class PopUpsController : MonoBehaviour
    {
        public string description;

        public bool IsVisible
        {
            get; private set;
        }
        private Action<PopUpsController> OpenEvent;
        private Action<PopUpsController> CloseEvent;
        private SoundMaster Sound { get { return SoundMaster.Instance; }}
        [SerializeField]
        private AudioClip openClip;
        [SerializeField]
        private bool playOpen;
        [SerializeField]
        private AudioClip closeClip;
        [SerializeField]
        private bool playClose;

        private bool close = false; // avoid double closing
        private bool open = false; // avoid double opening

        /// <summary>
        /// Fadeout window and play sound
        /// </summary>
        public void CloseWindow()
        {
            CloseWindow(playClose);
        }

        /// <summary>
        /// Fadeout window
        /// </summary>
        public void CloseWindow(bool playSound)
        {
            if (close) return;
            close = true;

            //Debug.Log("close window");
            SetControlActivity(false);
            playClose = playSound;
            if (playClose)
            {
                if (closeClip) Sound.PlayClip(0.2f, closeClip);
                else Sound.SoundPlayCloseWindow(0.2f, null);
            }
            GetComponent<GuiFader_v2>().FadeOut(0, () => {
                if (this)
                {
                    IsVisible = false;
                    CloseEvent?.Invoke(this);
                }
            });
        }

        /// <summary>
        /// Run after creating windows before it visible. Refresh window. Play open sound. FadeIn. Run  openDelegate. Set window control activity.
        /// </summary>
        public void ShowWindow()
        {
            ShowWindow(playOpen);
        }

        /// <summary>
        /// Run after creating windows before it visible. Refresh window. FadeIn. Run  openDelegate. Set window control activity.
        /// </summary>
        public void ShowWindow(bool playSound)
        {
            if (open) return;
            open = true;

            RefreshWindow();
            playOpen = playSound;
            if (playOpen)
            {
                if (openClip) Sound.PlayClip(0.2f, openClip);
                //else  Sound.SoundPlayOpenWindow(0.2f, null);
            }

            GetComponent<GuiFader_v2>().FadeIn(0, () =>
            {
                if (this)
                {
                    SetControlActivity(true);
                    IsVisible = true;
                    OpenEvent?.Invoke(this);
                }
            });
        }

        /// <summary>
        /// Set children buttons interactable = activity, toggles, 
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            Button[] buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }

            Toggle[] toggles = GetComponentsInChildren<Toggle>();
            {
                for (int i = 0; i < toggles.Length; i++)
                {
                    toggles[i].interactable = activity;
                }
            }

            InputField[] inputFields = GetComponentsInChildren<InputField>();
            {
                for (int i = 0; i < inputFields.Length; i++)
                {
                    inputFields[i].interactable = activity;
                }
            }
        }

        /// <summary>
        /// Set delegates openDel(started afetr opening), cleseDel started after closing
        /// </summary>
        /// <param name="openDel"></param>
        /// <param name="closeDel"></param>
        public void PopUpInit(Action<PopUpsController> openDel, Action<PopUpsController> closeDel)
        {
            if (openDel != null) OpenEvent += openDel;
            if (closeDel != null) CloseEvent += closeDel;
        }

        /// <summary>
        /// Refresh window data before it visible
        /// </summary>
        public virtual void RefreshWindow() { }

        private void  PlayOpenSound()
        {

        }

        private void PlayCloseSound()
        {

        }

        #region utils
        public void SetTextString(Text text, string textString)
        {
            if (text)
            {
                text.text = textString;
            }
        }

        public void SetImageSprite(Image image, Sprite sprite)
        {
            if (image)
            {
                image.sprite = sprite;
            }
        }
        #endregion utils
    }

    [Serializable]
    public class WindowOpions
    {
        public WinAnimType inAnim;
        public WinAnimType outAnim;

        public MoveAnim inMoveAnim;
        public MoveAnim outMoveAnim;

        public ScaleAnim inScaleAnim;
        public ScaleAnim outScaleAnim;

        public FadeAnim inFadeAnim;
        public FadeAnim outFadeAnim;

        public Position instantiatePosition;
        public Vector2 position;

        public EaseAnim inEase;
        public EaseAnim outEase;

        public WindowOpions()
        {
            inAnim = WinAnimType.AlphaFade;
            outAnim = WinAnimType.AlphaFade;
            inFadeAnim = new FadeAnim();
            outFadeAnim = new FadeAnim();
        }

        public WindowOpions(MoveAnim inMoveAnim, MoveAnim outMoveAnim)
        {
            inAnim = WinAnimType.Move;
            outAnim = WinAnimType.Move;
            this.inMoveAnim = inMoveAnim;
            this.outMoveAnim = outMoveAnim;
        }

        public WindowOpions(ScaleAnim inScaleAnim, ScaleAnim outScaleAnim)
        {
            inAnim = WinAnimType.Scale;
            outAnim = WinAnimType.Scale;
            this.inScaleAnim = inScaleAnim;
            this.outScaleAnim = outScaleAnim;
        }

        public WindowOpions(FadeAnim inFadeAnim, FadeAnim outFadeAnim)
        {
            inAnim = WinAnimType.AlphaFade;
            outAnim = WinAnimType.AlphaFade;
            this.inFadeAnim = inFadeAnim;
            this.outFadeAnim = outFadeAnim;
        }
    }

    [Serializable]
    public class FadeAnim 
    {
        public float time;
        public FadeAnim()
        {
            time = 0.2f;
        }
    }

    [Serializable]
    public class MoveAnim 
    {
        public Position toPosition;
        public float time;
        public Vector3 customPosition;
        public bool useMask;

        public MoveAnim()
        {
            time = 0.2f;
            toPosition = Position.AsIs;
        }
    }

    [Serializable]
    public class ScaleAnim 
    {
        public ScaleType scaleType;
        public float time;

        public ScaleAnim()
        {
            time = 0.2f;
            scaleType = ScaleType.CenterXY;
        }
    }

}

