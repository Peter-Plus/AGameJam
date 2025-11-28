using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文字提示面板 —— 显示后自动消失
/// </summary>
public class TextTipPanel : BasePanel
{
    [Tooltip("提示文字（显示在内容区域）")]
    public Text tipText;

    [Tooltip("提示外框上的文字（如果有底框）")]
    public Text tipTextBox;

    [Tooltip("显示持续时间")]
    public float displayDuration = 2f;
    [Header("字体设置")]
    [SerializeField] private int defaultFontSize = 80; // 默认字体大小
    private Coroutine hideCoroutine;

    // 显示提示文字 - 默认字体大小
    public void ShowTip(string text)
    {
        ShowTip(text, defaultFontSize);
    }

    // 显示提示文字 - 自定义字体大小
    public void ShowTip(string text, int fontSize)
    {
        tipText.text = text;
        tipText.fontSize = fontSize;
        tipTextBox.text = text;
        tipTextBox.fontSize = fontSize+1;
        Show();
        // 若已有计时协程，先停止
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        // 开始自动隐藏
        hideCoroutine = StartCoroutine(AutoHide());
    }

    // 自动隐藏协程
    private IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(displayDuration);
        Hide();
        hideCoroutine = null;
    }
}
