using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PixelatedInput : BaseRaycaster
{
    public Canvas canvas;        // canvas that directly contains the buttons
    public Camera renderCamera;  // camera that renders this canvas into the low-res texture
    public RawImage display;     // RawImage showing the texture (optional if fullscreen)

    public override Camera eventCamera => renderCamera;
    public override int sortOrderPriority => canvas.sortingOrder;
    public override int renderOrderPriority => canvas.renderOrder;

    static readonly List<Graphic> hits = new List<Graphic>();
    static readonly Vector3[] corners = new Vector3[4];

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        Vector2 min = Vector2.zero;
        Vector2 max = new Vector2(Screen.width, Screen.height);

        if (display != null)
        {
            Canvas root = display.canvas.rootCanvas;
            Camera cam = root.renderMode == RenderMode.ScreenSpaceOverlay ? null : root.worldCamera;
            display.rectTransform.GetWorldCorners(corners);
            min = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
            max = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);
        }

        float u = (eventData.position.x - min.x) / (max.x - min.x);
        float v = (eventData.position.y - min.y) / (max.y - min.y);
        if (u < 0f || u > 1f || v < 0f || v > 1f) return;

        hits.Clear();
        var graphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
        for (int i = 0; i < graphics.Count; i++)
        {
            Graphic g = graphics[i];
            if (!g.raycastTarget || g.canvasRenderer.cull || g.depth == -1) continue;

            g.rectTransform.GetWorldCorners(corners);
            Vector3 a = renderCamera.WorldToViewportPoint(corners[0]);
            Vector3 b = renderCamera.WorldToViewportPoint(corners[2]);

            if (u < Mathf.Min(a.x, b.x) || u > Mathf.Max(a.x, b.x) ||
                v < Mathf.Min(a.y, b.y) || v > Mathf.Max(a.y, b.y)) continue;

            hits.Add(g);
        }

        hits.Sort((x, y) => y.depth.CompareTo(x.depth));

        Vector2 screenPos = new Vector2(u * renderCamera.pixelWidth, v * renderCamera.pixelHeight);
        for (int i = 0; i < hits.Count; i++)
        {
            Graphic g = hits[i];
            resultAppendList.Add(new RaycastResult
            {
                gameObject = g.gameObject,
                module = this,
                screenPosition = screenPos,
                index = resultAppendList.Count,
                depth = g.depth,
                sortingLayer = canvas.sortingLayerID,
                sortingOrder = canvas.sortingOrder
            });
        }
    }
}