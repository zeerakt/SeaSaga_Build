using UnityEngine;
using UnityEngine.UI;
using Mkey;

namespace MkeyFW
{
    public class SpinButton : MonoBehaviour
    {
        [SerializeField]
        public Button.ButtonClickedEvent clickEvent;
        public bool interactable = true;
        private Collider2D bCollider;
        private SpriteRenderer sR;
        private Camera cam;
        private Ray ray;
        private RaycastHit2D hit;

        #region regular
        void Start()
        {
            bCollider = GetComponent<Collider2D>();
            sR = GetComponent<SpriteRenderer>();
            cam = Camera.main;
        }

        void Update()
        {
            if (!bCollider) return;
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
                    hit = Physics2D.Raycast(ray.origin, ray.direction);
                    if (hit && hit.collider && hit.collider == bCollider)
                    {
                        OnClickEvent();
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                ray = cam.ScreenPointToRay(Input.mousePosition);
                hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit && hit.collider && hit.collider == bCollider)
                {
                    OnClickEvent();
                }
            }
        }
        #endregion regular

        /// <summary>
        /// Raise click event
        /// </summary>
        private void OnClickEvent()
        {
            Debug.Log(name);
            if (!interactable) return;
            if (clickEvent != null) clickEvent.Invoke();
            SimpleTween.Value(gameObject, -0.2f, 0.2f, 0.3f)
                .SetOnUpdate((float val)=>
                {
                    if(this && sR)
                    {
                        if (val < 0)
                            val = -(val + 0.2f);
                        else
                            val = val - 0.2f;
                        sR.color = new Color(1 + val, 1 + val, 1 + val, 1);
                    }
                })
                .AddCompleteCallBack(()=> { if (this && sR) sR.color = new Color(1, 1, 1, 1); });
        }
    }
}