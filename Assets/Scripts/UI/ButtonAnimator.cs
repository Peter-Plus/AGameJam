using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Button button;
    private RectTransform rectTransform;

    [Header("动画设置")]
    public float hoverScale = 1.05f; // 悬停时的缩放比例
    public float clickScale = 0.95f; // 点击时的缩放比例
    public float animDuration = 0.2f; // 动画持续时间

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

//Q 这个脚本有什么作用？
//A 这个脚本用于为UI按钮添加交互动画效果。当用户将鼠标悬停在按钮上时，按钮会稍微放大；当用户按下按钮时，按钮会稍微缩小；当用户释放按钮时，按钮会恢复到悬停状态的大小。这些动画效果通过DOTween库实现，使按钮在交互时更加生动和有趣。
