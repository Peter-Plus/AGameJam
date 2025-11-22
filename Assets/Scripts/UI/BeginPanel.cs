using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    //获取组件 开始、设置、退出按钮
    [Header("UI References")]
    public Button BeginButton;
    public Button SettingsButton;
    public Button ExitButton;

    protected override void Awake()
    {
        base.Awake();
        //绑定按钮事件
        if (BeginButton != null)
        {
            BeginButton.onClick.AddListener(OnBeginClick);
        }
        if (SettingsButton != null)
        {
            SettingsButton.onClick.AddListener(OnSettingsClick);
        }
        if (ExitButton != null)
        {
            ExitButton.onClick.AddListener(OnExitClick);
        }
    }

    private void OnBeginClick()
    {
        //开始游戏逻辑
        Debug.Log("开始游戏--功能待开发--关联场景");
        //隐藏开始面板
        Hide();
        //进入游戏-SceneMgr-待开发
    }
    private void OnSettingsClick()
    {
        UIManager.Instance.settingsPanel.Show();
    }

    private void OnExitClick()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
