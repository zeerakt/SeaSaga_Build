using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace MkeyFW
{
    public class LampsPositions : MonoBehaviour
    {
        public float dist = 7.0f;
        public void SetPostion()
        {
            List<Transform> lamps = new List<Transform>(GetComponentsInChildren<Transform>());

            lamps.RemoveAll((t) => { return t == transform; });

            int length = lamps.Count;
            float dAngleDeg = 360f / length;
            float dAngleDegHalf = dAngleDeg / 2.0f;
            transform.localPosition = Vector3.zero;
            // set position
            for (int i = 0; i < length; i++)
            {
                lamps[i].transform.parent = null;
                float angleDeg = 90.0f - dAngleDegHalf - i * dAngleDeg;
                float angleRad = angleDeg * Mathf.Deg2Rad;
                lamps[i].transform.position = transform.position + new Vector3(dist * Mathf.Cos(angleRad), dist * Mathf.Sin(angleRad), 0);
                lamps[i].transform.localEulerAngles = new Vector3(0, 0, angleDeg);
                lamps[i].transform.parent = transform;
            }

        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(LampsPositions))]
    public class LampsPositionsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LampsPositions setLamps = (LampsPositions)target;
            serializedObject.Update();

            DrawDefaultInspector();
            if (GUILayout.Button("Arrange lamps"))
            {
                setLamps.SetPostion();
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

        }
    }
#endif

}