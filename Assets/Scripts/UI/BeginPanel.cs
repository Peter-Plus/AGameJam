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
        //根据存档判断玩家是新的游戏还是继续游戏
        //显示加载面板
        UIManager.Instance.ShowLoadingPanel(true);
        Hide(()=>
        {
            int passedLevelCount = DataManager.Instance.GetPassedLevelCount();
            if (passedLevelCount == 0) GameLevelManager.Instance.LoadInitialLevel(); //加载初始交互关卡
            else GameLevelManager.Instance.LoadNextLevel(); //加载下一个关卡 
        });
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
