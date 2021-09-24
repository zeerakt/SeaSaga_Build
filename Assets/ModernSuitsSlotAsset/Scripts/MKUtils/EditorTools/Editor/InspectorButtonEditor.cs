using UnityEngine;
using System.Collections;
using UnityEditor;

/*
 * 06.01.2020 - first
 */
namespace Mkey {
    [CustomEditor(typeof(Inspector_Button))]
    public class Editor_Ext_1 : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Inspector_Button myScript = (Inspector_Button)target;
            if (GUILayout.Button("first"))
            {
                myScript.FirstButton_Click();
            }

            if (GUILayout.Button("second"))
            {
                myScript.SeconButton_Click();
            }

            if (GUILayout.Button("third"))
            {
                myScript.ThirdButton_Click();
            }
        }
    }
}