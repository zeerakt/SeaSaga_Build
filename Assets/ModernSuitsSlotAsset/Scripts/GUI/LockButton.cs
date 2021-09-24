using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Mkey
{
    public class LockButton : MonoBehaviour
    {
        [SerializeField]
        private int level;

        void Start()
        {
            Button b = GetComponent<Button>();
            if (!b) return;
            b.interactable = level <= SlotPlayer.Instance.Level;
        }
    }
}
