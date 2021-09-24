using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//[ExecuteInEditMode]
public class Biome : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public int count;
    public List<LevelButton> levelButtons;

    [SerializeField]
    private bool snapButtonsToCurve = true;

    [HideInInspector]
    [SerializeField]
    private bool snapButtonsToCurveOld = false;

    public Vector2 BiomeSize
    {
        get { return GetComponent<RectTransform>().sizeDelta; }
    }

    private void OnValidate()
    {
        if (count < 0) count = 0;
        if(snapButtonsToCurve & !snapButtonsToCurveOld)
        {
            SetButtonsPositionOnCurve();
        }
        snapButtonsToCurveOld = snapButtonsToCurve;
    }

    public void Create()
    {
        LevelButton[] existingButtons = GetComponentsInChildren<LevelButton>();

        bool rebuildInOldPositions = false;
        Vector3[] existingPositions = new Vector3[existingButtons.Length];
        if(existingButtons!=null && count == existingButtons.Length)
        {
            rebuildInOldPositions = true;
            for (int i = 0; i < existingButtons.Length; i++)
            {
                if (existingButtons[i])
                {
                    existingPositions[i] = existingButtons[i].transform.position;
                }
                else
                {
                    rebuildInOldPositions = false;
                    break;
                }
            }
        }

        foreach (var b in existingButtons)
        {
            DestroyImmediate(b.gameObject);
        }
        levelButtons = new List<LevelButton>();

        for (int i = 0; i < count; i++)
        {
            if (rebuildInOldPositions) CreateButton(existingPositions[i]);
            else CreateButton();
        }
         SetButtonsPositionOnCurve();
    }

    private void CreateButton()
    {
        if (levelButtons == null) levelButtons = new List<LevelButton>();
        if (!levelButtonPrefab)
        {
            Debug.Log("Set level buttons prefab");
            return;
        }
        GameObject b = Instantiate(levelButtonPrefab);
        b.transform.localScale = transform.lossyScale;
        b.transform.position = transform.position;
        b.transform.SetParent(transform);
        levelButtons.Add(b.GetComponent<LevelButton>());
    }

    private void CreateButton(Vector3 position)
    {
        if (levelButtons == null) levelButtons = new List<LevelButton>();
        if (!levelButtonPrefab)
        {
            Debug.Log("Set level buttons prefab");
            return;
        }
        GameObject b = Instantiate(levelButtonPrefab);
        b.transform.localScale = transform.lossyScale;
        b.transform.SetParent(transform);
        b.transform.position = position;
        levelButtons.Add(b.GetComponent<LevelButton>());
    }


    List<Vector3> pos;
    CatmulRommSpline_1 curve;
    public void SetButtonsPositionOnCurve()
    {
        if (snapButtonsToCurve)
        {
            if (!curve) curve = GetComponent<CatmulRommSpline_1>();
            if (!curve) return;
            if (levelButtons == null) return;

            pos = curve.GetPositions(levelButtons.Count);
            if (levelButtons.Count > 0)
            {
                for (int i = 0; i < levelButtons.Count; i++)
                {
                  if(levelButtons[i])  levelButtons[i].transform.position = transform.TransformPoint(pos[i]); //  if(levelButtons[i])  levelButtons[i].transform.position =pos[i];
                }
            }
        }
    }
}


/* Old
   private void OnDestroy()
    {
#if UNITY_EDITOR
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }
        Debug.Log("Biome destroyed " + name);

        MapMaker m = GetComponentInParent<MapMaker>();
        if (m)
        {
            m.RemoveBiome(this, false);
        }
        else
        {
            Debug.Log("mapMaker not found : " + name);
        }
#endif
    }
*/
