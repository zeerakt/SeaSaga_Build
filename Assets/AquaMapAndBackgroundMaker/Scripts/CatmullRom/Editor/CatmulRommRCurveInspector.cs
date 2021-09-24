using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CatmulRommSpline_1))]
public class CatmulRommRCurveInspector : Editor
{
    private CatmulRommSpline_1 curve;
    private Transform handleTransform;
    private Quaternion handleRotation;
    private const float directionScale = 0.5f;
    private static Color[] modeColors = {
        Color.white,
        Color.yellow,
        Color.cyan
    };
    private const float handleSize = 0.04f;
    private const float pickSize = 0.06f;
    private int selectedIndex = -2;

    private void OnSceneGUI()
    {
        curve = target as CatmulRommSpline_1;
        handleTransform = curve.transform;
        handleRotation = (Tools.pivotRotation == PivotRotation.Local) ? handleTransform.rotation : Quaternion.identity;

        ShowControlPoints();
        Handles.color = Color.gray;
        ShowPivot();
    }

    void OnEnable()
    {
        curve = target as CatmulRommSpline_1;
        if (curve) curve.Selected = true;
    }

    void OnDisable()
    {
        curve = target as CatmulRommSpline_1;
        if (curve) curve.Selected = false;

    }

    private Vector3 ShowControlPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(curve.handlesPositions[index]);
        float size = HandleUtility.GetHandleSize(point);
        if (index == 0)
        {
            size *= 2f;
        }
        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            selectedIndex = index;
            Repaint();
        }
        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve.handlesPositions[index] = handleTransform.InverseTransformPoint(point);
                curve.OnChangeSpline();
            }
        }
        return point;
    }

    private void ShowControlPoints()
    {
        curve = target as CatmulRommSpline_1;
        for (int i = 0; i < curve.Count; i++)
        {
            ShowControlPoint(i);
        }
    }

    private Vector3 ShowPivot()
    {
        Vector3 point = curve.transform.position;
        float size = HandleUtility.GetHandleSize(point);
        Handles.color = Color.red;

        if (Handles.Button(point, handleRotation, 4f * handleSize, 4f * pickSize, Handles.RectangleHandleCap))
        {
            selectedIndex = -1;
            Repaint();
        }
        return point;
    }

    public override void OnInspectorGUI()
    {
        //   DrawDefaultInspector();
        curve = target as CatmulRommSpline_1;
        if (!curve) return;
        if (selectedIndex >= 0 && selectedIndex < curve.handlesPositions.Count - 1)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Press to add or remove curve control point.");

            if (GUILayout.Button("Add Point"))
            {
                Undo.RecordObject(curve, "Add Point");
                EditorUtility.SetDirty(curve);
                curve.AddPoint(selectedIndex);
                curve.OnChangeSpline();
            }

            if (curve.Count > 3)
            {
                if (GUILayout.Button("Remove Point"))
                {
                    Undo.RecordObject(curve, "Remove Point");
                    EditorUtility.SetDirty(curve);
                    curve.RemovePoint(selectedIndex);
                    curve.OnChangeSpline();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        if (selectedIndex >= 0 && selectedIndex < curve.handlesPositions.Count)
        {
            DrawSelectedPointInspector();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select curve control point for curve edit.");
            EditorGUILayout.EndHorizontal();
        }
    }

    private void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", curve.handlesPositions[selectedIndex]);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(curve, "Move Point");
            EditorUtility.SetDirty(curve);
            curve.handlesPositions [selectedIndex]= point;
        }

        EditorGUI.BeginChangeCheck();
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(curve);
        }
    }

}
