using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : BasePanel
{
    [Header("UI References")]
    public Button resumeBtn;

    protected override void Awake()
    {
        base.Awake();
        // 绑定按钮事件
        resumeBtn.onClick.AddListener(OnResume);
    }

    private void OnResume()
    {
        Hide();

        Time.timeScale = 1f; // 恢复游戏时间
    }
}
