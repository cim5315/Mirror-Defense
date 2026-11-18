using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RenderTextureGraphicRaycaster : GraphicRaycaster
{
    [SerializeField] private Camera uiCamera;

    public override Camera eventCamera => uiCamera != null ? uiCamera : base.eventCamera;

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (uiCamera == null || uiCamera.targetTexture == null)
        {
            base.Raycast(eventData, resultAppendList);
            return;
        }

        // Convert full screen mouse position into Render Texture pixel coordinates
        RenderTexture rt = uiCamera.targetTexture;
        Vector2 originalPosition = eventData.position;

        eventData.position = new Vector2(
            (originalPosition.x / Screen.width) * rt.width,
            (originalPosition.y / Screen.height) * rt.height
        );

        // Temporarily detach targetTexture during the raycast so Unity doesn't reject it
        uiCamera.targetTexture = null;
        base.Raycast(eventData, resultAppendList);
        uiCamera.targetTexture = rt;

        eventData.position = originalPosition;
    }
}