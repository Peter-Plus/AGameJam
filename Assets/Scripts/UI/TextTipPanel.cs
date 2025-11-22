using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 文本提示面板 - 显示后自动消失
/// </summary>
public class TextTipPanel : BasePanel
{
    public Text tipText;
    public float displayDuration = 2f; // 显示持续时间

    private Coroutine hideCoroutine;

    /// <summary>
    /// 显示提示文本
    /// </summary>
    public void ShowTip(string text)
    {
        tipText.text = text;
        Show(); 

        // 如果有正在执行的隐藏协程，先停止
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // 启动自动隐藏
        hideCoroutine = StartCoroutine(AutoHide());
    }

    private IEnumerator AutoHide()
    {
        yield return new WaitForSeconds(displayDuration);
        Hide(); 
        hideCoroutine = null;
    }
}