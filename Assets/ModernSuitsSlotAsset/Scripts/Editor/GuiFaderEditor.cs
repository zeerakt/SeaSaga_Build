using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
namespace Mkey
{
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
                if (woSP!=null)
                {
                    BeginBox();

                    ShowProperties(new SerializedProperty[] {
                        woSP.FindPropertyRelative("instantiatePosition")
                    }, true);

                    if(wo.instantiatePosition == Position.CustomPosition)
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
               if(!string.IsNullOrEmpty(properties[i])) EditorGUILayout.PropertyField(serializedObject.FindProperty(properties[i]), showHierarchy);
            }
        }

        private void ShowProperties(SerializedProperty[] properties, bool showHierarchy)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i]!=null) EditorGUILayout.PropertyField(properties[i], showHierarchy);
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
}