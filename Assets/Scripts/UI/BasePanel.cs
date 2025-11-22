using UnityEngine;
using System.Collections;

/// <summary>
/// 所有UI面板的基类，提供显示/隐藏/淡入淡出功能
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    [Header("Base Panel Settings")]
    [Tooltip("淡入淡出时间")]
    public float fadeTime = 0.3f;

    [Tooltip("显示时是否暂停游戏")]
    public bool pauseGameWhenShow = false;

    protected CanvasGroup canvasGroup;
    protected bool isShowing = false;

    protected virtual void Awake()
    {
        // 自动添加CanvasGroup组件用于淡入淡出
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 初始化时隐藏面板
        HideMe();
    }

    /// <summary>
    /// 显示面板（带淡入效果）
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
        isShowing = true;

        if (pauseGameWhenShow)
        {
            Time.timeScale = 0;
        }

        if (fadeTime > 0)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            canvasGroup.alpha = 1;
        }

        OnShow();
    }

    /// <summary>
    /// 隐藏面板（带淡出效果）
    /// </summary>
    public virtual void Hide()
    {
        isShowing = false;

        if (pauseGameWhenShow)
        {
            Time.timeScale = 1;
        }

        if (fadeTime > 0)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }

        OnHide();
    }

    /// <summary>
    /// 淡入协程
    /// </summary>
    protected IEnumerator FadeIn()
    {
        float elapsed = 0;
        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    /// <summary>
    /// 淡出协程
    /// </summary>
    protected IEnumerator FadeOut()
    {
        float elapsed = 0;
        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 快速隐藏面板（无淡出效果）
    /// </summary>
    public void HideMe()
    {
        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 子类重写：面板显示时调用
    /// </summary>
    protected virtual void OnShow() { }

    /// <summary>
    /// 子类重写：面板隐藏时调用
    /// </summary>
    protected virtual void OnHide() { }
}