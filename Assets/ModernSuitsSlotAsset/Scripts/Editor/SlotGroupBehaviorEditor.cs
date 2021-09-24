using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Mkey
{
    [CustomEditor(typeof(SlotGroupBehavior))]
    public class SlotGroupBehaviorEditor : Editor
    {
        SlotGroupBehavior slotGroupBehavior;
        private ReorderableList symbOrderList;
        string[] iconChoises;
        string[] orderChoises;
        List<int> symbOrder;
        SlotController slotController;
        private void OnEnable()
        {
            symbOrderList = new ReorderableList(serializedObject, serializedObject.FindProperty("symbOrder"),
                 true, true, true, true);

            symbOrderList.onRemoveCallback += RemoveCallback;
            symbOrderList.drawElementCallback += OnDrawCallback;
            symbOrderList.onAddCallback += OnAddCallBack;
            symbOrderList.onSelectCallback += OnSelectCallBack;
            symbOrderList.drawHeaderCallback += DrawHeaderCallBack;
            symbOrderList.onChangedCallback += OnChangeCallBack;
            //  symbOrderList.onAddDropdownCallback += OnAddDropDownCallBack;
        }

        private void OnDisable()
        {
            if (symbOrderList != null)
            {
                symbOrderList.onRemoveCallback -= RemoveCallback;
                symbOrderList.drawElementCallback -= OnDrawCallback;
                symbOrderList.onAddCallback -= OnAddCallBack;
                symbOrderList.onSelectCallback -= OnSelectCallBack;
                symbOrderList.drawHeaderCallback -= DrawHeaderCallBack;
                symbOrderList.onChangedCallback -= OnChangeCallBack;
                symbOrderList.onAddDropdownCallback -= OnAddDropDownCallBack;
            }
        }

        bool showSymbOrder = true;
        bool showSimul = true;
        bool showDefault;
        public override void OnInspectorGUI()
        {
            if (!slotController)
            {
                slotController = GameObject.Find("SlotController").GetComponent<SlotController>();
            }

            if (!slotController) return;
            slotGroupBehavior = (SlotGroupBehavior)target;

            symbOrder = slotGroupBehavior.symbOrder;
            iconChoises = slotController.GetIconNames(false);
            serializedObject.Update();

            #region symbOrder
            ShowReordListBoxFoldOut("Symbol Order", symbOrderList, ref showSymbOrder);
            serializedObject.ApplyModifiedProperties();
            #endregion symbOrder

            ShowPropertiesBox(new string[] { "rayCasters",
              "addRotateTime",  "spinStartDelay", "spinStartRandomize", "spinSpeedMultiplier", "randomStartPosition", "tileSizeY","gapY" ,"baseLink"}, true);

            serializedObject.ApplyModifiedProperties();

            #region simulate
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
  
            if (showSimul = EditorGUILayout.Foldout(showSimul, "Base line simulation"))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                ShowProperties(new string[] { "simulate" }, false);
                orderChoises = GetOrderNames();
                ShowOrderChoiseLO(orderChoises);
                EditorGUILayout.EndHorizontal();
            }
 
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
            #endregion simulate
            serializedObject.ApplyModifiedProperties();

            #region default
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            if (showDefault = EditorGUILayout.Foldout(showDefault, "Default Inspector"))
            {
                DrawDefaultInspector();
                if (slotGroupBehavior.CopyFrom != null && slotGroupBehavior.CopyFrom.symbOrder != null && slotGroupBehavior.CopyFrom.symbOrder.Count > 0)
                    if (GUILayout.Button("Copy data from reel"))
                    {
                        Debug.Log("copied");
                        slotGroupBehavior.symbOrder = new List<int>(slotGroupBehavior.CopyFrom.symbOrder);
                        serializedObject.ApplyModifiedProperties();
                    }
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
            #endregion default
        }

        #region reordList CallBacks
        private void OnAddDropDownCallBack(Rect buttonRect, ReorderableList list)
        {
        }

        private void OnChangeCallBack(ReorderableList list)
        {
           // Debug.Log("onchange");
        }

        private void DrawHeaderCallBack(Rect rect)
        {
            EditorGUI.LabelField(rect, "Symbols on reel ordering");
        }

        private void OnSelectCallBack(ReorderableList list)
        {
        }

        private void OnAddCallBack(ReorderableList list)
        {
            if (slotGroupBehavior == null || slotGroupBehavior.symbOrder == null) return;
            if (slotGroupBehavior.symbOrder != null && slotGroupBehavior.symbOrder.Count > 0)
            {
                slotGroupBehavior.symbOrder.Add(slotGroupBehavior.symbOrder[slotGroupBehavior.symbOrder.Count-1]);
            }
            else
                slotGroupBehavior.symbOrder.Add(0);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
           // Debug.Log("OnAddCallBack");
        }

        private void OnDrawCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.LabelField(rect, (index + 1).ToString());
            var element = symbOrderList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            rect.x += 20;
            ShowChoise( iconChoises, rect, 80, 20, 0, 0, index);
        }

        private void RemoveCallback(ReorderableList list)
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure?", "Yes", "No"))
            {
                slotGroupBehavior.symbOrder.RemoveAt(list.index); //ReorderableList.defaultBehaviours.DoRemoveButton(list);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }
        #endregion reordList  CallBacks

        #region showProperties
        private void ShowProperties(string [] properties, bool showHierarchy)
        {
            for (int i = 0; i <properties.Length; i++)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(properties[i]), showHierarchy);
            }
        }

        private void ShowPropertiesBox(string[] properties, bool showHierarchy)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            ShowProperties(properties, showHierarchy);
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }

        private void ShowPropertiesBoxFoldOut(string bName,string[] properties, ref bool fOut, bool showHierarchy)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            if (fOut = EditorGUILayout.Foldout(fOut, bName))
            {
                ShowProperties(properties, showHierarchy);
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }

        private void ShowReordListBoxFoldOut(string bName, ReorderableList rList, ref bool fOut)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            if (fOut = EditorGUILayout.Foldout(fOut, bName))
            {
                rList.DoLayoutList();
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }
        #endregion showProperties

        #region array
        public static void ShowList(SerializedProperty list, bool showListSize = true, bool showListLabel = true)
        {
            if (showListLabel)
            {
                EditorGUILayout.PropertyField(list);
                EditorGUI.indentLevel += 1;
            }
            if (!showListLabel || list.isExpanded)
            {
                if (showListSize)
                {
                    EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"));
                }
                for (int i = 0; i < list.arraySize; i++)
                {
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
                }
            }
            if (showListLabel)
            {
                EditorGUI.indentLevel -= 1;
            }
        }
        #endregion array

        #region showChoise EditorGuiLayOut
        private void ShowChoiseLO(string [] choise)
        {
            int _choiceIndex = 0;
            if (choise == null || choise.Length==0) return;
            _choiceIndex = EditorGUILayout.Popup(_choiceIndex, choise);
            Debug.Log("choice: " + _choiceIndex);
            EditorUtility.SetDirty(target);
        }

        private void ShowOrderChoiseLO(string[] ordChoises)
        {
            if (ordChoises == null || ordChoises.Length == 0) return;
            int _choiceIndex = slotGroupBehavior.simPos;
            _choiceIndex = EditorGUILayout.Popup(_choiceIndex, ordChoises);
            slotGroupBehavior.simPos = _choiceIndex;
            EditorUtility.SetDirty(target);
        }

        private void ShowSlotSymbolChoiseLO()
        {
            if (slotController == null) return;
            ShowChoiseLO(slotController.GetIconNames(true));
        }

        private void ShowChoiseLineLO(int count)
        {
            EditorGUILayout.BeginHorizontal("box");
            for (int i = 0; i < count; i++)
            {
                ShowSlotSymbolChoiseLO();
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion showChoise EditorGuiLayOut

        #region showChoise EditorGui
        private void ShowChoise(string[] choises, Rect rect, float width, float height, float dx, float dy,  int index)
        {
            if (choises == null || choises.Length == 0 ) return;
            int choiseIndex = symbOrder[index];
            int oldIndex = choiseIndex;
            choiseIndex = EditorGUI.Popup(new
                Rect(rect.x + dx, rect.y+dy, width, height),
                choiseIndex, choises);
           symbOrder[index] = choiseIndex;
            if (oldIndex != choiseIndex) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
        #endregion showChoise EditorGui

        ReorderableList CreateList(SerializedObject obj, SerializedProperty prop) // https://pastebin.com/WhfRgcdC
        {
            ReorderableList list = new ReorderableList(obj, prop, true, true, true, true);

            list.drawHeaderCallback = rect => {
                EditorGUI.LabelField(rect, "Sprites");
            };

            List<float> heights = new List<float>(prop.arraySize);

            list.drawElementCallback = (rect, index, active, focused) => {
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                Sprite s = (element.objectReferenceValue as Sprite);

                bool foldout = active;
                float height = EditorGUIUtility.singleLineHeight * 1.25f;
                if (foldout)
                {
                    height = EditorGUIUtility.singleLineHeight * 5;
                }

                try
                {
                    heights[index] = height;
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogWarning(e.Message);
                }
                finally
                {
                    float[] floats = heights.ToArray();
                    Array.Resize(ref floats, prop.arraySize);
                    heights = new List<float> (floats);
                }

                float margin = height / 10;
                rect.y += margin;
                rect.height = (height / 5) * 4;
                rect.width = rect.width / 2 - margin / 2;

                if (foldout)
                {
                    if (s)
                    {
                        EditorGUI.DrawPreviewTexture(rect, s.texture);
                    }
                }
                rect.x += rect.width + margin;
                EditorGUI.ObjectField(rect, element, GUIContent.none);
            };

            list.elementHeightCallback = (index) => {
                Repaint();
                float height = 0;

                try
                {
                    height = heights[index];
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.LogWarning(e.Message);
                }
                finally
                {
                    float[] floats = heights.ToArray();
                    Array.Resize(ref floats, prop.arraySize);
                    heights = new List<float>(floats);
                }

                return height;
            };

            list.drawElementBackgroundCallback = (rect, index, active, focused) => {
                rect.height = heights[index];
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                tex.Apply();
                if (active)
                    GUI.DrawTexture(rect, tex as Texture);
            };

            list.onAddDropdownCallback = (rect, li) => {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Add Element"), false, () => {
                    serializedObject.Update();
                    li.serializedProperty.arraySize++;
                    serializedObject.ApplyModifiedProperties();
                });

                menu.ShowAsContext();

                float[] floats = heights.ToArray();
                Array.Resize(ref floats, prop.arraySize);
                heights = new List<float>(floats);
            };

            return list;
        }

        public string[] GetOrderNames()
        {
            string[] res = new string[symbOrder.Count];

            for (int i = 0; i < symbOrder.Count; i++)
            {
                res[i] = (i + 1) + " - " + (symbOrder[i] < iconChoises.Length ? iconChoises[symbOrder[i]] : "failed");
            }
            return res;
        }

    }
}

