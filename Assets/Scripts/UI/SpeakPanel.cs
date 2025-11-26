using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class SpeakPanel : BasePanel
{
    [Header("UI References")]
    public Text speakText;
    public RectTransform backgroundRect;

    [Header("Display Settings")]
    public float autoHideTime = 2f;// 自动隐藏时间
    public float typingSpeed = 0.05f; // 打字速度

    [Header("Size Settings")]
    public Vector2 textPadding = new Vector2(20, 10);
    public float minWidth = 300f;
    public float maxWidth = 600f;
    public float widthK = 1.5f;
    public float heightK = 5f; // 文本高度的放大系数
    public string t = "这是一个测试对话气泡的内容,用于调试显示效果。";

    private Tween autoHideTween;
    private Tween typingTween;
    private Action pendingCallback;
    private string fullText; // 完整文本

    #region API
    // 显示对话气泡(世界坐标/屏幕坐标)
    public void ShowSpeak(string text, Vector3 worldPosition,bool useWorldPosition= true, Action onComplete = null)
    {
        autoHideTween?.Kill();
        typingTween?.Kill();

        fullText = text;
        speakText.text = text; // 先设置完整文本用于计算尺寸
        AdjustPanelSize();
        if(useWorldPosition)
            transform.position = Camera.main.WorldToScreenPoint(worldPosition);
        else
            transform.position = worldPosition;
        // 清空文本
        speakText.text = "";
        Show();
        //开始打字并开始倒计时隐藏
        StartTyping(onComplete,autoHideTime);
    }

    // 可传入显示时间参数和现实方式的重载方法
    public void ShowSpeak(string text,Vector3 position,float displayTime,bool ifUseFade = true,bool useWorldPosition=true, Action onComplete = null)
    {
        autoHideTween?.Kill();
        typingTween?.Kill();

        fullText = text;
        speakText.text = text;
        AdjustPanelSize();
        // 根据坐标类型设置位置
        if (useWorldPosition)
            transform.position = Camera.main.WorldToScreenPoint(position);
        else
            transform.position = position;

        speakText.text = "";
        if(ifUseFade)
        {
            Show();
            DOVirtual.DelayedCall(fadeTime,() =>
            {
                speakText.DOText(fullText,fullText.Length*typingSpeed)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    autoHideTween = DOVirtual.DelayedCall(displayTime, () =>
                    {
                        HideSpeak(onComplete);
                    }).SetUpdate(true);
                });
            }).SetUpdate(true);
        }
        else
        {
            canvasGroup.alpha = 1f;
            gameObject.SetActive(true);
            isShowing = true;
            speakText.DOText(fullText, fullText.Length * typingSpeed)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    autoHideTween = DOVirtual.DelayedCall(displayTime, () =>
                    {
                        HideSpeak(onComplete);
                    }).SetUpdate(true);
                });
        }
    }

    // 手动隐藏对话气泡
    public void HideSpeak(Action onComplete = null)
    {
        autoHideTween?.Kill();
        typingTween?.Kill();
        Hide(onComplete);
    }
    #endregion

    #region 内部方法
    // 调整面板大小以适应文本长度
    private void AdjustPanelSize()
    {
        if (speakText == null || backgroundRect == null) return;

        Canvas.ForceUpdateCanvases();

        float preferredWidth = Mathf.Clamp(speakText.preferredWidth * widthK + textPadding.x * 2, minWidth, maxWidth);
        speakText.rectTransform.sizeDelta = new Vector2((preferredWidth - textPadding.x * 2) / widthK, speakText.preferredHeight);
        Canvas.ForceUpdateCanvases();

        float preferredHeight = speakText.preferredHeight * heightK + textPadding.y * 2;
        backgroundRect.sizeDelta = new Vector2(preferredWidth, preferredHeight);
    }

    private void StartTyping(Action onComplete,float hideTime)
    {
        //X
        DOVirtual.DelayedCall(fadeTime,() =>
        {
            // 打字机效果
            speakText.text = "";
            speakText.DOText(fullText,fullText.Length*typingSpeed)
            .SetEase(Ease.Linear)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                // 打字完成后开始自动隐藏倒计时
                autoHideTween = DOVirtual.DelayedCall(hideTime, () =>
                {
                    HideSpeak(onComplete);
                }).SetUpdate(true);
            });
        }).SetUpdate(true);
    }

    #endregion

    //便于调试,公开方法,右键组件调用
    [ContextMenu("Test Show Speak")]
    private void TestShowSpeak()
    {
        ShowSpeak(t, Vector3.zero);
    }
}
