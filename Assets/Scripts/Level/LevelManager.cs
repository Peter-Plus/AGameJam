using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [Header("关卡参数相关")]
    //关卡可否暂停
    public bool CanPause = true;
    [Header("关卡敌人相关")]
    public List<Enemy> Enemys = new();
    private float levelTimer = 0f;// 关卡计时器
    private bool isLevelActive = true;// 关卡是否处于活动状态

    #region 交互相关 每个关卡的交互内容不一样，待子类重载实现
    public virtual void StartInteract() { }
    #endregion

    #region 固定API
    // 检查胜利条件API
    public bool CheckWinCondition()
    {
        if (Enemys.Count == 0)
        {
            return true;
        }
        return false;
    }
    // 关卡完成API
    public void LevelComplete()
    {
        isLevelActive = false; // 停止计时等
        Debug.Log($"关卡完成！耗时: {levelTimer}秒");
        Invoke(nameof(CallNextLevel), 2.0f);
    }
    // 关卡失败API
    public void LevelFailed()
    {
        isLevelActive = false;
        UIManager.Instance.ShowTip("关卡失败！", FailBack);
    }
    #endregion

    #region 生命周期(Awake, Update)可重载
    protected virtual void Awake()
    {
        Instance = this;
        //隐藏加载界面
        UIManager.Instance.ShowLoadingPanel(false);
        //先隐藏HUD
        UIManager.Instance.ShowGameUI(false);
    }

    protected virtual void Update()//实现暂停按键监听、计时功能
    {
        if (isLevelActive)
        {
            levelTimer += Time.deltaTime;
        }
        if (CanPause && Input.GetKeyDown(KeyCode.P))
        {
            //UIManager.Instance.ShowPausePanel();
            //isLevelActive=false;
            if(isLevelActive)
            {
                UIManager.Instance.ShowPausePanel();
                isLevelActive = false;
                Time.timeScale = 0f;
            }
            else
            {
                UIManager.Instance.HidePausePanel();
                isLevelActive = true;
                Time.timeScale = 1f;
            }
        }
    }
    #endregion

    #region 内部
    private void FailBack()
    {
        UIManager.Instance.ShowLoadingPanel(true);
        //等待2秒后调用GameLevelManager的FailAndReturnToMainMenu方法
        Invoke(nameof(GameLevelManager.Instance.FailAndReturnToMainMenu), 2.0f);
    }

    private void CallNextLevel()
    {
        GameLevelManager.Instance.LoadNextLevel();
    }
    #endregion
}