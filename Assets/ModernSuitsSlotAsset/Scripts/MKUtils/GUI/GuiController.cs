using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
/*
100219
	fixed  private void PopUpCloseH(PopUpsController pUP)
	old  Destroy(pUP);
	new  Destroy(pUP.gameObject);
	
	fixed  internal bool HasNoPopUp
			old   get { return PopupsList.Count > 0; }
			new   get { return PopupsList.Count == 0; }
 200219
    base GuiController
 25.06.2019
   -add styled message show
     public void ShowMessageWithYesNoCloseButton(WarningMessController prefab, string caption, string message, Action yesCallBack, Action cancelCallBack, Action noCallBack)
        {
            WarningMessController wMC = CreateMessage(prefab, caption, message, yesCallBack, cancelCallBack, noCallBack);
        }
  30.07.2019 - fixed     SetControlActivity (activity)
    inputFields[i].interactable = activity;

    add   public bool IsActive { get; private set; }
  08.04.2020
    -update messages
  
  03.12.2020 
    -IEnumerator CloseMessageC

*/
namespace Mkey
{
    [RequireComponent(typeof(Canvas))]
    public class GuiController : MonoBehaviour
    {
        [SerializeField]
        private PopUpsController MessageWindowPrefab;

        protected List<PopUpsController> PopupsList;

        #region properties
        public bool HasNoPopUp
        {
            get { return (PopupsList==null) ? true : (PopupsList.Count == 0); }
        }

        public bool IsActive { get; private set; }
        #endregion properties

        #region regular
        protected virtual void Start()
        {
            PopupsList = new List<PopUpsController>();
        }
        #endregion regular

        #region protected
        protected PopUpsController ShowPopUp(PopUpsController popup_prefab, Action openCallBack, Action closeCallBack)
        {
            return ShowPopUp(popup_prefab, transform, openCallBack, closeCallBack);
        }

        protected PopUpsController ShowPopUp(PopUpsController popup_prefab,  Transform parent, Action openCallBack, Action closeCallBack)
        {
            if (!popup_prefab) return null;
            PopUpsController pUp = CreateWindow(popup_prefab, parent);
            if (pUp)
            {
                pUp.PopUpInit(
                    (g) =>
                    {
                        PopUpOpenEventHandler(g); openCallBack?.Invoke();
                    }, (g) =>
                    {
                        PopUpCloseEventHandler(g);
                        closeCallBack?.Invoke();
                    });
                pUp.ShowWindow();
            }
            return pUp;
        }

        protected PopUpsController ShowPopUp(PopUpsController popup_prefab, Transform parent, Vector3 position, Action openCallBack, Action closeCallBack)
        {
            if (!popup_prefab) return null;
            PopUpsController pUp = CreateWindow(popup_prefab, parent, position);
            if (pUp)
            {
                pUp.PopUpInit(
                    (g) =>
                    {
                        PopUpOpenEventHandler(g);
                        openCallBack?.Invoke();
                    }, (g) =>
                    {
                        PopUpCloseEventHandler(g);
                        closeCallBack?.Invoke();
                    });
                pUp.ShowWindow();
            }
            return pUp;
        }

        private PopUpsController CreateWindow(PopUpsController prefab, Transform parent)
        {
            GameObject gP = (GameObject)Instantiate(prefab.gameObject, parent);
            RectTransform mainRT = gP.GetComponent<RectTransform>();
            mainRT.SetParent(parent);
            WindowOpions winOptions = gP.GetComponent<GuiFader_v2>().winOptions;
            Vector3[] vC = new Vector3[4];
            mainRT.GetWorldCorners(vC);

            RectTransform rt = gP.GetComponent<GuiFader_v2>().guiPanel;
            Vector3[] vC1 = new Vector3[4];
            rt.GetWorldCorners(vC1);
            float height = (vC1[2] - vC1[0]).y;
            float width = (vC1[2] - vC1[0]).x;

            switch (winOptions.instantiatePosition)
            {
                case Position.LeftMiddleOut:
                    rt.position = new Vector3(vC[0].x - width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.RightMiddleOut:
                    rt.position = new Vector3(vC[2].x + width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.MiddleBottomOut:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[0].y - height / 2f, rt.position.z);
                    break;
                case Position.MiddleTopOut:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[2].y + height / 2f, rt.position.z);
                    break;
                case Position.LeftMiddleIn:
                    rt.position = new Vector3(vC[0].x + width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.RightMiddleIn:
                    rt.position = new Vector3(vC[2].x - width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.MiddleBottomIn:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[0].y + height / 2f, rt.position.z);
                    break;
                case Position.MiddleTopIn:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[2].y - height / 2f, rt.position.z);
                    break;
                case Position.CustomPosition:
                    rt.position = winOptions.position;
                    break;
                case Position.AsIs:
                    break;
                case Position.Center:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
            }
            PopUpsController pUp = gP.GetComponent<PopUpsController>();
            if (pUp)
            {
                pUp.SetControlActivity(false);
            }
            return pUp;
        }

        private PopUpsController CreateWindow(PopUpsController prefab, Transform parent, Vector3 position)
        {
            GameObject gP = (GameObject)Instantiate(prefab.gameObject, parent);
            RectTransform mainRT = gP.GetComponent<RectTransform>();
            mainRT.SetParent(parent);
            WindowOpions winOptions = gP.GetComponent<GuiFader_v2>().winOptions;

            Vector3[] vC = new Vector3[4];
            mainRT.GetWorldCorners(vC);

            RectTransform rt = gP.GetComponent<GuiFader_v2>().guiPanel;
            Vector3[] vC1 = new Vector3[4];
            rt.GetWorldCorners(vC1);
            float height = (vC1[2]-vC1[0]).y;
            float width = (vC1[2] - vC1[0]).x;
            if (winOptions == null) winOptions = new WindowOpions();
            winOptions.position =  position;

            switch (winOptions.instantiatePosition)
            {
                case Position.LeftMiddleOut:
                    rt.position = new Vector3(vC[0].x - width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.RightMiddleOut:
                    rt.position = new Vector3(vC[2].x + width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.MiddleBottomOut:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[0].y - height / 2f, rt.position.z);
                    break;
                case Position.MiddleTopOut:
                    rt.position = new Vector3((vC[0].x + vC[2].x)/2f,  vC[2].y + height / 2f, rt.position.z);
                    break;
                case Position.LeftMiddleIn:
                    rt.position = new Vector3(vC[0].x + width / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
                case Position.RightMiddleIn:
                    rt.position = new Vector3(vC[2].x - width / 2f, (vC[0].y + vC[2].y)/ 2f, rt.position.z);
                    break;
                case Position.MiddleBottomIn:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[0].y + height / 2f, rt.position.z);
                    break;
                case Position.MiddleTopIn:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, vC[2].y - height / 2f, rt.position.z);
                    break;
                case Position.CustomPosition:
                    rt.position = winOptions.position;
                    break;
                case Position.AsIs:
                    break;
                case Position.Center:
                    rt.position = new Vector3((vC[0].x + vC[2].x) / 2f, (vC[0].y + vC[2].y) / 2f, rt.position.z);
                    break;
            }
            PopUpsController pUp = gP.GetComponent<PopUpsController>();
            if (pUp)
            {
                pUp.SetControlActivity(false);
            }
            return pUp;
        }
        #endregion protected

        #region public

        public PopUpsController ShowPopUp(PopUpsController popup_prefab)
        {
            PopUpsController pUp = ShowPopUp(popup_prefab, null, null);
            return pUp;
        }

        /// <summary>
        /// Set children buttons interactable = activity, toggles, 
        /// </summary>
        /// <param name="activity"></param>
        public void SetControlActivity(bool activity)
        {
            IsActive = activity;
            Button[] buttons = GetComponentsInChildren<Button>();
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].interactable = activity;
            }

            Toggle [] toggles = GetComponentsInChildren<Toggle>();
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
        #endregion public 

        #region messages - simple popup with yes, no, close buttons, caption and message text field 
        public WarningMessController ShowMessageWithCloseButton(string caption, string message, Action cancelCallBack)
        {
            return ShowMessageWithYesNoCloseButton(caption, message, null, cancelCallBack, null);
        }

        public WarningMessController ShowMessageWithYesCloseButton(string caption, string message, Action yesCallBack, Action cancelCallBack)
        {
            return ShowMessageWithYesNoCloseButton(caption, message, yesCallBack, cancelCallBack, null);
        }

        public WarningMessController ShowMessageWithYesNoButton(string caption, string message, Action yesCallBack, Action noCallBack)
        {
            return ShowMessageWithYesNoCloseButton(caption, message, yesCallBack, null, noCallBack);
        }

        public WarningMessController ShowMessageWithYesNoCloseButton(string caption, string message, Action yesCallBack, Action cancelCallBack, Action noCallBack)
        {
            return CreateMessage(MessageWindowPrefab, caption, message, yesCallBack, cancelCallBack, noCallBack);
        }

        public WarningMessController ShowMessageWithYesNoCloseButton(WarningMessController prefab, string caption, string message, Action yesCallBack, Action cancelCallBack, Action noCallBack)
        {
            return CreateMessage(prefab, caption, message, yesCallBack, cancelCallBack, noCallBack);
        }

        /// <summary>
        /// Show message without buttons, auto closing 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        /// <param name="completeCallBack"></param>
        public WarningMessController ShowMessage(string caption, string message, float showTime, Action completeCallBack)
        {
            WarningMessController pUp = CreateMessage(MessageWindowPrefab, caption, message, null, null, null);
            StartCoroutine(CloseMessageC(pUp, showTime, completeCallBack));
            return pUp;
        }

        /// <summary>
        /// Show message without buttons, auto closing 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="message"></param>
        /// <param name="showTime"></param>
        /// <param name="completeCallBack"></param>
        public WarningMessController ShowMessage(WarningMessController prefab, string caption, string message, float showTime, Action completeCallBack)
        {
            WarningMessController pUp = CreateMessage(prefab, caption, message, null, null, null);
            StartCoroutine(CloseMessageC(pUp, showTime, completeCallBack));
            return pUp;
        }

        private IEnumerator CloseMessageC(WarningMessController pUp, float showTime, Action completeCallBack)
        {
            while (showTime > 0 && pUp)
            {
                yield return new WaitForEndOfFrame();
                showTime -= Time.deltaTime;
            }
            completeCallBack?.Invoke();
            if (pUp) pUp.CloseWindow(); // check if popup exist
        }

        private WarningMessController CreateMessage(PopUpsController prefab, string caption, string message, Action yesCallBack, Action cancelCallBack, Action noCallBack)
        {
            PopUpsController p = CreateWindow(prefab, transform);
            WarningMessController pUp = p.GetComponent<WarningMessController>();

            pUp.SetControlActivity(false);
            pUp.PopUpInit(new Action<PopUpsController>(PopUpOpenEventHandler), (g) =>
            {
                PopUpCloseEventHandler(g);
                switch (pUp.Answer)
                {
                    case MessageAnswer.Yes:
                        yesCallBack?.Invoke();
                        break;
                    case MessageAnswer.Cancel:
                        cancelCallBack?.Invoke();
                        break;
                    case MessageAnswer.No:
                        noCallBack?.Invoke();
                        break;
                }
            });
            pUp.SetMessage(caption, message, yesCallBack != null, cancelCallBack != null, noCallBack != null);
            p.ShowWindow();
            return pUp;
        }
        #endregion messages

        #region handlers
        /// <summary>
        /// Add to popuplist
        /// </summary>
        /// <param name="pUP"></param>
        private void PopUpOpenEventHandler(PopUpsController pUP)
        {
            if (PopupsList.IndexOf(pUP) == -1)
            {
                PopupsList.Add(pUP);
            }
        }

        /// <summary>
        /// Remove from list and destroy
        /// </summary>
        /// <param name="pUP"></param>
        private void PopUpCloseEventHandler(PopUpsController pUP)
        {
            if (PopupsList.IndexOf(pUP) != -1)
            {
                PopupsList.Remove(pUP);
                Destroy(pUP.gameObject);
            }
        }
        #endregion handlers
    }
}

 
