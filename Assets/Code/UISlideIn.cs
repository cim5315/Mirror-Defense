using DG.Tweening;
using UnityEngine;

public class UISlideIn : MonoBehaviour
{
    public RectTransform uiPanel;
    public Vector2 targetPosition = Vector2.zero; // Center of the screen
    public float duration = 0.5f;
    public static UISlideIn uISlideIn;
    private void Awake()
    {
        uISlideIn = this;
    }
    public void SlideIn()
    {
        // Smoothly moves anchoredPosition with a natural slow-down at the end
        uiPanel.DOAnchorPos(targetPosition, duration).SetEase(Ease.OutCubic);
    }
}
