using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

/// <summary>
/// CG插画面板,用于显示插图和黑幕,使用BasePanel的淡入淡出效果
/// </summary>
public class CGPanel : BasePanel
{
    [Header("UI References")]
    public Image cgImage;

    private Tween autoHideTween;// 自动隐藏的Tween
    private Action pendingCallback;// 待调用的回调函数

    #region API
    //不淡入但淡出的显示CG的方法
    public void ShowCGInstant(Sprite sprite,float displayTime, Action onComplete = null)
    {
        autoHideTween?.Kill();
        // 设置图片
        cgImage.sprite = sprite;
        // 不再通过基类的Show方法淡入
        // 直接设置透明度为1
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        //激活对象
        this.gameObject.SetActive(true);
        //等待显示时间后淡出
        autoHideTween = DOVirtual.DelayedCall(displayTime, () =>
        {
            HideCG(onComplete);// 淡出并调用回调
        }).SetUpdate(true);
    }

    public void ShowCGInstant(Sprite sprite)
    {
        autoHideTween?.Kill();
        // 设置图片
        cgImage.sprite = sprite;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        //激活对象
        this.gameObject.SetActive(true);
        //该操作不耗时间，不需要回调
    }

    // 显示CG图片,淡入完成后调用回调
    public void ShowCG(Sprite sprite, Action onComplete = null)
    {
        autoHideTween?.Kill();
        cgImage.sprite = sprite;
        pendingCallback = onComplete;
        Show();
        autoHideTween = DOVirtual.DelayedCall(fadeTime, () =>
        {
            pendingCallback?.Invoke();
            pendingCallback = null;
        }).SetUpdate(true);
    }

    // 显示CG图片,并在指定时间后自动隐藏
    public void ShowCG(Sprite sprite, float displayTime, Action onComplete = null)
    {
        autoHideTween?.Kill();
        cgImage.sprite = sprite;
        Show();
        autoHideTween = DOVirtual.DelayedCall(fadeTime + displayTime, () =>
        {
            HideCG(onComplete);// 淡出并调用回调
        }).SetUpdate(true);
    }

    // 隐藏CG(淡出),淡出完成后调用回调
    public void HideCG(Action onComplete = null)
    {
        autoHideTween?.Kill();
        Hide(onComplete);
    }

    // 立即隐藏CG
    public void HideCGInstant()
    {
        autoHideTween?.Kill();
        base.HideInstant();
    }
    #endregion

    #region 内部
    protected override void OnDestroy()
    {
        base.OnDestroy();
        autoHideTween?.Kill();
    }
    #endregion
}
