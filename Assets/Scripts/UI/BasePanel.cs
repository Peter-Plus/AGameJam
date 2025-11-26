using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 所有UI面板的基类，提供显示/隐藏/淡入淡出功能
/// </summary>
public abstract class BasePanel : MonoBehaviour
{
    [Header("Base Panel Settings")]
    [Tooltip("淡入淡出时间")]
    public float fadeTime = 0.3f;

    [Tooltip("显示时是否暂停游戏")]
    private bool pauseGameWhenShow = false;//关闭面板暂停属性，不再用面板控制游戏暂停
                                           //面板时间相关用Time.unscaledDeltaTime和WaitForSecondsRealtime代替

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
        // 停止可能正在进行的淡出
        StopAllCoroutines();
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
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        isShowing = false;
        //先检查对象是否active，以避免重复隐藏时的问题
        if (gameObject.activeSelf == false)
        {
            return;
        }

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
    //重载隐藏方法，带回调
    public virtual void Hide(Action onComplete)
    {
        isShowing = false;

        if (pauseGameWhenShow)
        {
            Time.timeScale = 1;
        }

        if (fadeTime > 0)
        {
            Debug.Log("Starting FadeOutWithCallback coroutine.");
            StartCoroutine(FadeOutWithCallback(onComplete));
        }
        else
        {
            canvasGroup.alpha = 0;
            onComplete?.Invoke();
            gameObject.SetActive(false);
        }
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
    ///  带回调的淡出协程
    /// </summary>
    /// <param name="onComplete"></param>
    /// <returns></returns>
    protected IEnumerator FadeOutWithCallback(Action onComplete)
    {
        float elapsed = 0;
        while (elapsed < fadeTime)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeTime);
            yield return null;
        }
        canvasGroup.alpha = 0;
        Debug.Log("FadeOut complete, invoking callback and disabling panel.");
        onComplete?.Invoke(); // 先回调
        gameObject.SetActive(false); // 再禁用
    }
    /// <summary>
    /// 快速隐藏面板（无淡出效果）
    /// </summary>
    public void HideMe()
    {
        gameObject.SetActive(false);
    }

    //Q 下面这两个方法有什么用处？
    //A 这两个方法是为子类提供的钩子方法，允许子类在面板显示或隐藏时执行特定的逻辑。
    //例如，子类可以重写OnShow方法来初始化面板内容，或者重写OnHide方法来清理资源或保存状态。
    //Q 我最讨厌钩子你知道吗？
    /// <summary>
    /// 子类重写：面板显示时调用
    /// </summary>
    protected virtual void OnShow() { }
    /// <summary>
    /// 子类重写：面板隐藏时调用
    /// </summary>
    protected virtual void OnHide() { }


    //调试使用方法：（右键组件使用）
    [ContextMenu("Toggle Panel Visibility")]
    private void ShowInDebug()
    {
        if (isShowing)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
}
