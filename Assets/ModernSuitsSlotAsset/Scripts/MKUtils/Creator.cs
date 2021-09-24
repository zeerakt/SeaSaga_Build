using System;
using UnityEngine;

/* Helper class for sprite creating and prefab instantiation
 * 11.11.2019 - first
 */

namespace Mkey
{
    public class Creator : MonoBehaviour
    {
        /// <summary>
        /// Instantiate prefab at position, set parent, parent lossyScale, set sorting order and sorting layer foreach SpiteRenderers
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="destroyTime"></param>
        internal static GameObject InstantiatePrefab(GameObject prefab, Transform parent, Vector3 position, int sortingLayer, int sortingOrder)
        {
            if (!prefab) return null;
            GameObject g = InstantiatePrefab(prefab, parent, position, 0);
            if (!g) return null;
            SpriteRenderer[] sRs = g.GetComponentsInChildren<SpriteRenderer>();
            if (sRs != null)
            {
                foreach (var sR in sRs)
                {
                    if (sR)
                    {
                        sR.sortingLayerID = sortingLayer;
                        sR.sortingOrder = sortingOrder;
                    }
                }
            }
            return g;
        }

        /// <summary>
        /// Instantiate prefab at position, set parent, parent lossyScale, and if (destroyTime>0) destroy result gameobject after destroytime.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="destroyTime"></param>
        internal static GameObject InstantiatePrefab(GameObject prefab, Transform parent, Vector3 position, float destroyTime)
        {
            if (!prefab) return null;
            GameObject g = Instantiate(prefab);
            if (!g) return null;
            g.transform.position = position;
            if (parent)
            {
                g.transform.localScale = parent.lossyScale;
                g.transform.parent = parent;
            }
            if (destroyTime > 0) Destroy(g, destroyTime);
            return g;
        }

        /// <summary>
        /// Instantiate sprite anim prefab at position, set parent, parent lossyScale, set sortingOrder
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="destroyTime"></param>
        internal static GameObject InstantiateAnimPrefab(GameObject prefab, Transform parent, Vector3 position, int sortingOrder, bool destroyAfterPlay, Action completeCallback)
        {
            if (!prefab)
            {
                completeCallback?.Invoke();
                return null;
            }

            GameObject g = Instantiate(prefab);
            if (!g)
            {
                completeCallback?.Invoke();
                return null;
            }

            g.transform.position = position;
            if (parent)
            {
                g.transform.localScale = parent.lossyScale;
                g.transform.parent = parent;
            }
            SpriteRenderer sR = g.GetComponent<SpriteRenderer>();
            if (sR) sR.sortingOrder = sortingOrder;

            AnimCallBack aC = g.GetComponent<AnimCallBack>();
            if (aC)
            {
                aC.SetEndCallBack(() => { if (destroyAfterPlay) Destroy(g); completeCallback?.Invoke(); });
            }
            else
            {
                completeCallback?.Invoke();
            }
            return g;
        }

        /// <summary>
        /// Instantiate new 3D Sprite at position, and set parent (if parent !=null), set scale like parent lossyScale
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sprite"></param>
        /// <param name="position"></param>
        /// <param name="renderLayer"></param>
        /// <param name="renderOrder"></param>
        /// <returns></returns>
        internal static SpriteRenderer CreateSprite(Transform parent, Sprite sprite, Vector3 position)
        {
            GameObject gO = new GameObject();

            if (parent)
            {
                gO.transform.localScale = parent.lossyScale;
                gO.transform.parent = parent;
            }

            gO.transform.position = position;
            SpriteRenderer sR = gO.AddComponent<SpriteRenderer>();
            sR.sprite = sprite;
            return sR;
        }

        /// <summary>
        /// Instantiate new 3D Sprite at position, and set parent (if parent !=null), set scale like parent lossyScale, set sortingLayerID, sortingOrder
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sprite"></param>
        /// <param name="position"></param>
        /// <param name="renderLayer"></param>
        /// <param name="renderOrder"></param>
        /// <returns></returns>
        internal static SpriteRenderer CreateSprite(Transform parent, Sprite sprite, Vector3 position, int sortingOrder)
        {
            SpriteRenderer sr = CreateSprite(parent, sprite, position);
            if (sr)
            {
                sr.sortingOrder = sortingOrder;
            }
            return sr;
        }

        /// <summary>
        /// Instantiate new 3D Sprite at position, and set parent (if parent !=null), set scale like parent lossyScale, set sortingLayerID, sortingOrder
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sprite"></param>
        /// <param name="position"></param>
        /// <param name="renderLayer"></param>
        /// <param name="renderOrder"></param>
        /// <returns></returns>
        internal static SpriteRenderer CreateSprite(Transform parent, Sprite sprite, Vector3 position, int sortingLayerID, int sortingOrder)
        {
            SpriteRenderer sr = CreateSprite(parent, sprite, position);
            if (sr)
            {
                sr.sortingLayerID = sortingLayerID;
                sr.sortingOrder = sortingOrder;
            }
            return sr;
        }

        /// <summary>
        /// Instantiate new 3D Sprite at position, and set parent (if parent !=null), set scale like parent lossyScale, set sortingLayerID, sortingOrder
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="sprite"></param>
        /// <param name="position"></param>
        /// <param name="renderLayer"></param>
        /// <param name="renderOrder"></param>
        /// <returns></returns>
        internal static SpriteRenderer CreateSprite(Transform parent, Sprite sprite, Material material, Vector3 position, int sortingLayerID, int sortingOrder)
        {
            SpriteRenderer sr = CreateSprite(parent, sprite, position, sortingLayerID, sortingOrder);
            if (sr && material) sr.material = material;
            return sr;
        }
    }
}