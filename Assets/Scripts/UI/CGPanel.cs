using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// CG插画面板,用于显示插图和黑幕,使用BasePanel的淡入淡出效果
/// </summary>
public class CGPanel : BasePanel
{
    [Header("UI References")]
    [Tooltip("用于显示CG图片的Image组件")]
    public Image cgImage;

    private Coroutine autoHideCoroutine;// 自动隐藏协程引用
    private Action pendingCallback;// 待调用的回调函数

    protected override void Awake()
    {
        base.Awake();
    }

    #region API
    //提供一个不淡入但淡出的显示CG的方法
    public void ShowCGInstant(Sprite sprite,float displayTime, Action onComplete = null)
    {
        // 停止之前的自动隐藏协程
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
        // 设置图片
        cgImage.sprite = sprite;
        // 直接设置透明度为1
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        //激活对象
        this.gameObject.SetActive(true);
        // 启动自动隐藏协程
        autoHideCoroutine = StartCoroutine(AutoHideAfterDelay(displayTime, onComplete));
    }


    // 显示CG图片,淡入完成后调用回调
    public void ShowCG(Sprite sprite, Action onComplete = null)
    {
        // 万一有，停止之前的自动隐藏协程
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
        // 设置图片
        cgImage.sprite = sprite;
        // 保存回调,在淡入完成后调用
        pendingCallback = onComplete;

        // 显示面板
        Show();
        StartCoroutine(WaitForFadeInComplete());
    }

    // 显示CG图片,并在指定时间后自动隐藏
    public void ShowCG(Sprite sprite, float displayTime, Action onComplete = null)
    {
        // 停止之前的自动隐藏协程
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
        // 设置图片
        cgImage.sprite = sprite;
        // 显示面板(使用BasePanel的淡入)
        Show();
        // 启动自动隐藏协程
        autoHideCoroutine = StartCoroutine(AutoHideAfterDelay(displayTime, onComplete));
    }

    // 隐藏CG(淡出),淡出完成后调用回调
    public void HideCG(Action onComplete = null)
    {
        // 停止自动隐藏协程
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
        // 保存回调
        pendingCallback = onComplete;

        // 隐藏面板(使用BasePanel的淡出)
        Hide();
        StartCoroutine(WaitForFadeOutComplete());
    }
    #endregion

    #region 内部
    // 等待淡入完成
    private IEnumerator WaitForFadeInComplete()
    {
        yield return new WaitForSecondsRealtime(fadeTime);
        pendingCallback?.Invoke();
        pendingCallback = null;
    }

    // 等待淡出完成
    private IEnumerator WaitForFadeOutComplete()
    {
        yield return new WaitForSecondsRealtime(fadeTime);
        pendingCallback?.Invoke();
        pendingCallback = null;
    }

    // 延迟后自动隐藏
    private IEnumerator AutoHideAfterDelay(float displayTime, Action onComplete = null)
    {
        Debug.Log($"开始自动隐藏协程, 显示时间: {displayTime}s");
        // 等待淡入完成
        yield return new WaitForSecondsRealtime(fadeTime);
        // 等待显示时间
        yield return new WaitForSecondsRealtime(displayTime);
        // 淡出并调用回调
        Hide(onComplete);
    }
    #endregion
}
