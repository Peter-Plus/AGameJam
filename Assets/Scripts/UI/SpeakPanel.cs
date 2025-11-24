using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class SpeakPanel : BasePanel
{
    [Header("UI References")]
    public Text speakText;
    public RectTransform backgroundRect;

    [Header("Display Settings")]
    public float autoHideTime = 3f;
    public float typingSpeed = 0.05f; // 打字速度

    [Header("Size Settings")]
    public Vector2 textPadding = new Vector2(20, 10);
    public float minWidth = 300f;
    public float maxWidth = 600f;
    public float widthK = 1.5f;
    public float heightK = 5f; // 文本高度的放大系数
    public string t = "这是一个测试对话气泡的内容,用于调试显示效果。";

    private Coroutine autoHideCoroutine;
    private Coroutine typingCoroutine;
    private Action pendingCallback;
    private string fullText; // 完整文本

    #region API
    // 显示对话气泡(世界坐标)
    public void ShowSpeak(string text, Vector3 worldPosition, Action onComplete = null)
    {
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        fullText = text;
        speakText.text = text; // 先设置完整文本用于计算尺寸
        AdjustPanelSize();
        transform.position = Camera.main.WorldToScreenPoint(worldPosition);
        // 清空文本
        speakText.text = "";
        Show();
        typingCoroutine = StartCoroutine(TypeText(onComplete));
    }

    // 显示对话气泡(屏幕坐标)
    public void ShowSpeakAtScreenPosition(string text, Vector2 screenPosition, Action onComplete = null)
    {
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        fullText = text;
        speakText.text = text; // 先设置完整文本用于计算尺寸
        AdjustPanelSize();
        transform.position = screenPosition;

        Show();
        typingCoroutine = StartCoroutine(TypeText(onComplete));
    }

    // 手动隐藏对话气泡
    public void HideSpeak(Action onComplete = null)
    {
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        pendingCallback = onComplete;

        Hide();
        StartCoroutine(WaitForFadeOutComplete());
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

    // 打字机效果
    private IEnumerator TypeText( Action onComplete = null)
    {
        // 等待淡入完成
        yield return new WaitForSecondsRealtime(fadeTime);

        // 逐字显示
        foreach (char c in fullText)
        {
            speakText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        // 打字完成,开始自动隐藏倒计时
        Debug.Log("打字完成,开始自动隐藏倒计时");
        autoHideCoroutine = StartCoroutine(AutoHideAfterDelay(onComplete));
    }

    // 等待淡出完成
    private IEnumerator WaitForFadeOutComplete()
    {
        yield return new WaitForSecondsRealtime(fadeTime);
        pendingCallback?.Invoke();
        pendingCallback = null;
    }

    // 自动隐藏
    private IEnumerator AutoHideAfterDelay(Action onComplete = null)
    {
        // 等待显示时间
        yield return new WaitForSecondsRealtime(autoHideTime);
        // 淡出
        Debug.Log("准备淡出");
        Hide(onComplete);
    }

    #endregion

    //便于调试,公开方法,右键组件调用
    [ContextMenu("Test Show Speak")]
    private void TestShowSpeak()
    {
        ShowSpeak(t, Vector3.zero);
    }
}