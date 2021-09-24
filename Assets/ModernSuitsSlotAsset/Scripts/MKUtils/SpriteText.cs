using UnityEngine;

#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace Mkey
{
    /*
     25.11.18
     V1.1
     Add editor
     Add method RefreshSort();
     110119
     add properties
     add onvalidate
     
    31.01.19
    add editor in one file

    19.10.2019
        add check sorting layer id in Start method

     */
    [ExecuteInEditMode]
    public class SpriteText : MonoBehaviour
    {
        [SerializeField]
        private int sortingOrder;
        [SerializeField]
        private int sortingLayerID;

        public int SortingLayerID
        {
            get { return sortingLayerID; }
            set
            {
                var layers = SortingLayer.layers;

                if (!SortingLayer.IsValid(value))
                {
                    sortingLayerID = layers[0].id;
                }
                sortingLayerID = value;
                RefreshSort();
            }
        }

        private int SortingOrder
        {
            get { return sortingOrder; }
            set { sortingOrder = Mathf.Max(0, value); RefreshSort(); }
        }

        private Renderer rend;

        #region regular
        void Start()
        {
            // check sorting layer id
            var layers = SortingLayer.layers;
            if (!SortingLayer.IsValid(sortingLayerID))
            {
                sortingLayerID = layers[0].id;
            }

            RefreshSort();
        }

        private void OnValidate()
        {
            sortingOrder = Mathf.Max(0, sortingOrder);
        }
        #endregion regular

        public void RefreshSort()
        {
            if (!rend)
                rend = GetComponent<Renderer>();
            if (!rend) return;
           
            rend.sortingLayerID = SortingLayerID;
            rend.sortingOrder = SortingOrder;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpriteText))]
    public class SpriteTextEditor : Editor
    {
        SpriteText spriteText;

        public override void OnInspectorGUI()
        {
            //  base.OnInspectorGUI();
            spriteText = (SpriteText)target;
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sortingOrder"), false);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spriteText, "sorting order");
                EditorUtility.SetDirty(spriteText);
                spriteText.RefreshSort();
            }

            EditorGUI.BeginChangeCheck();
            spriteText.SortingLayerID = DrawSortingLayersPopup("Sorting layer: ", spriteText.SortingLayerID);
            serializedObject.ApplyModifiedProperties();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spriteText, "sorting layer");
                EditorUtility.SetDirty(spriteText);
                spriteText.RefreshSort();
            }
        }

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
    }
#endif
}