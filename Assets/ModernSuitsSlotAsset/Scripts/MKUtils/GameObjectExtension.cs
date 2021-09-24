using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
	11.11.2019 - first
*/

namespace Mkey
{
    // GetInterface method for gameobject
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Returns all monobehaviours (casted to T)
        /// </summary>
        /// <typeparam name="T">interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T[] GetInterfaces<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            //  var mObjs = gObj.GetComponents<MonoBehaviour>();
            var mObjs = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }

        /// <summary>
        /// Returns the first monobehaviour that is of the interface type (casted to T)
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T GetInterface<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            return gObj.GetInterfaces<T>().FirstOrDefault();
        }

        /// <summary>
        /// Returns the first instance of the monobehaviour that is of the interface type T (casted to T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T GetInterfaceInChildren<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            return gObj.GetInterfacesInChildren<T>().FirstOrDefault();
        }

        /// <summary>
        /// Gets all monobehaviours in children that implement the interface of type T (casted to T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        public static T[] GetInterfacesInChildren<T>(this GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");

            var mObjs = gObj.GetComponentsInChildren<MonoBehaviour>();

            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }

        public static T GetOrAddComponent<T>(this Component child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.gameObject.AddComponent<T>();
            }
            return result;
        }

        public static T GetOrAddComponent<T>(this GameObject child) where T : Component
        {
            T result = child.GetComponent<T>();
            if (result == null)
            {
                result = child.gameObject.AddComponent<T>();
            }
            return result;
        }

        /// <summary>
        /// Get all childs recursively
        /// </summary>
        /// <param name="g"></param>
        /// <param name="gList"></param>
        public static void GetChilds(GameObject g, ref List<GameObject> gList)
        {
            int childs = g.transform.childCount;
            if (childs > 0)//The condition that limites the method for calling itself
                for (int i = 0; i < childs; i++)
                {
                    Transform gT = g.transform.GetChild(i);
                    GameObject gC = gT.gameObject;
                    if (gC) gList.Add(gC);
                    GetChilds(gT.gameObject, ref gList);
                }
        }
    }
}
