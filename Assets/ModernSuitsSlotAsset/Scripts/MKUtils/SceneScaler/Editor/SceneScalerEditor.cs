using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Mkey
{
    [CustomEditor(typeof(SceneScaler))]
    public class SceneScalerEditor : Editor
    {
        bool showDefault;
        public override void OnInspectorGUI()
        {
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
    }
}