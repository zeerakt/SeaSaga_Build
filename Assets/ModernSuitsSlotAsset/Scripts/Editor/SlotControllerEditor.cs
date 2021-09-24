using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Mkey
{
    [CustomEditor(typeof(SlotController))]
    public class SlotControllerEditor : Editor
    {
        SlotController slotController;
        private ReorderableList payTableList;
        string[] choises;
        private void OnEnable()
        {
            payTableList = new ReorderableList(serializedObject, serializedObject.FindProperty("payTable"),
                 true, true, true, true);

            payTableList.onRemoveCallback += RemoveCallback;
            payTableList.drawElementCallback += OnDrawCallback;
            payTableList.onAddCallback += OnAddCallBack;
            payTableList.onSelectCallback += OnSelectCallBack;
            payTableList.drawHeaderCallback += DrawHeaderCallBack;
            payTableList.onChangedCallback += OnChangeCallBack;
            payTableList.elementHeightCallback += OnElementHeightCallback;
            //  payTableList.onAddDropdownCallback += OnAddDropDownCallBack;

            ptFocusedIndex = -1;
            ptActiveIndex = -1;
    }

        private void OnDisable()
        {
            if (payTableList != null)
            {
                payTableList.onRemoveCallback -= RemoveCallback;
                payTableList.drawElementCallback -= OnDrawCallback;
                payTableList.onAddCallback -= OnAddCallBack;
                payTableList.onSelectCallback -= OnSelectCallBack;
                payTableList.drawHeaderCallback -= DrawHeaderCallBack;
                payTableList.onChangedCallback -= OnChangeCallBack;
                payTableList.onAddDropdownCallback -= OnAddDropDownCallBack;
                payTableList.elementHeightCallback -= OnElementHeightCallback;
            }
        }

        bool showPrefabs;
        bool showPayTable;
        bool showMajor;
        bool showTweenTarg;
        bool showOptions;
        bool showRotOptions;
        bool showDefault;
        bool showRef;
        bool showScatter;
        bool showJackPot;
        bool showLevelProgress;
        public override void OnInspectorGUI()
        {
            slotController = (SlotController)target;
            choises = slotController.GetIconNames(true);
            serializedObject.Update();

            #region main reference
            ShowPropertiesBoxFoldOut("Main references: ", new string[] { "menuController", "controls" ,"winController" }, ref showRef, false);
            #endregion main reference

            #region icons
            ShowPropertiesBox(new string[] { "slotIcons", "winSymbolBehaviors" }, true);
            #endregion icons

            #region payTable
            ShowReordListBoxFoldOut("Pay Table", payTableList, ref showPayTable);
           // serializedObject.ApplyModifiedProperties();
            #endregion payTable

            #region special major
            ShowMajorChoise("Special Major Symbols", ref showMajor);
            #endregion spaecial major

            #region prefabs
            ShowPropertiesBoxFoldOut("Prefabs: ", new string[]{ "tilePrefab", "particlesStars", "BigWinPrefab" }, ref showPrefabs, false);
            #endregion prefabs

            #region slotGroups
            ShowPropertiesBox(new string[] { "slotGroupsBeh" }, true);
            #endregion slotGroups

            #region tweenTargets
            ShowPropertiesBoxFoldOut("Tween targets: ", new string[] { "bottomJumpTarget", "topJumpTarget" },ref showTweenTarg, true);
            #endregion tweenTargets

            #region spin options
            ShowPropertiesBoxFoldOut("Spin options: ", new string[] {
                "inRotType",
                "inRotTime",
                "inRotAngle",
                "outRotType",
                "outRotTime",
                "outRotAngle",
                "mainRotateType",
                "mainRotateTime",
                "mainRotateTimeRandomize"
            }, ref showRotOptions, false);
            #endregion spin options

            #region options
            ShowPropertiesBoxFoldOut("Options: ", new string[] {
                "RandomGenerator",
                "winLineFlashing",
                "winSymbolParticles",
                "useLineBetMultiplier",
                "useLineBetFreeSpinMultiplier",
                "debugPredictSymbols"
            }, ref showOptions, false);
            #endregion options

            #region jackpots
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            if (showJackPot = EditorGUILayout.Foldout(showJackPot, "Jackpots"))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useMiniJacPot"), false);
                if (slotController.useMiniJacPot)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("miniJackPotCount"), false);
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("useMaxiJacPot"), false);
                if (slotController.useMaxiJacPot)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxiJackPotCount"), false);
                }

                EditorGUILayout.PropertyField(serializedObject.FindProperty("useMegaJacPot"), false);
                if (slotController.useMegaJacPot)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("megaJackPotCount"), false);
                }

                if (slotController.useMiniJacPot || slotController.useMaxiJacPot || slotController.useMegaJacPot)
                {
                   // EditorGUILayout.PropertyField(serializedObject.FindProperty("jackPotIncType"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("jackPotIncValue"), false);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("jpController"), false);
                }
                ShowJPSymbChoiseLO();
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
            #endregion jackpots

            #region levelprogress
            ShowPropertiesBoxFoldOut("Level progress: ", new string[] {
                "useLineBetProgressMultiplier",
                "loseSpinLevelProgress",
                "winSpinLevelProgress",
            }, ref showLevelProgress, false);
            #endregion levelprogress

            #region calculate
            EditorGUILayout.BeginHorizontal("box");
            if (GUILayout.Button("Calculate"))
            {
                DataWindow.Init();
                float sum, sumPreeSpins;
                string[,] probTable = slotController.CreatePropabilityTable();
                string [,] payTable = slotController.CreatePayTable(out sum, out sumPreeSpins);
                DataWindow.SetData(probTable, payTable, sum, sumPreeSpins);
            }
            EditorGUILayout.EndHorizontal();
            #endregion calculate

            #region default
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            if (showDefault = EditorGUILayout.Foldout(showDefault, "Default Inspector"))
            {
                DrawDefaultInspector();
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
            #endregion default

            serializedObject.ApplyModifiedProperties();
        }

        #region payTableList CallBacks
        private void OnAddDropDownCallBack(Rect buttonRect, ReorderableList list)
        {
        }

        private void OnChangeCallBack(ReorderableList list)
        {
           // Debug.Log("onchange");
        }

        private void DrawHeaderCallBack(Rect rect)
        {
            EditorGUI.LabelField(rect, "Pay Table");
        }

        private void OnSelectCallBack(ReorderableList list)
        {
        }

        private void OnAddCallBack(ReorderableList list)
        {
            if (slotController == null || slotController.slotGroupsBeh == null || slotController.slotGroupsBeh.Length == 0) return;
            if (slotController.payTable != null && slotController.payTable.Count > 0)
            {
                slotController.payTable.Add(new PayLine(slotController.payTable[slotController.payTable.Count - 1]));
            }
            else
                slotController.payTable.Add(new PayLine());
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
           // Debug.Log("OnAddCallBack");
        }

        private int ptFocusedIndex = -1;
        private int ptActiveIndex = -1;
        private void OnDrawCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            EditorGUI.LabelField(rect, (index + 1).ToString());
            
            rect.y += 2;
            rect.x += 20;
            int count = (slotController && slotController.slotGroupsBeh != null && slotController.slotGroupsBeh.Length>0) ? slotController.slotGroupsBeh.Length : 5;
            ShowPayLine(choises, rect, count, 70, 20, index, isActive, isFocused, slotController.payTable[index]);
         //   Debug.Log("inex " + index + " ;active: " + isActive + " ; focused: " + isFocused);
            if (isFocused) ptFocusedIndex = index;
            if (isActive) ptActiveIndex = index;

        }

        private void RemoveCallback(ReorderableList list)
        {
            if (EditorUtility.DisplayDialog("Warning!", "Are you sure?", "Yes", "No"))
            {
                slotController.payTable.RemoveAt(list.index); //ReorderableList.defaultBehaviours.DoRemoveButton(list);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
        }

        private float OnElementHeightCallback(int index)
        {
            Repaint();
            float height = EditorGUIUtility.singleLineHeight; 
            var element = payTableList.serializedProperty.GetArrayElementAtIndex(index);
            if (element.FindPropertyRelative("showEvent").boolValue && index==ptActiveIndex)
            {
                UnityEvent ue = slotController.payTable[index].LineEvent;
                int length = (ue!=null) ? ue.GetPersistentEventCount() : 0;
                float emptyLength = (length > 0) ? 3.5f : 5.5f;
                height = EditorGUIUtility.singleLineHeight * emptyLength + length * EditorGUIUtility.singleLineHeight*2.5f;
               // height = 120;
            }
            return height;
        }
        #endregion payTableList  CallBacks

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
        private void ShowMajorChoise(string bName, ref bool fOut)
        {
            string[] sChoise = slotController.GetIconNames(false);
            if (sChoise == null || sChoise.Length == 0) return;
            if (slotController == null) return;

            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            if (fOut = EditorGUILayout.Foldout(fOut, bName))
            {
               // ShowBonusChoiseLO(sChoise);
              //  ShowFreeSpinChoiseLO(sChoise);
                ShowWildChoiseLO(sChoise);
                ShowScatterChoiseLO(sChoise);
                if (slotController.useScatter)
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("scatterPayTable"), true);
                //  ShowHeartChoiseLO(sChoise);
                //  ShowDiamondChoiseLO(sChoise);
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }

        private void ShowWildChoiseLO(string[] sChoise)
        {
            EditorGUILayout.BeginHorizontal();
            ShowProperties(new string[] { "useWild" }, false);
            if (slotController.useWild)
            {
                //  EditorGUILayout.LabelField("Select Wild ");
                int choiseIndex = slotController.wild_id;
                int oldIndex = choiseIndex;
                choiseIndex = EditorGUILayout.Popup(choiseIndex, sChoise);
                slotController.wild_id = choiseIndex;
                if (oldIndex != choiseIndex) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowScatterChoiseLO(string[] sChoise)
        {
            EditorGUILayout.BeginHorizontal();
            ShowProperties(new string[] { "useScatter" }, false);
            if (slotController.useScatter)
            {
                // EditorGUILayout.LabelField("Select Scatter ");
                int choiseIndex = slotController.scatter_id;
                int oldIndex = choiseIndex;
                choiseIndex = EditorGUILayout.Popup(choiseIndex, sChoise);
                slotController.scatter_id = choiseIndex;
                if (oldIndex != choiseIndex) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowJPSymbChoiseLO()
        {
            string[] sChoise = slotController.GetIconNames(true);
            if (sChoise == null || sChoise.Length == 0) return;
            EditorGUILayout.BeginHorizontal();
            if (slotController.useMiniJacPot || slotController.useMegaJacPot|| slotController.useMaxiJacPot)
            {
                EditorGUILayout.LabelField("Select Jackpot symbol ");
                int choiseIndex = slotController.jp_symbol_id+1;
                int oldIndex = choiseIndex;
                choiseIndex = EditorGUILayout.Popup(choiseIndex, sChoise);
                slotController.jp_symbol_id = choiseIndex-1;
                if (oldIndex != choiseIndex) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowChoiseLO(string [] choises)
        {
            int _choiceIndex = 0;
            if (choises == null || choises.Length==0) return;
            _choiceIndex = EditorGUILayout.Popup(_choiceIndex, choises);
            Debug.Log("choice: " + _choiceIndex);
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
        private void ShowChoise(string[] choises, Rect rect, float width, float height, float dx, float dy, PayLine pLine, int index)
        {
            if (choises == null || choises.Length == 0 || pLine.line==null || pLine.line.Length ==0 || pLine.line.Length <= index) return;
          
            int choiseIndex = pLine.line[index]+1; // any == 0;
            int oldIndex = choiseIndex;
            choiseIndex = EditorGUI.Popup(new
                Rect(rect.x + dx, rect.y+dy, width, height),
                //  "Icon: ",
                choiseIndex, choises);
            pLine.line[index] = choiseIndex-1;
            if(oldIndex != choiseIndex) EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
             
        }

        private void ShowPayLine(string[] choises, Rect rect, int count, float width, float height, int index, bool isActive, bool isFocused, PayLine pLine)
        {
            if (pLine == null) return;
            var element = payTableList.serializedProperty.GetArrayElementAtIndex(index);
            for (int i = 0; i < count; i++)
            {
                ShowChoise(choises, rect, width, height, i * width + i * 1.0f, 0, pLine, i);
            }
            float dx = rect.x + count * width + count;
            float w = 40;
            float h = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(new Rect(dx, rect.y, w, h), "Pay ");
            dx += w;
            w = 50;

            EditorGUI.PropertyField(new Rect(dx, rect.y, w, h),
                        element.FindPropertyRelative("pay"), GUIContent.none);
            dx += w; w = 75;
            EditorGUI.LabelField(new Rect(dx, rect.y, w,h), "FreeSpins");
            dx += w; w = 50;
            EditorGUI.PropertyField(new Rect(dx, rect.y, w,h),
                        element.FindPropertyRelative("freeSpins"), GUIContent.none);
            dx += w; w = 65;
            EditorGUI.LabelField(new Rect(dx, rect.y, w, h), "PayMult");
            dx += w; w = 40;
            EditorGUI.PropertyField(new Rect(dx, rect.y, w, h),
                        element.FindPropertyRelative("payMult"), GUIContent.none);
            dx += w; w = 100;
            EditorGUI.LabelField(new Rect(dx, rect.y, w, h), "FreeSpinsMult");
            dx += w; w = 40;
            EditorGUI.PropertyField(new Rect(dx, rect.y, w, h),
                        element.FindPropertyRelative("freeSpinsMult"), GUIContent.none);

            dx += w; w = 105;
            int evCounts = 0;
            if (slotController && slotController.payTable!= null )
            {
                evCounts = slotController.payTable[index].LineEvent.GetPersistentEventCount();
                //Debug.Log(evCounts);
            }
            string sE = (evCounts > 0) ? "ShowEvent(" + evCounts + "):" : "ShowEvent :";
            EditorGUI.LabelField(new Rect(dx, rect.y, w, h), sE);
            dx += w; w = 40;
            EditorGUI.PropertyField(new Rect(dx, rect.y, w, h),
                        element.FindPropertyRelative("showEvent"), GUIContent.none);
            if (element.FindPropertyRelative("showEvent").boolValue && isActive)
            {
                dx += w; w = 170;
                EditorGUI.PropertyField(new Rect(dx, rect.y, w, h),
                 element.FindPropertyRelative("LineEvent"), GUIContent.none);
            }
        }
        #endregion showChoise EditorGui
      
    }
}

/*
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
 */

    /*
     reordable list
        is active if any element of list is active - gray color
        is focused - blue
     */