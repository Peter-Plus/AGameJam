using UnityEngine;
using UnityEngine.UI;
using System;

/// <summary>
/// 提示框面板，用于显示确认消息
/// </summary>
public class TipPanel : BasePanel
{
    [Header("UI References")]
    [Tooltip("提示文本")]
    public Text tipText;

    [Tooltip("确认按钮")]
    public Button confirmButton;

    [Tooltip("遮罩（可选）")]
    public Image maskImage;

    private Action onConfirmCallback;

    protected override void Awake()
    {
        base.Awake();

        // 绑定按钮事件
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmClick);
        }
    }

    /// <summary>
    /// 显示提示框
    /// </summary>
    public void ShowTip(string tip, Action onConfirm = null)
    {
        onConfirmCallback = onConfirm;

        // 设置提示文本
        if (tipText != null)
        {
            tipText.text = tip;
        }

        Show();
    }

    /// <summary>
    /// 点击确认按钮
    /// </summary>
    private void OnConfirmClick()
    {
        Hide();

        // 触发回调
        onConfirmCallback?.Invoke();
        onConfirmCallback = null;
    }

    protected override void OnShow()
    {
        // 启用遮罩（如果有）
        if (maskImage != null)
        {
            maskImage.gameObject.SetActive(true);
        }
    }

    protected override void OnHide()
    {
        // 禁用遮罩
        if (maskImage != null)
        {
            maskImage.gameObject.SetActive(false);
        }
    }
}