//#define useinterface

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
using System.Linq;

/*
    changes
    11.02.19 
        remove link to GameObjectExt class
        remove private fields isActive, touch
        add  ScreenTouchPos 
        add events
            public Action<TouchPadEventArgs> ScreenDragEvent;
            public Action<TouchPadEventArgs> ScreenPointerDownEvent;
            public Action<TouchPadEventArgs> ScreenPointerUpEvent;
        add TouchPadEventArgs class
        add ICustomMessageTarget : IEventSystemHandler

    29.09.19
        - fixed double pointer up
        - add class TouchPadMessageTarget
    12.12.19
        - fixed  OnPointerUp, OnPointerExit  (IsTouched = false)
        - fixed  OnPointerDown  (IsTouched = true)
 */

namespace Mkey
{
    public class TouchPad : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IBeginDragHandler, IDropHandler, IPointerExitHandler
    {
        #region events
        public Action<TouchPadEventArgs> ScreenDragEvent;
        public Action<TouchPadEventArgs> ScreenPointerDownEvent;
        public Action<TouchPadEventArgs> ScreenPointerUpEvent;
        #endregion events

        #region properties
        /// <summary>
        /// Return drag direction in screen coord
        /// </summary>
        public Vector2 ScreenDragDirection
        {
            get { return ScreenTouchPos - oldPosition; }
        }

        /// <summary>
        /// Return world position of touch.
        /// </summary>
        public Vector3 WorldTouchPos
        {
            get { return Camera.main.ScreenToWorldPoint(ScreenTouchPos); }
        }

        public Vector2 ScreenTouchPos { get; private set; }

        /// <summary>
        /// Return true if touchpad is touched with mouse or finger
        /// </summary>
        public bool IsTouched { get; private set; }

        /// <summary>
        /// Return true if touch activity enabled
        /// </summary>
        public bool IsActive { get; private set; }
        #endregion properties

        [SerializeField]
        private bool dlog = false;

        #region temp vars
        private List<Collider2D> hitList;
        private List<Collider2D> newHitList;
        private TouchPadEventArgs tpea;
        private int pointerID;
        private Vector2 oldPosition;
        #endregion temp vars

        public static TouchPad Instance;

        #region regular
        void Awake()
        {
            IsActive = true;
            hitList = new List<Collider2D>();
            newHitList = new List<Collider2D>();
            tpea = new TouchPadEventArgs();

            if (Instance) Destroy(gameObject);
            else Instance = this;
        }
        #endregion regular

        #region raise events
        public void OnPointerDown(PointerEventData data)
        {
            if (IsActive)
            {
                if (!IsTouched)
                {
                    #if UNITY_EDITOR
                        if (dlog) Debug.Log("----------------POINTER Down--------------( " + data.pointerId);
                    #endif

                    IsTouched = true;
                    tpea = new TouchPadEventArgs();
                    ScreenTouchPos = data.position;
                    oldPosition = ScreenTouchPos;
                    pointerID = data.pointerId;

                    tpea.SetTouch(ScreenTouchPos, Vector2.zero, TouchPhase.Began);
                    hitList = new List<Collider2D>();
                    hitList.AddRange(tpea.hits);
                    if (hitList.Count > 0)
                    {
                        for (int i = 0; i < hitList.Count; i++)
                        {
                            ExecuteEvents.Execute<TouchPadMessageTarget>(hitList[i].gameObject, null, (x, y) => x.PointerDown(tpea));
                            if (tpea.firstSelected == null) tpea.firstSelected = hitList[i].GetComponent<TouchPadMessageTarget>();
                        }
                    }
                    ScreenPointerDownEvent?.Invoke(tpea);
                }
            }
            else
            {
                IsTouched = true;
            }
        }

        public void OnBeginDrag(PointerEventData data)
        {
            if (IsActive)
            {
                if (data.pointerId == pointerID)
                {
                    #if UNITY_EDITOR
                        if (dlog) Debug.Log("----------------BEGIN DRAG--------------( " + data.pointerId);
                    #endif

                    ScreenTouchPos = data.position;
                    tpea.SetTouch(ScreenTouchPos, ScreenTouchPos - oldPosition, TouchPhase.Moved);
                    oldPosition = ScreenTouchPos;
                    newHitList = new List<Collider2D>(tpea.hits); // garbage

                    //0 ---------------------------------- send drag begin message --------------------------------------------------
                    for (int i = 0; i < hitList.Count; i++)
                    {
                        if (hitList[i]) ExecuteEvents.Execute<TouchPadMessageTarget>(hitList[i].transform.gameObject, null, (x, y) => x.DragBegin(tpea));
                    }
                    ScreenDragEvent?.Invoke(tpea);
                }
                hitList = newHitList;
            }
        }

        public void OnDrag(PointerEventData data)
        {
            if (IsActive)
            {
                if (data.pointerId == pointerID)
                {
                    #if UNITY_EDITOR
                        if (dlog) Debug.Log("---------------- ONDRAG --------------( " + data.pointerId + " : " + pointerID);
                    #endif

                    ScreenTouchPos = data.position;
                    tpea.SetTouch(ScreenTouchPos, ScreenTouchPos - oldPosition, TouchPhase.Moved);
                    oldPosition = ScreenTouchPos;
                    newHitList = new List<Collider2D>(tpea.hits); // garbage

                    //1 ------------------ send drag exit message and drag message --------------------------------------------------
                    foreach (Collider2D cHit in hitList)
                    {
                        if (newHitList.IndexOf(cHit) == -1)
                        {
                            if (cHit) ExecuteEvents.Execute<TouchPadMessageTarget>(cHit.gameObject, null, (x, y) => x.DragExit(tpea));
                        }
                        else
                        {
                            if (cHit) ExecuteEvents.Execute<TouchPadMessageTarget>(cHit.gameObject, null, (x, y) => x.Drag(tpea));
                        }

                    }

                    //2 ------------------ send drag enter message -----------------------------------------------------------------
                    for (int i = 0; i < newHitList.Count; i++)
                    {
                        if (hitList.IndexOf(newHitList[i]) == -1)
                        {
                            if (newHitList[i]) ExecuteEvents.Execute<TouchPadMessageTarget>(newHitList[i].gameObject, null, (x, y) => x.DragEnter(tpea));
                        }
                    }

                    hitList = newHitList;
                    ScreenDragEvent?.Invoke(tpea);
                }
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            IsTouched = false;

            if (IsActive)
            {
                if (data.pointerId == pointerID)
                {
                    #if UNITY_EDITOR
                        if (dlog) Debug.Log("----------------POINTER UP--------------( " + data.pointerId + " : " + pointerID);
                    #endif

                    ScreenTouchPos = data.position;
                    tpea.SetTouch(ScreenTouchPos, ScreenTouchPos - oldPosition, TouchPhase.Ended);
                    oldPosition = ScreenTouchPos;

                    foreach (Collider2D cHit in hitList)
                    {
                        if (cHit) ExecuteEvents.Execute<TouchPadMessageTarget>(cHit.gameObject, null, (x, y) => x.PointerUp(tpea));
                    }
                   
                    newHitList = new List<Collider2D>(tpea.hits);
                    foreach (Collider2D cHit in newHitList)
                    {
                        if (cHit && hitList.IndexOf(cHit)==-1) ExecuteEvents.Execute<TouchPadMessageTarget>(cHit.gameObject, null, (x, y) => x.PointerUp(tpea));
                        if (cHit) ExecuteEvents.Execute<TouchPadMessageTarget>(cHit.gameObject, null, (x, y) => x.DragDrop(tpea));
                    }
                    hitList = new List<Collider2D>();
                    newHitList = new List<Collider2D>();
                    ScreenPointerUpEvent?.Invoke(tpea);
                }
            }
        }

        public void OnPointerExit(PointerEventData data)
        {
            IsTouched = false;
            if (IsActive)
            {
                if (data.pointerId == pointerID)
                {
                    #if UNITY_EDITOR
                        if (dlog) Debug.Log("----------------POINTER EXIT--------------( " + data.pointerId + " : " + pointerID);
                    #endif

                    ScreenTouchPos = data.position;
                    tpea.SetTouch(ScreenTouchPos, ScreenTouchPos - oldPosition, TouchPhase.Ended);
                    oldPosition = ScreenTouchPos;

                    foreach (Collider2D cHit in hitList)
                    {
                        if (cHit) ExecuteEvents.Execute<TouchPadMessageTarget>(cHit.gameObject, null, (x, y) => x.PointerUp(tpea));
                    }

                    newHitList = new List<Collider2D>(tpea.hits);
                    foreach (Collider2D cHit in newHitList)
                    {
                        if (cHit && hitList.IndexOf(cHit) == -1) ExecuteEvents.Execute<TouchPadMessageTarget>(cHit.gameObject, null, (x, y) => x.PointerUp(tpea));
                        if (cHit) ExecuteEvents.Execute<TouchPadMessageTarget>(cHit.gameObject, null, (x, y) => x.DragDrop(tpea));
                    }
                    hitList = new List<Collider2D>();
                    newHitList = new List<Collider2D>();
                }
            }
        }

        public void OnDrop(PointerEventData data)
        {
            if (IsActive)
            {
                if (data.pointerId == pointerID)
                {
                    #if UNITY_EDITOR
                        if (dlog) Debug.Log("----------------ONDROP--------------( " + data.pointerId + " : " + pointerID);
                    #endif
                }
            }

        }
        #endregion raise events

        /// <summary>
        /// Return world position of touch.
        /// </summary>
        public Vector3 GetWorldTouchPos()
        {
            return Camera.main.ScreenToWorldPoint(ScreenTouchPos);
        }

        /// <summary>
        /// Enable or disable touch pad callbacks handling.
        /// </summary>
        public void SetTouchActivity(bool activity)
        {
            IsActive = activity;
#if UNITY_EDITOR
            if (dlog) Debug.Log("touch activity: " + activity);
#endif
        }

#if useinterface
        /// <summary>
        /// Returns all monobehaviours (casted to T)
        /// </summary>
        /// <typeparam name="T">interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        private T[] GetInterfaces<T>(GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            var mObjs = MonoBehaviour.FindObjectsOfType<MonoBehaviour>();
            return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
        }

        /// <summary>
        /// Returns the first monobehaviour that is of the interface type (casted to T)
        /// </summary>
        /// <typeparam name="T">Interface type</typeparam>
        /// <param name="gObj"></param>
        /// <returns></returns>
        private T GetInterface<T>(GameObject gObj)
        {
            if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
            return GetInterfaces<T>(gObj).FirstOrDefault();
        }
#endif
    }

#if useinterface
    /// <summary>
    /// Interface for handling touchpad events.
    /// </summary>
    public interface ICustomMessageTarget : IEventSystemHandler
    {
        void PointerDown(TouchPadEventArgs tpea);
        void DragBegin(TouchPadEventArgs tpea);
        void DragEnter(TouchPadEventArgs tpea);
        void DragExit(TouchPadEventArgs tpea);
        void DragDrop(TouchPadEventArgs tpea);
        void PointerUp(TouchPadEventArgs tpea);
        void Drag(TouchPadEventArgs tpea);
        GameObject GetDataIcon();
        GameObject GetGameObject();
    }
#endif
    public class TouchPadMessageTarget : MonoBehaviour, IEventSystemHandler //, ICustomMessageTarget
    {
        public Action<TouchPadEventArgs> PointerDownEvent;
        public Action<TouchPadEventArgs> DragBeginEvent;
        public Action<TouchPadEventArgs> DragEnterEvent;
        public Action<TouchPadEventArgs> DragExitEvent;
        public Action<TouchPadEventArgs> DragDropEvent;
        public Action<TouchPadEventArgs> PointerUpEvent;
        public Action<TouchPadEventArgs> DragEvent;

        GameObject dataIcon;

        public void PointerDown(TouchPadEventArgs tpea)
        {
            PointerDownEvent?.Invoke(tpea);
        }

        public void DragBegin(TouchPadEventArgs tpea)
        {
            DragBeginEvent?.Invoke(tpea);
        }

        public void DragEnter(TouchPadEventArgs tpea)
        {
            DragEnterEvent?.Invoke(tpea);
        }

        public void DragExit(TouchPadEventArgs tpea)
        {
            DragExitEvent?.Invoke(tpea);
        }

        public void DragDrop(TouchPadEventArgs tpea)
        {
           DragDropEvent?.Invoke(tpea);
        }

        public void PointerUp(TouchPadEventArgs tpea)
        {
            PointerUpEvent?.Invoke(tpea);
        }

        public void Drag(TouchPadEventArgs tpea)
        {
            DragEvent?.Invoke(tpea);
        }

        public GameObject GetDataIcon()
        {
            return dataIcon;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }

    [Serializable]
    public class TouchPadEventArgs
    {
        /// <summary>
        /// First selected object.
        /// </summary>
        public TouchPadMessageTarget firstSelected;
        /// <summary>
        /// The cast results.
        /// </summary>
        public Collider2D[] hits;
        /// <summary>
        /// Priority dragging direction.  (0,1) or (1,0)
        /// </summary>
        public Vector2 PriorAxe
        {
            get { return priorityAxe; }
        }
        /// <summary>
        /// Touch delta position in screen coordinats;
        /// </summary>
        public Vector2 DragDirection
        {
            get { return touchDeltaPosRaw; }
        }
        /// <summary>
        /// Last drag direction.
        /// </summary>
        public Vector2 LastDragDirection
        {
            get { return lastDragDir; }
        }
        /// <summary>
        /// Return touch world position.
        /// </summary>
        public Vector3 WorldPos
        {
            get { return wPos; }
        }

        private Vector2 touchDeltaPosRaw;
        private Vector2 priorityAxe;
        private Vector2 lastDragDir;
        private Vector3 wPos;
        private Vector2 touchPos;

        /// <summary>
        /// Fill touch arguments from touch object;
        /// </summary>
        public void SetTouch(Touch touch)
        {
            touchPos = touch.position;
            wPos = Camera.main.ScreenToWorldPoint(touchPos);
            hits = Physics2D.OverlapPointAll(new Vector2(wPos.x, wPos.y));
            touchDeltaPosRaw = touch.deltaPosition;

            if (touch.phase == TouchPhase.Moved)
            {
                lastDragDir = touchDeltaPosRaw;
                priorityAxe = GetPriorityOneDirAbs(touchDeltaPosRaw);
            }
        }

        /// <summary>
        /// Fill touch arguments.
        /// </summary>
        public void SetTouch(Vector2 position, Vector2 deltaPosition, TouchPhase touchPhase)
        {
            touchPos = position;
            wPos = Camera.main.ScreenToWorldPoint(touchPos);
            hits = Physics2D.OverlapPointAll(new Vector2(wPos.x, wPos.y));
            touchDeltaPosRaw = deltaPosition;

            if (touchPhase == TouchPhase.Moved)
            {
                lastDragDir = touchDeltaPosRaw;
                priorityAxe = GetPriorityOneDirAbs(touchDeltaPosRaw);
            }
        }

        /// <summary>
        /// Return drag icon for firs touched elment or null.
        /// </summary>
        public GameObject GetIconDrag()
        {
            if (firstSelected != null)
            {
                GameObject icon = firstSelected.GetDataIcon();
                return icon;
            }
            else
            {
                return null;
            }

        }

        private Vector2 GetPriorityOneDirAbs(Vector2 sourceDir)
        {

            if (Mathf.Abs(sourceDir.x) > Mathf.Abs(sourceDir.y))
            {
                float x = (sourceDir.x > 0) ? 1 : 1;
                return new Vector2(x, 0f);
            }
            else
            {
                float y = (sourceDir.y > 0) ? 1 : 1;
                return new Vector2(0f, y);
            }
        }
    }
}
