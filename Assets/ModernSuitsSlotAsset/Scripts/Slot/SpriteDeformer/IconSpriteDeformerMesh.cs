using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Mkey
{
    public class IconSpriteDeformerMesh : MonoBehaviour
    {
        [SerializeField]
        private Material m;
        //[SerializeField]
        private float effect = 10f;

        //[Header("Vertices by width, height")]
        //[SerializeField]
        private int width = 10;
        //[SerializeField]
        private int height = 3;


        #region private
      //  [SerializeField]
        private Transform cent;
        private Vector3 wCenterPosition;
        private float mWidth = 1;
        private float mHeight = 1;
       // [SerializeField]
        private Texture2D t2d;
        private int vertexCount;
      //  [SerializeField]
        private Vector3[] vertices;
     //   [SerializeField]
        private Vector3[] newVertices;
        private int[] triangles;
        private Vector2[] uv;

        public int SortingOrder { get; private set; }
        public int SortingLayerID { get; private set; }

        private SpriteRenderer sr;
        float ppu = 100f;
        private Mesh mesh;
        private MeshRenderer mR;
        private Vector3 centerLocPos;
        private bool canUpdate = false;
        #endregion private

        #region regular
        private void Start()
        {
            cent = GetComponentInParent<SlotGroupBehavior>().transform;
            CreateMesh(cent.position);
        }

        private void Update()
        {
          if(canUpdate)  UpdateVertexPerspectiveReel();
        }
        #endregion regular

        private void RefreshSort()
        {
            if (!mR)
                mR = GetComponent<MeshRenderer>();
            if (!mR) return;

            mR.sortingLayerID = SortingLayerID;
            mR.sortingOrder = SortingOrder;
        }

        public void CreateMesh(Vector3 wCenterPosition)
        {
            canUpdate = false;
            this.wCenterPosition = wCenterPosition;

            sr = GetComponent<SpriteRenderer>();
            if (!sr || !sr.sprite) return;

            // get texture data
            t2d = sr.sprite.texture;
            ppu = sr.sprite.pixelsPerUnit;
            mWidth = t2d.width / ppu;
            mHeight = t2d.height / ppu;
            SortingOrder = sr.sortingOrder;
            SortingLayerID = sr.sortingLayerID;
            DestroyImmediate(GetComponent<SpriteRenderer>());

            vertexCount = (width + 1) * (height + 1);

            int trianglesCount = width * height * 6;
            vertices = new Vector3[vertexCount];
            triangles = new int[trianglesCount];
            uv = new Vector2[vertexCount];
            int t;

            for (int y = 0; y <= height; y++)
            {
                for (int x = 0; x <= width; x++)
                {
                    int v = (width + 1) * y + x;
                    vertices[v] = new Vector3(mWidth * (x / (float)width - 0.5f),
                            mHeight * (y / (float)height - 0.5f), 0);
                    uv[v] = new Vector2(x / (float)width, y / (float)height);

                    if (x < width && y < height)
                    {
                        t = 3 * (2 * width * y + 2 * x);

                        triangles[t] = v;
                        triangles[++t] = v + width + 1;
                        triangles[++t] = v + width + 2;
                        triangles[++t] = v;
                        triangles[++t] = v + width + 2;
                        triangles[++t] = v + 1;
                    }
                }
            }

            if (!GetComponent<MeshFilter>()) gameObject.AddComponent<MeshFilter>(); // GetComponent<MeshFilter>().mesh;
            mesh = GetComponent<MeshFilter>().mesh;
            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;

            if (!GetComponent<MeshRenderer>()) mR = gameObject.AddComponent<MeshRenderer>();
            mR = GetComponent<MeshRenderer>();
            mR.material = m;
            mR.material.mainTexture = t2d;
            mR.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            mR.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            mR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mR.receiveShadows = false;

            newVertices = new Vector3[vertexCount];

            RefreshSort();
            canUpdate = true;
        }

        public void SetTexture(Texture2D newTexture)
        {
            //  if (!newTexture) canUpdate = false;
            t2d = newTexture;
            mR.material.mainTexture  = newTexture;
        }

        private void UpdateVertexPerspectiveReel()
        {
            centerLocPos = LocalPosition(wCenterPosition);

            for (int i = 0; i < vertexCount; i++)
            {
                newVertices[i] = new Vector3(
                    vertices[i].x * Mathf.Lerp(1.0f, 0.8f, (Mathf.Abs((centerLocPos.y - vertices[i].y) / effect))), 
                    vertices[i].y * Mathf.Lerp(1.0f, 0.7f, (Mathf.Abs((centerLocPos.y - vertices[i].y) / effect))),
                    vertices[i].z);
            }

            mesh.vertices = newVertices;
            mesh.RecalculateBounds();
        }

        private Vector3 LocalPosition(GameObject obj)
        {
            return transform.InverseTransformPoint(obj.transform.position);
        }

        private Vector3 LocalPosition(Transform obj)
        {
            return transform.InverseTransformPoint(obj.position);
        }

        private Vector3 LocalPosition(Vector3 pos)
        {
            return transform.InverseTransformPoint(pos);
        }
    }
}