using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : BasePanel
{
    [Header("UI References")]
    public Slider musicSlider;
    public Slider soundSlider;
    public Button closeBtn;

    protected override void Awake()
    {
        base.Awake();

        // 绑定滑动条事件
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        soundSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // 绑定按钮事件
        closeBtn.onClick.AddListener(OnClose);
    }

    protected override void OnShow()
    {
        // 显示时从DataManager读取保存的音量值
        musicSlider.value = DataManager.Instance.GetMusicVolume();
        soundSlider.value = DataManager.Instance.GetSfxVolume();
    }

    // 音乐音量改变
    private void OnMusicVolumeChanged(float value)
    {
        // 保存到DataManager
        DataManager.Instance.SetMusicVolume(value);

        // 实时应用到AudioSource
        AudioManager.Instance.SetMusicVolume(value);
    }

    // 音效音量改变
    private void OnSFXVolumeChanged(float value)
    {
        DataManager.Instance.SetSfxVolume(value);
        AudioManager.Instance.SetSfxVolume(value);
    }

    // 关闭按钮
    private void OnClose()
    {
        Hide();
    }

}