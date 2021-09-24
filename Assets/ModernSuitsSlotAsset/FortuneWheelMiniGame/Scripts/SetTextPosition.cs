using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace MkeyFW
{
    public class SetTextPosition : MonoBehaviour
    {
      public float dist = 2.5f;
      public void SetPostion()
        {
            Sector[] sectors = GetComponentsInChildren<Sector>();
            int length = sectors.Length;
            float dAngleDeg = 360f / length;

            // set position
            for (int i = 0; i < length; i++)
            {
              sectors[i].transform.parent = null;
              float angleDeg =90.0f - i * dAngleDeg;
              float angleRad = angleDeg * Mathf.Deg2Rad;
              sectors[i].transform.position = transform.position + new Vector3(dist * Mathf.Cos(angleRad), dist * Mathf.Sin(angleRad), 0);
              sectors[i].transform.localEulerAngles = new Vector3(0, 0, angleDeg);
              sectors[i].transform.parent = transform;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SetTextPosition))]
    public class SetTextPositionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SetTextPosition setText = (SetTextPosition)target;
            serializedObject.Update();

            DrawDefaultInspector();
            if (GUILayout.Button("Arrange sector text"))
            {
                setText.SetPostion();
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
          
        }
    }
#endif
}
