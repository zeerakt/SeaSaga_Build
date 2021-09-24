using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MkeyFW
{
    public class WheelEvent : MonoBehaviour
    {
        public void TestEvent_1()
        {
            Debug.Log("Test Event _1");
        }

        public void HitEvent_2500000()
        {
            Debug.Log("Hit Event 2 500 000");
        }

        public void HitEvent_10000000()
        {
            Debug.Log("Hit Event 10 000 000");
        }

        public void ResultEvent()
        {
            Debug.Log("Result event");
        }

        public void CloseWheel()
        {

        }
    }
}