using DG.Tweening;
using UnityEngine;

public class UISlideIn : MonoBehaviour
{
    public RectTransform uiPanel;
    public Vector2 targetInPosition = Vector2.zero;
    public Vector2 targetOutPosition = Vector2.zero;
    public float duration = 0.5f;
    public static UISlideIn uISlideIn;
    private void Awake()
    {
        uISlideIn = this;
    }
    public void SlideIn()
    {
        // Smoothly moves anchoredPosition with a natural slow-down at the end
        uiPanel.DOAnchorPos(targetInPosition, duration).SetEase(Ease.OutCubic);
    }
    public void SlideOut()
    {
        // Smoothly moves anchoredPosition with a natural slow-down at the end
        uiPanel.DOAnchorPos(targetOutPosition, duration).SetEase(Ease.OutCubic);
    }
}
