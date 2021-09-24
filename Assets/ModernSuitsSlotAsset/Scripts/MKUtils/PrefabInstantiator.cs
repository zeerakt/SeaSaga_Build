using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    27.08.2020 - first
 */
namespace Mkey
{
    public class PrefabInstantiator : MonoBehaviour
    {
        [SerializeField]
        private GameObject prefab;

        public void InstantiatePrefab()
        {
            if (prefab)
            {
                Instantiate(prefab, transform);
            }
        }
    }
}
