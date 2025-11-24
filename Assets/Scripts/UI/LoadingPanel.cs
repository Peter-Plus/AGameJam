using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : BasePanel
{
    [Header("UI References")]
    public Image loadingImage;

    protected override void Awake()
    {
        base.Awake();
        // ×Ô¶¯¼ÓÔØºÚÄ»Í¼Æ¬
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