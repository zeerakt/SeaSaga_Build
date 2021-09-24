using UnityEngine;
using System.Collections.Generic;

//http://www.habrador.com/tutorials/interpolation/1-catmull-rom-splines/
public class CatmulRommSpline_1 : MonoBehaviour
{
    public List<Vector3> handlesPositions;
    public List<Vector3> controlPositions;

    [HideInInspector]
    [SerializeField]
    private Biome biome;
    public void OnChangeSpline()
    {
        if (!biome) biome = GetComponent<Biome>();
        if (biome)
        {
            biome.SetButtonsPositionOnCurve();
        }
    }

    public bool Selected { get; set; }

    public float[] distances;


    /// <summary>
    /// Return handles count
    /// </summary>
    public int Count 
    {
        get
        {
            return handlesPositions.Count;
        }
    }

    /// <summary>
    /// Return path length
    /// </summary>
    public float Length
    {
        get
        {
            if (distances == null || distances.Length == 0) return 0;
            return distances[distances.Length - 1];
        }
    }

    /// <summary>
    /// Create initial curve
    /// </summary>
    public void Reset()
    {
        RectTransform rT = GetComponent<RectTransform>();

        Vector2 sd2 = rT.sizeDelta /2;
        Vector2 sd6 = rT.sizeDelta / 6;
        Vector3 []  points = new Vector3[4] {
            new Vector3(0f, -sd2.y, 0f),
            new Vector3(0f, -sd6.y, 0f),
            new Vector3(0f, sd6.y, 0f),
            new Vector3(0f, sd2.y, 0f)
        };
        handlesPositions = new List<Vector3>(points);
    }

    public void OnDrawGizmos()
    {
        if (Selected)
        {
            BuildAndShow();
        }
    }

    public void BuildAndShow()
    {
        Gizmos.color = Color.white;
        if (Count < 2) return;

        CreatePositons();
        CacheDistances();
        for (int i = 0; i < controlPositions.Count; i++)
        {
            if ((i == 0 || i == controlPositions.Count - 2 || i == controlPositions.Count - 1))
            {
                continue;
            }
            DisplayCatmullRomSpline(i);
        }
    }

    /// <summary>
    /// Create positions list for curve calculating
    /// </summary>
    /// <param name="points"></param>
    /// <returns></returns>
    private void CreatePositons()
    {
        //controlPositions = new List<Vector3>();
        //controlPositions.Add(transform.TransformPoint(handlesPositions[0])); // hidden duplicate 0 point
        //handlesPositions.ForEach((p) => { controlPositions.Add(transform.TransformPoint(p)); });
        //controlPositions.Add(transform.TransformPoint(handlesPositions[Count-1])); // hidden duplicate last point

        controlPositions = new List<Vector3>();
        controlPositions.Add(handlesPositions[0]); // hidden duplicate 0 point
        handlesPositions.ForEach((p) => { controlPositions.Add(p); });
        controlPositions.Add(handlesPositions[Count - 1]); // hidden duplicate last point
    }

    private void DisplayCatmullRomSpline(int pos)
    {
        Vector3 p0 = controlPositions[ClampListPos(pos - 1, controlPositions.Count)];
        Vector3 p1 = controlPositions[pos];
        Vector3 p2 = controlPositions[ClampListPos(pos + 1, controlPositions.Count)];
        Vector3 p3 = controlPositions[ClampListPos(pos + 2, controlPositions.Count)];
 
        Vector3 lastPos = p1;

        float resolution = 0.2f;

        int loops = Mathf.FloorToInt(1f / resolution);

        for (int i = 1; i <= loops; i++)
        {
            float t = i * resolution;
            Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);
            Gizmos.DrawLine(transform.TransformPoint(lastPos), transform.TransformPoint(newPos));
            lastPos = newPos;
        }
    }

    private int ClampListPos(int pos, int count)
    {
        if (pos < 0)
        {
            pos = count - 1;
        }

        if (pos > count)
        {
            pos = 1;
        }
        else if (pos > count - 1)
        {
            pos = 0;
        }

        return pos;
    }

    //Returns a position between 4 Vector3 with Catmull-Rom spline algorithm
    //http://www.iquilezles.org/www/articles/minispline/minispline.htm
    private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

        return pos;
    }

    public void RemovePoint(int selectedIndex)
    {
        Debug.Log("Remove point: " + selectedIndex);
        handlesPositions.RemoveAt(selectedIndex);
    }

    public void AddPoint(int selectedIndex)
    {
        Debug.Log("Add point: " + selectedIndex);
        Vector3 p0 = handlesPositions[selectedIndex];
        Vector3 p1 = handlesPositions[selectedIndex+1];
        Vector3 pn = (p1 + p0) / 2.0f;
        handlesPositions.Insert(selectedIndex+1, pn);
    }

    /// <summary>
    /// Retorn position on spline at distance
    /// </summary>
    /// <param name="dist"></param>
    /// <returns></returns>
    public Vector3 GetPosition(float dist)
    {
        int point = 0;
        int numPoints = distances.Length;
        float pathLength = Length;

        dist =(dist==pathLength) ? pathLength: Mathf.Repeat(dist, pathLength);
     
        while (point < distances.Length - 1 && distances[point] <= dist)
        {
            ++point;// Debug.Log("point: "+point + " ; dis: " + dist);
        }

        int p1n = ((point - 1) + numPoints) % numPoints;
        int p2n = point;

        float t = Mathf.InverseLerp(distances[p1n], distances[p2n], dist);

        int p0n = ((point - 2) + numPoints) % numPoints;
        int p3n = (point + 1) % numPoints;
        p2n = p2n % numPoints;

        Vector3 P0 = controlPositions[p0n];
        Vector3 P1 = controlPositions[p1n];
        Vector3 P2 = controlPositions[p2n];
        Vector3 P3 = controlPositions[p3n];
        return GetCatmullRomPosition(t, P0, P1, P2, P3);
    }

    private void CacheDistances()
    {
        int count = controlPositions.Count;
        distances = new float[count];

        float accumulateDistance = 0;
        for (int i = 0; i < count; ++i)
        {
            var t1 = controlPositions[(i) % count];
            var t2 = controlPositions[(i + 1) % count];
            if (t1 != null && t2 != null)
            {
                distances[i] = accumulateDistance;
                accumulateDistance += (t1 - t2).magnitude;
            }
        }
    }

    /// <summary>
    /// Return List of position on curve with equal distances inbetween
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public List<Vector3> GetPositions(int count )
    {
        List<Vector3> pos = new List<Vector3>();
        float delta = (Length) / (count - 1);
        for (int i = 0; i < count-1; i++)
        {
            pos.Add(GetPosition(i * delta));
        }
        pos.Add(GetPosition(Length));
        return pos;
    }

}
