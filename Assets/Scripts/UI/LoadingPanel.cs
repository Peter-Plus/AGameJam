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

    public void ShowLoading()
    {
        Show();
    }

    public void HideLoading()
    {
        Hide();
    }
}
