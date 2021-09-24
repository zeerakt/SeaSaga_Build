using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    [Serializable]
    public class SlotIcon
    {
        [HideInInspector]
        [SerializeField]
        public string name = "eee";
        public Sprite iconSprite;
        public bool useWildSubstitute = true;
        public Sprite iconBlur;
        [Space(8)]
        [SerializeField]
        private List<WinSymbolBehavior> privateWinBehaviors;

        public string Name
        {
            get { return name; }
        }

        public void OnBeforeSerialize()
        {
            name = (iconSprite) ? iconSprite.name : "null";
        }

        public SlotIcon(Sprite iconSprite, List<WinSymbolBehavior> privateWinBehaviors, bool useWildSubstitute)
        {
            this.iconSprite = iconSprite;
            this.privateWinBehaviors = privateWinBehaviors;
            this.useWildSubstitute = useWildSubstitute;
        }

        public WinSymbolBehavior GetWinPrefab(string tag)
        {
            if (privateWinBehaviors == null || privateWinBehaviors.Count == 0) return null;
            foreach (var item in privateWinBehaviors)
            {
                if (item.WinTag.Contains(tag))
                {
                    return item;
                }
            }
            return null;
        }
    }
}
