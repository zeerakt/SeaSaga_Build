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
        
*/
namespace Mkey
{
    public class SlotGuiController : GuiController
    {
        public static SlotGuiController Instance;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); }
            else
            {
                Instance = this;
                Application.targetFrameRate = 35;
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        public IList<PopUpsController> GetAllPopUps()
        {
            return PopupsList.AsReadOnly();
        }

        public void ShowCustomMessage()
        {

        }
    }
}

 
