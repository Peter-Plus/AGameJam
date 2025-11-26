using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Button button;
    private RectTransform rectTransform;

    [Header("动画设置")]
    public float hoverScale = 1.05f;
    public float clickScale = 0.95f;
    public float animDuration = 0.2f;

    void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            rectTransform.DOScale(hoverScale, animDuration).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
            rectTransform.DOScale(1f, animDuration).SetUpdate(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
            rectTransform.DOScale(clickScale, animDuration * 0.5f).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button.interactable)
            rectTransform.DOScale(hoverScale, animDuration * 0.5f).SetUpdate(true);
    }
}
