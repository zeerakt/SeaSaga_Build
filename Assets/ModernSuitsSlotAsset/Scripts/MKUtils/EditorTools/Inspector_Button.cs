using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

/*
 * 06.01.2020 - first
 */ 
namespace Mkey
{
    [ExecuteInEditMode]
    public class Inspector_Button : MonoBehaviour
    {
        public Action FirstClickD;
        public Action SecondClickD;
        public Action ThirdClickD;
        [SerializeField]
        private Button.ButtonClickedEvent clickFirstEvent;
        [SerializeField]
        private Button.ButtonClickedEvent clickSecondEvent;
        [SerializeField]
        private Button.ButtonClickedEvent clickThirdEvent;

        public void FirstButton_Click()
        {
            if (FirstClickD != null) FirstClickD.Invoke();
            if (clickFirstEvent != null) clickFirstEvent.Invoke();
        }

        public void SeconButton_Click()
        {
            if (SecondClickD != null) SecondClickD.Invoke();
            if (clickSecondEvent != null) clickSecondEvent.Invoke();
        }

        public void ThirdButton_Click()
        {
            if (ThirdClickD != null) ThirdClickD.Invoke();
            if (clickThirdEvent != null) clickThirdEvent.Invoke();
        }
    }
}