using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance { get; private set; }

    //===================
    //主界面是MainScene，初始交互关卡是Level0,战斗关卡是Level1-Leveln，Boss房间是LevelBoss
    //按场景名称加载场景
    //===================

    private int currentLevelIndex = 0; //当前战斗关卡索引（注意这里不是场景索引，是关卡编号）,-1是Boss房间，0是初始交互关卡和主界面
    private int totalLevels = 0; //总关卡数，不包括Boss房间、主界面和初始交互界面
    //public List<Scene> scenes = new List<Scene>(); //已加载的场景列表

    #region 场景加载事件 - 存储已完成关卡
    // 注册场景加载事件
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 注销场景加载事件
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 处理场景加载完成事件-记录已完成关卡
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //获取通过关卡数
        totalLevels = DataManager.Instance.GetTotalLevelCount();
        if (scene == null) { return; }
        //如果是战斗关卡，记录当前关卡索引
        if (scene.name.StartsWith("Level") && scene.name != "LevelBoss" && scene.name != "Level0")
        {
            //提取关卡索引
            string indexStr = scene.name.Replace("Level", "");
            if (int.TryParse(indexStr, out int index))
            {
                currentLevelIndex = index;
            }
            //记录已完成的关卡
            DataManager.Instance.AddLevel(currentLevelIndex);
        }
        else if(scene.name == "Level0" || scene.name == "MainScene")
        {
            currentLevelIndex = 0; //非战斗关卡，重置索引
        }
        else if(scene.name == "LevelBoss")
        {
            currentLevelIndex = -1; //Boss关卡，特殊标记
        }
        else
        {
            currentLevelIndex = -2; //其他测试场景，特殊标记
        }
    }
    #endregion

    #region API
    //加载下一个关卡API
    public void LoadNextLevel()
    {
        if (DataManager.Instance.CanReachBossRoom())
        {
            LoadFinalLevel();
            //播放boss音乐
            AudioManager.Instance.PlayBossMusic();
        }
        else
        {
            LoadLevel(Random.Range(1, totalLevels + 1));//加载1到totalLevels之间的随机关卡
            //播放战斗音乐
            AudioManager.Instance.PlayBattleMusic();
        }
    }

    //加载初始交互关卡API
    public void LoadInitialLevel()
    {
        //按场景名加载初始交互关卡
        SceneManager.LoadScene("Level0");
    }

    //战斗失败返回主界面--================别被名字误导了，胜利也可以调用这个API==================
    public void FailAndReturnToMainMenu()
    {
        //按场景名加载主界面
        SceneManager.LoadScene("MainScene");
        //清空存档数据
        DataManager.Instance.ResetSaveData();
        //播放主界面音乐
        AudioManager.Instance.PlayerMainMusic();
    }

    //获取当前关卡索引API
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    #endregion

    #region 内部
    //加载最终boss关卡
    private void LoadFinalLevel()
    {
        //按场景名加载boss房间
        SceneManager.LoadScene("LevelBoss");
    }
    //加载战斗场景
    private void LoadLevel(int index)
    {
        //按场景名称加载场景
        SceneManager.LoadScene("Level" + index);
    }
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        totalLevels = DataManager.Instance.GetTotalLevelCount();
    }
    #endregion

}