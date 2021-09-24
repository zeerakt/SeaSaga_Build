using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Linq;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Mkey
{
    [CustomEditor(typeof(LineBehavior))]
    public class LineBehaviorEditor : Editor
    {
        LineBehavior lineBehavior;
 

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        bool showDefault;
        public override void OnInspectorGUI()
        {
            lineBehavior = (LineBehavior)target;
            serializedObject.Update();

            ShowPropertiesBox("", new string[] { "number" }, false);

            #region raycasters
            ShowPropertiesBox("", new string[] { "rayCasters" }, true);
            #endregion

          //  #region dots
          //  EditorGUILayout.BeginVertical("box");
          //  EditorGUI.indentLevel += 1;
          //  EditorGUILayout.Space();
          //  ShowProperties("Line dots: ", new string[] { "dotMaterial", "dotSprite", "dotDistance", "burnTime", "dotList", "dotSortingOrder" }, true);
          ////  lineBehavior.dotSortingLayerID = DrawSortingLayersPopup("Dots sorting layer: ", lineBehavior.dotSortingLayerID);
          //  EditorGUILayout.Space();
          //  EditorGUI.indentLevel -= 1;
          //  EditorGUILayout.EndVertical();
          //  #endregion dots

            #region lineInfo
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            ShowProperties("Line Info panel: ", new string[] { "lineInfoColor","lineInfoBGColor" }, true);
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
            #endregion lineInfo


            serializedObject.ApplyModifiedProperties();
            
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
        }

        #region showProperties
        private void ShowProperties(string header, string[] properties, bool showHierarchy)
        {
            if (!string.IsNullOrEmpty(header))
            {
                EditorGUILayout.LabelField(header, new GUIStyle(EditorStyles.boldLabel));
            }
            for (int i = 0; i < properties.Length; i++)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(properties[i]), showHierarchy);
            }
        }

        private void ShowPropertiesBox(string header, string[] properties, bool showHierarchy)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            ShowProperties(header, properties, showHierarchy);
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndVertical();
        }

        private void ShowPropertiesBoxFoldOut(string bName, string[] properties, ref bool fOut, bool showHierarchy)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUI.indentLevel += 1;
            EditorGUILayout.Space();
            if (fOut = EditorGUILayout.Foldout(fOut, bName))
            {
                ShowProperties(null, properties, showHierarchy);
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
        private void ShowChoiseLO(string[] choises)
        {
            int _choiceIndex = 0;
            if (choises == null || choises.Length == 0) return;
            _choiceIndex = EditorGUILayout.Popup(_choiceIndex, choises);
            EditorUtility.SetDirty(target);
        }
        #endregion showChoise EditorGuiLayOut

        /// <summary>
        /// Draws a popup of the project's existing sorting layers.
        /// </summary>
        ///<param name="layerID">The internal layer id, can be assigned to renderer.SortingLayerID to change sorting layers.</param>
        /// <returns></returns>
        public static int DrawSortingLayersPopup(string label, int layerID)
        {
            /*
              https://answers.unity.com/questions/585108/how-do-you-access-sorting-layers-via-scripting.html
            */

            EditorGUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.LabelField(label);
            }
            var layers = SortingLayer.layers;
            var names = layers.Select(l => l.name).ToArray();
            if (!SortingLayer.IsValid(layerID))
            {
                layerID = layers[0].id;
            }
            var layerValue = SortingLayer.GetLayerValueFromID(layerID);
            var newLayerValue = EditorGUILayout.Popup(layerValue, names);
            EditorGUILayout.EndHorizontal();
            SetSceneDirty(newLayerValue != layerValue);
            return layers[newLayerValue].id;
        }

        private static void SetSceneDirty(bool dirty)
        {
            if (dirty)
            {
                if (!SceneManager.GetActiveScene().isDirty)
                {
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
        }

        ReorderableList CreateList(SerializedObject obj, SerializedProperty prop, bool showAddMenu) // https://pastebin.com/WhfRgcdC
        {
            ReorderableList list = new ReorderableList(obj, prop, true, true, true, true);

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Sprites");
            };

            List<float> heights = new List<float>(prop.arraySize);

            list.drawElementCallback = (rect, index, active, focused) =>
            {
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
                    heights = new List<float>(floats);
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

            list.elementHeightCallback = (index) =>
            {
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

            list.drawElementBackgroundCallback = (rect, index, active, focused) =>
            {
                rect.height = heights[index];
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                tex.Apply();
                if (active)
                    GUI.DrawTexture(rect, tex as Texture);
            };

            list.onAddDropdownCallback = (rect, li) =>
            {
                if (showAddMenu)
                {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Add Element"), false, () =>
                    {
                        serializedObject.Update();
                        li.serializedProperty.arraySize++;
                        serializedObject.ApplyModifiedProperties();
                    });

                    menu.ShowAsContext();
                }
                else
                {
                    serializedObject.Update();
                    li.serializedProperty.arraySize++;
                    serializedObject.ApplyModifiedProperties();
                }
                float[] floats = heights.ToArray();
                Array.Resize(ref floats, prop.arraySize);
                heights = new List<float>(floats);
            };

            return list;
        }
    }
}
