using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsPanel : BasePanel
{
    [Header("UI References")]
    public Slider musicSlider;
    public Slider soundSlider;
    public Button closeBtn;
    public Image maskImage; // 遮罩

    [Header("Animation Settings")]
    public float popScale = 0.8f;
    public float animDuration = 0.3f;

    private RectTransform panelRect;

    protected override void Awake()
    {
        base.Awake();
        panelRect = GetComponent<RectTransform>();

        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        soundSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        closeBtn.onClick.AddListener(OnClose);
    }

    public override void Show()
    {
        // 遮罩淡入
        if (maskImage != null)
        {
            maskImage.gameObject.SetActive(true);
            var maskCG = maskImage.GetComponent<CanvasGroup>();
            if (maskCG == null) maskCG = maskImage.gameObject.AddComponent<CanvasGroup>();
            maskCG.alpha = 0;
            maskCG.DOFade(1f, fadeTime).SetUpdate(true);
        }

        // 面板弹出动画
        gameObject.SetActive(true);
        isShowing = true;
        panelRect.localScale = Vector3.one * popScale;
        canvasGroup.alpha = 0;

        panelRect.DOScale(1f, animDuration).SetEase(Ease.OutBack).SetUpdate(true);
        canvasGroup.DOFade(1f, animDuration).SetUpdate(true).OnComplete(() => OnShow());
    }

    public override void Hide()
    {
        currentTween?.Kill();
        isShowing = false;

        // 面板缩小动画
        panelRect.DOScale(popScale, animDuration).SetEase(Ease.InBack).SetUpdate(true);
        canvasGroup.DOFade(0f, animDuration).SetUpdate(true).OnComplete(() =>
        {
            gameObject.SetActive(false);
            OnHide();
        });

        // 遮罩淡出
        if (maskImage != null)
        {
            var maskCG = maskImage.GetComponent<CanvasGroup>();
            if (maskCG != null)
            {
                maskCG.DOFade(0f, fadeTime).SetUpdate(true)
                    .OnComplete(() => maskImage.gameObject.SetActive(false));
            }
        }
    }

    protected override void OnShow()
    {
        musicSlider.value = DataManager.Instance.GetMusicVolume();
        soundSlider.value = DataManager.Instance.GetSfxVolume();
    }

    private void OnMusicVolumeChanged(float value)
    {
        DataManager.Instance.SetMusicVolume(value);
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        DataManager.Instance.SetSfxVolume(value);
        AudioManager.Instance.SetSfxVolume(value);
    }

    private void OnClose()
    {
        Hide();
    }
}