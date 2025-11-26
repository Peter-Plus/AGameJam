using UnityEngine;
using System;
using DG.Tweening;

/// <summary>
/// 所有UI面板的基类，提供显示/隐藏/淡入淡出功能
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    [Header("Base Panel Settings")]
    [Tooltip("淡入淡出时间")]
    public float fadeTime = 0.3f;

    protected CanvasGroup canvasGroup;
    protected bool isShowing = false;
    protected Tween currentTween;// 当前的淡入淡出动画

    protected virtual void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        HideInstant();
    }

    // 显示面板（带淡入效果）
    public virtual void Show()
    {
        currentTween?.Kill();
        gameObject.SetActive(true);
        isShowing = true;
        if (fadeTime > 0)
        {
            currentTween = canvasGroup.DOFade(1f,fadeTime)
                .SetUpdate(true)
                .OnComplete(() =>
            {
                OnShow();
            });
        }
        else
        {
            canvasGroup.alpha = 1;
            OnShow();
        }
    }

    // 隐藏面板（带淡出效果）
    public virtual void Hide()
    {
        currentTween?.Kill();
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        isShowing = false;
        //先检查对象是否active，以避免重复隐藏时的问题
        if (!gameObject.activeSelf) return;

        if (fadeTime > 0)
        {
            currentTween = canvasGroup.DOFade(0f, fadeTime)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    OnHide();
                });
        }
        else
        {
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
            OnHide();
        }
    }
    //重载隐藏方法，带回调
    public virtual void Hide(Action onComplete)
    {
        currentTween?.Kill();
        isShowing = false;

        if (fadeTime > 0)
        {
            currentTween = canvasGroup.DOFade(0f, fadeTime)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                    gameObject.SetActive(false);
                });
        }
        else
        {
            canvasGroup.alpha = 0;
            onComplete?.Invoke();
            gameObject.SetActive(false);
        }
    }
    // 快速隐藏面板（无淡出效果）
    public void HideInstant()
    {
        currentTween?.Kill();
        isShowing = false;
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }
    //两个钩子
    protected virtual void OnShow() { }
    protected virtual void OnHide() { }
    //清理动画
    protected virtual void OnDestroy()
    {
        currentTween?.Kill();
    }
    [ContextMenu("显示开关")]
    private void ShowInDebug()
    {
        if (isShowing) Hide();
        else Show();
    }
}
