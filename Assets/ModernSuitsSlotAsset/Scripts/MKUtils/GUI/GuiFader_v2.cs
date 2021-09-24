using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;
#endif
/*
  changes
 27.03.18
    1) WinAnimptions
    21.12.18
            private void OnDestroy()
        {
            SimpleTween.Cancel(gameObject, false);
        }
    7.04.19
    1) c#6.0
    2) add ease

    16.04.19 (comment moveout)
             //   guiPanel.pivot = new Vector2(0.5f, 0.5f);
    22.04.19 (disable guimask)
        -   if (guiMask) guiMask.gameObject.SetActive(false);

    11.01.20 - avoid null error
        - if (fObjectR == null)  fObjectR = new FaderObjectsRec(guiPanel.gameObject);

    11.11.2020 +GuiFaderEditor
 */
namespace Mkey
{
    public class GuiFader_v2 : MonoBehaviour
    {
        public WindowOpions winOptions;

        public RectTransform backGround;
    
        public RectTransform guiPanel;
        public RectTransform guiMask;

        public UnityEvent completeCallback_in;
        public UnityEvent completeCallback_out;

        private bool initialised = false;
        private FaderObjectsRec fObjectR;

        private void OnDestroy()
        {
            SimpleTween.Cancel(gameObject, false);
        }

        public void FadeIn(float tweenDelay, Action completeCallBack)
        {
            if((!backGround) && (!guiPanel))
            {
                completeCallBack?.Invoke();
                return;
            }
            if (!initialised)
            {
                Initialize();
            }
            if (winOptions == null)
            {
                winOptions = new WindowOpions();
            }
            if (guiMask) guiMask.gameObject.SetActive(true);
            if(guiPanel) guiPanel.gameObject.SetActive(true);
            if (backGround) backGround.gameObject.SetActive(true);
            switch (winOptions.inAnim)
            {
                case WinAnimType.AlphaFade:
                    AlphaFadeIn(tweenDelay,winOptions.inFadeAnim.time, winOptions.inEase, completeCallBack);
                    break;
                case WinAnimType.Move:
                    MoveIn(tweenDelay,winOptions.inMoveAnim.time, winOptions.inEase, completeCallBack);
                    break;
                case WinAnimType.Scale:
                    ScaleIn(tweenDelay, winOptions.inScaleAnim.time, winOptions.inEase, completeCallBack);
                    break;
            }
        }

        public void FadeOut(float tweenDelay, Action completeCallBack)
        {
            if ((!backGround) && (!guiPanel))
            {
                completeCallBack?.Invoke();
                return;
            }

            if (!initialised)
            {
                Initialize();
            }

            if (winOptions == null)
            {
                winOptions = new WindowOpions();
            }

            switch (winOptions.outAnim)
            {
                case WinAnimType.AlphaFade:
                    AlphaFadeOut(tweenDelay, winOptions.outFadeAnim.time, winOptions.outEase, completeCallBack);
                    break;
                case WinAnimType.Move:
                    MoveOut(tweenDelay, winOptions.outMoveAnim.time, winOptions.outEase, completeCallBack);
                    break;
                case WinAnimType.Scale:
                    ScaleOut(tweenDelay, winOptions.outScaleAnim.time, winOptions.outEase, completeCallBack);
                    break;
            }
        }

        private void AlphaFadeIn(float tweenDelay, float tweenTime, EaseAnim ease, Action completeCallBack)
        {
            fObjectR.SetAlpha(0f);
            SimpleTween.Value(gameObject, 0f, 1.0f, tweenTime).SetEase(EaseAnim.EaseInCirc).SetOnUpdate((float val) => { fObjectR.SetAlphaK(val); }).
                SetDelay(tweenDelay).SetEase(ease).
                AddCompleteCallBack(() =>
                {
                     completeCallBack?.Invoke();
                    completeCallback_in?.Invoke();
                });
        }

        private void AlphaFadeOut(float tweenDelay, float tweenTime, EaseAnim ease, Action completeCallBack)
        {
            fObjectR.SetActive(true);
            SimpleTween.Value(gameObject, 1.0f, 0.0f, tweenTime).SetEase(EaseAnim.EaseInCirc).SetOnUpdate((float val) => { fObjectR.SetAlphaK(val); }).
                SetDelay(tweenDelay).SetEase(ease).
                AddCompleteCallBack(() =>
                {
                    fObjectR.SetActive(false);
                    if (guiMask) guiMask.gameObject.SetActive(false);
                    completeCallBack?.Invoke();
                    completeCallback_out?.Invoke();
                });
        }

        Vector3 startPosition = Vector3.zero;
        private void MoveIn(float tweenDelay, float tweenTime, EaseAnim ease, Action completeCallBack)
        {
            if (!guiPanel)
            {
                completeCallBack?.Invoke();
                completeCallback_in?.Invoke();
                return;
            }
            RectTransform mainRT = GetComponent<RectTransform>();
            Vector3[] wC = new Vector3[4];
            mainRT.GetWorldCorners(wC);

            Vector3[] wC1 = new Vector3[4];
            guiPanel.GetWorldCorners(wC1);
            float height = (wC1[2] - wC1[0]).y;
            float width = (wC1[2] - wC1[0]).x;

           
            Vector3 pos = guiPanel.position;
            Vector3 posTo = pos;
            float fTime = winOptions.inMoveAnim.time;
            startPosition = pos;

            switch (winOptions.inMoveAnim.toPosition)
            {
                case Position.LeftMiddleOut:
                    posTo = new Vector3(wC[0].x - width / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;
                case Position.RightMiddleOut:
                    posTo = new Vector3(wC[2].x + width / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;
                case Position.MiddleBottomOut:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, wC[0].y - height / 2f, pos.z);
                    break;
                case Position.MiddleTopOut:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, wC[2].y + height / 2f, pos.z);
                    break;
                case Position.LeftMiddleIn:
                    posTo = new Vector3(wC[0].x + width / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;
                case Position.RightMiddleIn:
                    posTo = new Vector3(wC[2].x - width / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;
                case Position.MiddleBottomIn:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, wC[0].y + height / 2f, pos.z);
                    break;
                case Position.MiddleTopIn:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, wC[2].y - height / 2f, pos.z);
                    break;
                case Position.CustomPosition:
                    posTo = winOptions.inMoveAnim.customPosition;
                    if (winOptions.inMoveAnim.useMask) posTo = guiMask.position;
                    break;
                case Position.AsIs:
                    break;
                case Position.Center:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;

            }
            SimpleTween.Value(gameObject, pos, posTo, fTime).SetOnUpdate((val) =>
            {
                if (guiPanel)
                {
                    guiPanel.position = val;
                   // Debug.Log("movein guiPanel.position :" + guiPanel.position);
                }
            }).SetDelay(tweenDelay).SetEase(ease).
            AddCompleteCallBack(() =>
            {
              //  Debug.Log("movein full --------------------------------");
                completeCallBack?.Invoke();
                completeCallback_in?.Invoke();
            });
        }

        private void MoveOut(float tweenDelay, float tweenTime, EaseAnim ease, Action completeCallBack)
        {
            if (!guiPanel)
            {
                completeCallBack?.Invoke();
                completeCallback_out?.Invoke();
                return;
            }
            RectTransform mainRT = GetComponent<RectTransform>();
            Vector3[] wC = new Vector3[4];
            mainRT.GetWorldCorners(wC);

            Vector3[] wC1 = new Vector3[4];
            guiPanel.GetWorldCorners(wC1);

            float height = (wC1[2] - wC1[0]).y;
            float width = (wC1[2] - wC1[0]).x;

            Vector3 pos = guiPanel.position;
            Vector3 posTo = pos;
            float fTime = winOptions.outMoveAnim.time;
         //   guiPanel.pivot = new Vector2(0.5f, 0.5f);

            switch (winOptions.outMoveAnim.toPosition)
            {
                case Position.LeftMiddleOut:
                    posTo = new Vector3(wC[0].x - width / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;
                case Position.RightMiddleOut:
                    posTo = new Vector3(wC[2].x + width / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;
                case Position.MiddleBottomOut:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, wC[0].y - height / 2f, pos.z);
                    break;
                case Position.MiddleTopOut:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, wC[2].y + height / 2f, pos.z);
                    break;
                case Position.LeftMiddleIn:
                    posTo = new Vector3(wC[0].x + width / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;
                case Position.RightMiddleIn:
                    posTo = new Vector3(wC[2].x - width / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;
                case Position.MiddleBottomIn:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, wC[0].y + height / 2f, pos.z);
                    break;
                case Position.MiddleTopIn:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, wC[2].y - height / 2f, pos.z);
                    break;
                case Position.CustomPosition:
                    posTo = winOptions.inMoveAnim.customPosition;
                    break;
                case Position.AsIs:
                    posTo = startPosition;
                    break;
                case Position.Center:
                    posTo = new Vector3((wC[0].x + wC[2].x) / 2f, (wC[0].y + wC[2].y) / 2f, pos.z);
                    break;

            }
            SimpleTween.Value(gameObject, pos, posTo, fTime).SetOnUpdate((val) =>
            {
                if (guiPanel)
                {
                    guiPanel.position = val;
                  //  Debug.Log("moveout guiPanel.position :" + guiPanel.position);
                }
            }).SetDelay(tweenDelay).SetEase(ease).
            AddCompleteCallBack(() =>
            {
                if (guiPanel)  guiPanel.gameObject.SetActive(false);
                if (backGround) backGround.gameObject.SetActive(false);
                if (guiMask) guiMask.gameObject.SetActive(false);
                completeCallBack?.Invoke();
                completeCallback_out?.Invoke();
            });
        }

        private void ScaleIn(float tweenDelay, float tweenTime, EaseAnim ease, Action completeCallBack)
        {
            if (!guiPanel)
            {
                completeCallBack?.Invoke();
                completeCallback_in?.Invoke();
                return;
            }

            RectTransform mainRT = GetComponent<RectTransform>();
            Vector3[] wC = new Vector3[4];
            mainRT.GetWorldCorners(wC);

            Vector3[] wC1 = new Vector3[4];
            guiPanel.GetWorldCorners(wC1);

            float height = (wC1[2] - wC1[0]).y;
            float width = (wC1[2] - wC1[0]).x;

            float fTime = winOptions.inScaleAnim.time;

            Vector3 scaleTo = guiPanel.localScale;
            Vector3 scale = scaleTo;

            switch (winOptions.inScaleAnim.scaleType)
            {
                case ScaleType.CenterXY:
                    scale = new Vector3(0,0,0);
                    break;
                case ScaleType.CenterX:
                    scale = new Vector3(scaleTo.x, 0, 0);
                    break;
                case ScaleType.CenterY:
                    scale = new Vector3(0, scaleTo.y, 0);
                    break;
                case ScaleType.Top:
                    guiPanel.position = guiPanel.position + new Vector3(0, height / 2f, 0);
                    scale = new Vector3(scaleTo.x, 0, 0);
                    break;
                case ScaleType.Bottom:
                    guiPanel.position = guiPanel.position - new Vector3(0, height / 2f, 0);
                    scale = new Vector3(scaleTo.x, 0, 0);
                    break;
                case ScaleType.Left:
                    guiPanel.position = guiPanel.position - new Vector3(width / 2f, 0,  0);
                    scale = new Vector3(0, scaleTo.y, 0);
                    break;
                case ScaleType.Right:
                    guiPanel.position = guiPanel.position + new Vector3(width / 2f, 0, 0);
                    scale = new Vector3(0, scaleTo.y, 0);
                    break;
            }

            float posY = guiPanel.position.y;
            float posX = guiPanel.position.x;
            float posZ = guiPanel.position.z;
            SimpleTween.Value(gameObject, scale, scaleTo, fTime).SetOnUpdate((val) =>
            {
                guiPanel.localScale = val;
                if(winOptions.inScaleAnim.scaleType == ScaleType.Top)
                {
                    if (guiPanel) guiPanel.position = new Vector3(posX, posY - height/2.0f * val.y, guiPanel.position.z);
                }
                else if(winOptions.inScaleAnim.scaleType == ScaleType.Bottom)
                {
                    if (guiPanel) guiPanel.position = new Vector3(posX, posY + height / 2.0f * val.y, posZ);
                }
                else if (winOptions.inScaleAnim.scaleType == ScaleType.Left)
                {
                    if (guiPanel) guiPanel.position = new Vector3(posX + width / 2.0f * val.x, posY, posZ);
                }
                else if (winOptions.inScaleAnim.scaleType == ScaleType.Right)
                {
                    if (guiPanel) guiPanel.position = new Vector3(posX - width / 2.0f * val.x, posY , posZ);
                }

            }).SetDelay(tweenDelay).SetEase(ease).
            AddCompleteCallBack(() =>
            {
                completeCallBack?.Invoke();
                completeCallback_in?.Invoke();
            });
        }

        private void ScaleOut(float tweenDelay, float tweenTime, EaseAnim ease, Action completeCallBack)
        {
            if (!guiPanel)
            {
                completeCallBack?.Invoke();
                completeCallback_in?.Invoke();
                return;
            }

            RectTransform mainRT = GetComponent<RectTransform>();
            Vector3[] vC = new Vector3[4];
            mainRT.GetWorldCorners(vC);

            Vector3[] vC1 = new Vector3[4];
            guiPanel.GetWorldCorners(vC1);
            float height = (vC1[2] - vC1[0]).y;
            float width = (vC1[2] - vC1[0]).x;

            Vector3 pos = guiPanel.position;
            Vector3 pos1 = pos;
            float fTime = winOptions.outScaleAnim.time;

            Vector3 locScale = guiPanel.localScale;
            Vector3 startScale = guiPanel.localScale;

            switch (winOptions.outScaleAnim.scaleType)
            {
                case ScaleType.CenterXY:
                    locScale = new Vector3(0, 0, 0);
                    break;
                case ScaleType.CenterX:
                    locScale = new Vector3(locScale.x, 0, 0);
                    break;
                case ScaleType.CenterY:
                    locScale = new Vector3(0, locScale.y, 0);
                    break;
                case ScaleType.Top:
                    locScale = new Vector3(locScale.x, 0, 0);
                    break;
                case ScaleType.Bottom:
                    locScale = new Vector3(locScale.x, 0, 0);
                    break;
                case ScaleType.Left:
                    locScale = new Vector3(0, locScale.y, 0);
                    break;
                case ScaleType.Right:
                    locScale = new Vector3(0, locScale.y, 0);
                    break;
            }
            float posY = guiPanel.position.y;
            float posX = guiPanel.position.x;
            float posZ = guiPanel.position.z;

            SimpleTween.Value(gameObject, startScale, locScale, fTime).SetOnUpdate((val) =>
            {
                if (winOptions.outScaleAnim.scaleType == ScaleType.Top)
                {
                    if (guiPanel) guiPanel.position = new Vector3(posX, posY + height / 2.0f *(startScale.y - val.y), pos.z);
                }
                else if (winOptions.outScaleAnim.scaleType == ScaleType.Bottom)
                {
                    if (guiPanel) guiPanel.position = new Vector3(posX, posY - height / 2.0f * (startScale.y - val.y), pos.z);
                }
                else if (winOptions.outScaleAnim.scaleType == ScaleType.Left)
                {
                    if (guiPanel) guiPanel.position = new Vector3(posX - width / 2.0f * (startScale.x - val.x), posY, pos.z);
                }
                else if (winOptions.outScaleAnim.scaleType == ScaleType.Right)
                {
                    if (guiPanel) guiPanel.position = new Vector3(posX + width / 2.0f * (startScale.x - val.x), posY, pos.z);
                }
                if (guiPanel) guiPanel.localScale = val;
            }).SetDelay(tweenDelay).SetEase(ease).
            AddCompleteCallBack(() =>
            {
                if (guiPanel) guiPanel.gameObject.SetActive(false);
                if (backGround) backGround.gameObject.SetActive(false);
                if (guiMask) guiMask.gameObject.SetActive(false);
                completeCallBack?.Invoke();
                completeCallback_out?.Invoke();
            });
        }

        private void SetInitState()
        {
            if (initialised)
            {
                fObjectR.SetInitState();
            }
        }

        private void Initialize()
        {
            if (backGround) fObjectR = new FaderObjectsRec(backGround.gameObject);
            if (guiPanel)
            {
                if (fObjectR == null)
                    fObjectR = new FaderObjectsRec(guiPanel.gameObject);
                else
                    fObjectR.Add(new FaderObjectsRec(guiPanel.gameObject));
            }
            initialised = true;
        }
    }

    public class FaderObject
    {
        public Image image;
        public Text text;
        public GameObject gOb;

        private float initAlpha;
        private bool isInitActiv;

        private float currAlpha;

        public FaderObject(GameObject gO)
        {
            Image imageIn = gO.GetComponent<Image>();
            Text textIn = gO.GetComponent<Text>();

            isInitActiv = gO.activeSelf;
            gOb = gO;

            if (imageIn != null) { image = imageIn; initAlpha = GetAlpha(image); }
            if (textIn != null) { text = textIn; initAlpha = GetAlpha(text); }
            currAlpha = initAlpha;
        }

        private float GetAlpha(Image im)
        {
            Color c = im.color;
            return c.a;
        }

        private float GetAlpha(Text tx)
        {
            Color c = tx.color;
            return c.a;
        }



        public void SetInitState()
        {
            if (text != null)
            {
                Color c = text.color;
                c.a = initAlpha;
                text.color = c;
                text.gameObject.SetActive(isInitActiv);
            }



            if (image != null)
            {
                Color c = image.color;
                c.a = initAlpha;
                image.color = c;
                image.gameObject.SetActive(isInitActiv);
            }
            currAlpha = initAlpha;
        }

        public void SetAlpha(float alpha)
        {
            currAlpha = alpha;
            if (text != null)
            {
                Color c = text.color;
                c.a = currAlpha;
                text.color = c;
            }



            if (image != null)
            {
                Color c = image.color;
                c.a = currAlpha;
                image.color = c;
            }
        }

        public void SetAlphaK(float multiplier)
        {
            currAlpha = initAlpha * multiplier;
            SetAlpha(currAlpha);
        }

        public void SetActive(bool activity)
        {
            gOb.SetActive(activity);
        }

        public float GetCurrAlpha()
        {
            return currAlpha;
        }

    }

    public class FaderObjectsRec
    {
        List<FaderObject> fObjects;
        List<FaderObject> parents;

        public FaderObjectsRec(GameObject gObjectParent)
        {
            List<GameObject> gObjects = new List<GameObject>();
            parents = new List<FaderObject>();
            gObjects.Add(gObjectParent);
            parents.Add(new FaderObject(gObjectParent));
            fObjects = new List<FaderObject>();
            GetChilds(gObjectParent, ref gObjects);
            gObjects.ForEach((gO) => { fObjects.Add(new FaderObject(gO)); });
        }

        public void SetInitState()
        {
            fObjects.ForEach((fO) => { fO.SetInitState(); });
        }

        public void SetAlpha(float alpha)
        {
            fObjects.ForEach((fO) => { fO.SetAlpha(alpha); });
        }

        public void SetAlphaK(float multiplier)
        {
            fObjects.ForEach((fO) => { fO.SetAlphaK(multiplier); });
        }

        /// <summary>
        /// Set Active only parent objects
        /// </summary>
        public void SetActive(bool activity)
        {
            parents.ForEach((pFO) => { pFO.SetActive(activity); });
        }

        public void Add(FaderObjectsRec fOb)
        {
            fOb.fObjects.ForEach((ob) => { fObjects.Add(ob); });
            fOb.parents.ForEach((pOb) => { parents.Add(pOb); });
        }

        // GetInterface childs recursively
        private  void GetChilds(GameObject g, ref List<GameObject> gList)
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

#if UNITY_EDITOR
    [CustomEditor(typeof(GuiFader_v2))]
    public class PopupsControllerEditor : Editor
    {
        WindowOpions wo;
        GuiFader_v2 gF;
        SerializedProperty woSP;
        public override void OnInspectorGUI()
        {
            gF = (GuiFader_v2)target;
            wo = gF.winOptions;
            if (wo != null)
            {
                ShowPropertiesBox(new string[] { "backGround", "guiPanel", "guiMask" }, true);
                woSP = serializedObject.FindProperty("winOptions");
                if (woSP != null)
                {
                    BeginBox();

                    ShowProperties(new SerializedProperty[] {
                        woSP.FindPropertyRelative("instantiatePosition")
                    }, true);

                    if (wo.instantiatePosition == Position.CustomPosition)
                    {
                        EditorGUI.indentLevel += 1;
                        ShowProperties(new SerializedProperty[] {
                            woSP.FindPropertyRelative("position"),
                           // woSP.FindPropertyRelative("rectPosition")
                        }, true);
                        EditorGUI.indentLevel -= 1;
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();

                    ShowProperties(new SerializedProperty[] {
                        woSP.FindPropertyRelative("inAnim"), woSP.FindPropertyRelative("inEase")
                    }, true);

                    switch (wo.inAnim)
                    {
                        case WinAnimType.AlphaFade:
                            ShowProperties(new SerializedProperty[] {
                            woSP.FindPropertyRelative("inFadeAnim") }, true);
                            break;
                        case WinAnimType.Move:
                            if (wo.inMoveAnim.toPosition == Position.CustomPosition)
                            {
                                ShowProperties(new SerializedProperty[] {
                            woSP.FindPropertyRelative("inMoveAnim") }, true);
                            }
                            else
                            {
                                SerializedProperty inAnSP = woSP.FindPropertyRelative("inMoveAnim");
                                ShowProperties(new SerializedProperty[] {
                                    inAnSP.FindPropertyRelative("toPosition"),
                                    inAnSP.FindPropertyRelative("time"),
                                },

                            true);
                            }

                            break;
                        case WinAnimType.Scale:
                            ShowProperties(new SerializedProperty[] {
                            woSP.FindPropertyRelative("inScaleAnim") }, true);
                            break;
                    }
                    EndBox();

                    EditorGUILayout.Space();
                    BeginBox();
                    ShowProperties(new SerializedProperty[] {
                        woSP.FindPropertyRelative("outAnim"), woSP.FindPropertyRelative("outEase")
                    }, true);

                    switch (wo.outAnim)
                    {
                        case WinAnimType.AlphaFade:
                            ShowProperties(new SerializedProperty[] {
                            woSP.FindPropertyRelative("outFadeAnim") }, true);
                            break;

                        case WinAnimType.Move:
                            if (wo.outMoveAnim.toPosition == Position.CustomPosition)
                            {
                                ShowProperties(new SerializedProperty[] {
                                        woSP.FindPropertyRelative("outMoveAnim") }, true);
                            }
                            else
                            {
                                SerializedProperty outAnSP = woSP.FindPropertyRelative("outMoveAnim");
                                ShowProperties(new SerializedProperty[] {
                                    outAnSP.FindPropertyRelative("toPosition"),
                                    outAnSP.FindPropertyRelative("time"),
                                },
                                true);
                            }
                            break;
                        case WinAnimType.Scale:
                            ShowProperties(new SerializedProperty[] {
                            woSP.FindPropertyRelative("outScaleAnim") }, true);
                            break;
                    }
                    EndBox();
                }
            }
            // DrawDefaultInspector();

            serializedObject.ApplyModifiedProperties();
        }

        #region showProperties
        private void ShowProperties(string[] properties, bool showHierarchy)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (!string.IsNullOrEmpty(properties[i])) EditorGUILayout.PropertyField(serializedObject.FindProperty(properties[i]), showHierarchy);
            }
        }

        private void ShowProperties(SerializedProperty[] properties, bool showHierarchy)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i] != null) EditorGUILayout.PropertyField(properties[i], showHierarchy);
            }
        }

        private void BeginBox()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
        }

        private void EndBox()
        {
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }

        private void ShowPropertiesBox(string[] properties, bool showHierarchy)
        {
            BeginBox();
            ShowProperties(properties, showHierarchy);
            EndBox();
        }

        private void ShowPropertiesBox(SerializedProperty[] properties, bool showHierarchy)
        {
            BeginBox();
            ShowProperties(properties, showHierarchy);
            EndBox();
        }

        private void ShowPropertiesBoxFoldOut(string bName, string[] properties, ref bool fOut, bool showHierarchy)
        {
            BeginBox();
            if (fOut = EditorGUILayout.Foldout(fOut, bName))
            {
                ShowProperties(properties, showHierarchy);
            }
            EndBox();
        }

        private void ShowReordListBoxFoldOut(string bName, ReorderableList rList, ref bool fOut)
        {
            BeginBox();
            if (fOut = EditorGUILayout.Foldout(fOut, bName))
            {
                rList.DoLayoutList();
            }
            EndBox();
        }
        #endregion showProperties
    }
#endif
}