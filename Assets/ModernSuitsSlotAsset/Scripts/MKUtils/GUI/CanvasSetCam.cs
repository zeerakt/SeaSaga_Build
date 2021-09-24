using UnityEngine;

/*
	23.10.2019 - first
	22.11.2019 - remove slot reference
*/
namespace Mkey
{
    public class CanvasSetCam : MonoBehaviour
	{
        #region temp vars
        private Camera cam;
        private Canvas c;
        #endregion temp vars

        private void Update()
        {
            if (!cam) cam = Camera.main;
            if (!c) c = GetComponent<Canvas>();
            if (c && cam)
            {
                if (!c.worldCamera)
                {
                    c.worldCamera = cam;
                    Debug.Log("Camera set complete");
                }
            }
            else
            {
                Debug.Log("Camera not set to canvas, canvas component not found");
            }
        }
	}
}
