using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    [Header("UI References")]
    public Image loadingImage;

    protected override void Awake()
    {
        base.Awake();
        // 自动加载黑幕图片
        if (loadingImage != null)
        {
            loadingImage.sprite = Resources.Load<Sprite>("CG/BlackScreen");
        }
    }

    public void ShowLoading(Action onComplete = null)
    {
        Show();
        if(onComplete!=null)
        {
            DOVirtual.DelayedCall(fadeTime, () =>
            {
                onComplete?.Invoke();
            }).SetUpdate(true);
        }
    }

    public void ShowLoading(float customFadeTime, Action onComplete = null)
    {
        float originalFadeTime = fadeTime;
        fadeTime = customFadeTime;

        if (onComplete == null)
        {
            Show();
        }
        else
        {
            Show();
            DOVirtual.DelayedCall(fadeTime, () => onComplete?.Invoke()).SetUpdate(true);
        }

        fadeTime = originalFadeTime;
    }

    public void HideLoading(Action onComplete = null)
    {
        Hide(onComplete);
    }
    public void HideLoading(float customFadeTime, Action onComplete = null)
    {
        float originalFadeTime = fadeTime;
        fadeTime = customFadeTime;
        Hide(onComplete);
        fadeTime = originalFadeTime;
    }
}
