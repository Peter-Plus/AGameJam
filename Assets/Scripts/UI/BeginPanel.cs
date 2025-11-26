using DG.Tweening;
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
    public Text Title;
    public Text TitleBox;

    [Header("Animation Settings")]
    public float buttonMoveDistance = 200f;
    public float titleMoveDistance = 200f;
    public float moveAnimDuration = 0.5f;// 按钮和标题移动动画时间
    public float bgScaleDuration = 1f;// 背景缩放时间=加载时间
    public float bgTargetScale = 1.2f;

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
        // 禁用按钮避免重复点击
        BeginButton.interactable = false;
        SettingsButton.interactable = false;
        ExitButton.interactable = false;
        Sequence seq = DOTween.Sequence();
        // 标题向上移动
        if (Title != null)
            seq.Join(Title.transform.DOLocalMoveY(Title.transform.localPosition.y + titleMoveDistance, moveAnimDuration));
        if (TitleBox != null)
            seq.Join(TitleBox.transform.DOLocalMoveY(TitleBox.transform.localPosition.y + titleMoveDistance, moveAnimDuration));

        // 按钮父物体向下，这里图片本身是按钮的父物体
        seq.Join(BeginButton.transform.parent.DOLocalMoveY(BeginButton.transform.parent.localPosition.y - buttonMoveDistance, moveAnimDuration));
        seq.Join(SettingsButton.transform.parent.DOLocalMoveY(SettingsButton.transform.parent.localPosition.y - buttonMoveDistance, moveAnimDuration));
        seq.Join(ExitButton.transform.parent.DOLocalMoveY(ExitButton.transform.parent.localPosition.y - buttonMoveDistance, moveAnimDuration));
        
        seq.OnComplete(() =>
        {
            MainManager mgr = MainManager.Instance as MainManager;
            if(mgr != null) mgr.ScaleBackground(bgTargetScale, bgScaleDuration);

            UIManager.Instance.ShowLoadingPanel(true,bgScaleDuration, () =>
            {
                Hide(() =>
                {
                    int passedLevelCount = DataManager.Instance.GetPassedLevelCount();
                    if (passedLevelCount == 0) GameLevelManager.Instance.LoadInitialLevel();
                    else GameLevelManager.Instance.LoadNextLevel();
                });
            });
        });
    }
    private void OnSettingsClick()
    {
        UIManager.Instance.ShowSettingsPanel();
    }

    private void OnExitClick()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
}
