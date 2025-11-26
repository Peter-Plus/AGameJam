using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class GameCGPanel : BasePanel
{
    [Header("UI References")]
    public Image cgImage;

    private Tween autoHideTween;
    private Action pendingCallback;

    protected override void Awake()
    {
        base.Awake();
    }

    public void ShowCGInstant(Sprite sprite, float displayTime, Action onComplete = null)
    {
        autoHideTween?.Kill();

        cgImage.sprite = sprite;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        gameObject.SetActive(true);

        autoHideTween = DOVirtual.DelayedCall(displayTime, () => Hide(onComplete))
            .SetUpdate(true);
    }

    public void ShowCGInstant(Sprite sprite)
    {
        DOTween.Kill(this);

        cgImage.sprite = sprite;
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        gameObject.SetActive(true);
    }

    public void ShowCG(Sprite sprite, Action onComplete = null)
    {
        autoHideTween?.Kill();
        cgImage.sprite = sprite;
        pendingCallback = onComplete;

        Show();

        if (fadeTime > 0)
        {
            DOVirtual.DelayedCall(fadeTime, () =>
            {
                pendingCallback?.Invoke();
                pendingCallback = null;
            }).SetUpdate(true);
        }
        else
        {
            pendingCallback?.Invoke();
            pendingCallback = null;
        }
    }

    public void ShowCG(Sprite sprite, float displayTime, Action onComplete = null)
    {
        autoHideTween?.Kill();
        cgImage.sprite = sprite;

        Show();

        autoHideTween = DOVirtual.DelayedCall(fadeTime + displayTime, () => Hide(onComplete))
            .SetUpdate(true);
    }

    public void HideCG(Action onComplete = null)
    {
        autoHideTween?.Kill();
        Hide(onComplete);
    }

    public void HideCGInstant()
    {
        autoHideTween?.Kill();
        base.HideInstant();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        autoHideTween?.Kill();
    }
}