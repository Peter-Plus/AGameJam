using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public bool allowSkip = false;// 是否允许跳过交互
    //[Header("关卡参数相关")]
    ////关卡可否暂停
    ////public bool CanPause = true;
    ////直接通过单例InputManager控制可否暂停无需此变量
    //public bool canSkipInteract = true;// 是否允许跳过交互
    [Header("关卡敌人相关")]
    public List<Enemy> Enemys = new();
    private float levelTimer = 0f;// 关卡计时器
    protected bool isLevelActive = true;// 关卡是否处于活动状态
    public bool skipRequested = false;// 是否请求跳过交互


    #region 交互相关 每个关卡的交互内容不一样，待子类重载实现
    public virtual void StartInteract() { }
    #endregion

    #region 固定API
    public void TogglePause()
    {
        if (isLevelActive)
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
        Invoke(nameof(CallNextLevel), 2.0f);
    }
    // 关卡失败API
    public void LevelFailed()
    {
        isLevelActive = false;
        UIManager.Instance.ShowTip("关卡失败！", FailBack);
    }
    //UI交互开关API 禁止玩家操作、暂停计时
    public void SetUIInteract(bool open)
    {
        InputManager.Instance.SetAllowPlayerInput(!open);
        isLevelActive = !open;
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

    protected virtual void Update()//实现计时功能
    {
        if (isLevelActive)
        {
            levelTimer += Time.deltaTime;
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
    protected IEnumerator WaitForSecondsOrSkip(float seconds, System.Func<bool> skipCondition)
    {
        float elapsed = 0f;
        while (elapsed < seconds)
        {
            if (skipCondition()) yield break;
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
    #endregion
}