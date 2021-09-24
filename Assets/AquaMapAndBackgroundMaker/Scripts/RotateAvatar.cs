using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateAvatar : MonoBehaviour
{
    public Image playerImage;
    public RectTransform maskRect;
    public bool rotate = true;
    public bool flip = true;

    private RectTransform Group;
    private LevelButton activeButton;
    private Canvas mainCanvas;
    private float groupAngleZ;

    void Start ()
    {
        Group = GetComponent<RectTransform>();
    }

    bool topHalf;
    void Update ()
    {
        if (!MapController.Instance) return;
        activeButton = MapController.Instance.ActiveButton;
        if (!mainCanvas) mainCanvas = MapController.Instance.parentCanvas;

        if (activeButton && mainCanvas)
        {
            Group.anchoredPosition = RectTransformToCanvasSpaceCenterCenter(activeButton.GetComponent<RectTransform>(),mainCanvas);
            topHalf = (flip) ? Group.anchoredPosition.y > 0 : false;

            groupAngleZ = (topHalf) ? Mathf.LerpAngle(groupAngleZ, 180, Time.deltaTime * 2.0f) : Mathf.LerpAngle(groupAngleZ, 0, Time.deltaTime * 2.0f);
            maskRect.localRotation = Quaternion.Euler(-Group.localRotation.eulerAngles);
            if (rotate) Group.localRotation = Quaternion.Euler(new Vector3(0, 0, groupAngleZ + 5f * Mathf.Sin(2f * Time.time)));
            else Group.localRotation = Quaternion.Euler(new Vector3(0, 0, topHalf ? 180 : 0));
        }
    }

    static Vector2 RectTransformToCanvasSpaceCenterCenter(RectTransform transform, Canvas c)
    {
        RectTransform CanvasRect = c.GetComponent<RectTransform>();
        Vector2 viewportPoint = Camera.main.ScreenToViewportPoint(new Vector3(transform.position.x, Screen.height - transform.position.y, transform.position.z));
        Vector2 WorldObject_ScreenPosition = new Vector2(
       ((viewportPoint.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
       (-(viewportPoint.y * CanvasRect.sizeDelta.y) + (CanvasRect.sizeDelta.y * 0.5f)));
        return WorldObject_ScreenPosition;
    }
}
