using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏数据单例管理器
/// 负责所有游戏数据的存取
/// </summary>
public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance => instance;

    #region 数据结构类
    //存档数据
    [System.Serializable]
    public class SaveData
    {
        public int hp = 100;
        public int maxhp = 100;
        //蓝条
        public int mp = 50;
        public int maxmp = 50;
        public int level = 1;//当前等级
        public int exp = 0;//当前经验值
        public int attack = 10;
        public int currentScene = 0;//玩家当前所在的关卡，0为第一个交互关卡，1为第一个战斗关卡，以此类推
        public int defense = 5;//防御力
        public List<int> levelList = new List<int>(); //（1,3,5）代表通过了1,3,5关卡
        public int passedLevelCount = 0;//经过的战斗关卡数（包括正在进行的战斗关卡）
        public int healthPotionCount = 3;//血瓶数量
    }
    //设置数据
    [System.Serializable]
    public class SettingData
    {
        public float musicVolume = 1;//音乐音量
        public float soundVolume = 1;//音效音量
    }
    //成就数据
    [System.Serializable]
    public class AchievementData
    {
        public bool isFirstClear = false;//是否第一次通关
        public bool isFirstBossKill = false;//是否第一次击杀Boss
        public int totalKillCount = 0;//总击杀数
    }
    #endregion

    // 数据实例
    private SaveData saveData = new SaveData();
    private SettingData settingData = new SettingData();
    private AchievementData achievementData = new AchievementData();

    // 其他参数
    //关卡总数(不含boss房和初始交互关卡)
    public const int totalLevelCount = 5;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);// 保持单例
        //重置本机数据开关：
        //SaveGameData();
        LoadAllData();// 加载所有数据
    }

    #region Set&Get
    //非存档数据API
    public int GetTotalLevelCount() => totalLevelCount;
    //存档数据API
    public int GetHp() => saveData.hp;
    public int GetMaxHp() => saveData.maxhp;
    public void SetHp(int value)
    {
        saveData.hp = value;
        SaveGameData();
        UIManager.Instance.HUDPanel.UpdateHpUI();
    }
    public void SetMaxHp(int value)
    {
        saveData.maxhp = value;
        SaveGameData();
        UIManager.Instance.HUDPanel.UpdateHpUI();
    }
    public int GetMp() => saveData.mp;
    public int GetMaxMp() => saveData.maxmp;
    public void SetMp(int value)
    {
        saveData.mp = value;
        SaveGameData();
        UIManager.Instance.HUDPanel.UpdateMpUI();
    }
    public void SetMaxMp(int value)
    {
        saveData.maxmp = value;
        SaveGameData();
        UIManager.Instance.HUDPanel.UpdateMpUI();
    }
    public int GetLevel() => saveData.level;
    public void SetLevel(int value)
    {
        saveData.level = value;
        SaveGameData();
        UIManager.Instance.HUDPanel.UpdateLevelUI();
    }

    public int GetExp() => saveData.exp;
    public void SetExp(int value)
    {
        saveData.exp = value;
        SaveGameData();
        UIManager.Instance.HUDPanel.UpdateExpUI();
    }

    public int GetAttack() => saveData.attack;
    public void SetAttack(int value)
    {
        saveData.attack = value;
        SaveGameData();
    }

    public int GetDefense() => saveData.defense;
    public void SetDefense(int value)
    {
        saveData.defense = value;
        SaveGameData();
    }

    public int GetHealthPotionCount() => saveData.healthPotionCount;
    public void SetHealthPotionCount(int value)
    {
        saveData.healthPotionCount = value;
        SaveGameData();
        UIManager.Instance.HUDPanel.UpdatePotionUI();
    }

    public int GetPassedLevelCount() => saveData.passedLevelCount;
    public void SetPassedLevelCount(int value)
    {
        saveData.passedLevelCount = value;
        SaveGameData();
    }
    public int GetCurrentScene() => saveData.currentScene;
    public void SetCurrentScene(int value)
    {
        saveData.currentScene = value;
        SaveGameData();
    }
    public List<int> GetLevelList() => new List<int>(saveData.levelList);
    public void SetLevelList(List<int> levelList)
    {
        saveData.levelList = new List<int>(levelList);
        SaveGameData();
    }

    //设置数据API
    public float GetMusicVolume() => settingData.musicVolume;
    public void SetMusicVolume(float value)
    {
        settingData.musicVolume = value;
        SaveSettingData();
    }

    public float GetSfxVolume() => settingData.soundVolume;
    public void SetSfxVolume(float value)
    {
        settingData.soundVolume = value;
        SaveSettingData();
    }
    //成就数据API
    public bool GetIsFirstClear() => achievementData.isFirstClear;
    public void SetIsFirstClear(bool value)
    {
        achievementData.isFirstClear = value;
        SaveAchievementData();
    }

    public bool GetIsFirstBossKill() => achievementData.isFirstBossKill;
    public void SetIsFirstBossKill(bool value)
    {
        achievementData.isFirstBossKill = value;
        SaveAchievementData();
    }

    public int GetTotalKillCount() => achievementData.totalKillCount;
    public void SetTotalKillCount(int value)
    {
        achievementData.totalKillCount = value;
        SaveAchievementData();
    }

    public void AddKillCount()
    {
        achievementData.totalKillCount++;
        SaveAchievementData();
    }
    #endregion

    #region 核心API
    /// <summary>
    /// 判断是否第一次到达某关卡
    /// </summary>
    public bool IsFirstTimeReachLevel(int levelIndex)
    {
        return !saveData.levelList.Contains(levelIndex);
    }

    /// <summary>
    /// 判断是否可以到达Boss房
    /// </summary>
    public bool CanReachBossRoom()
    {
        return saveData.levelList.Count == totalLevelCount && saveData.passedLevelCount > 10;
    }

    /// <summary>
    /// 添加关卡
    /// </summary>
    public void AddLevel(int levelIndex)
    {
        saveData.passedLevelCount++;//经过的关卡数加一
        //调试需要，遍历levelList
        // 添加到已通过关卡列表
        if (!saveData.levelList.Contains(levelIndex))
        {// 仅当关卡未存在时添加
            if (saveData.levelList.Count < totalLevelCount)
            {
                saveData.levelList.Add(levelIndex);// 添加关卡
            }
        }

        SaveGameData();
    }

    /// <summary>
    /// 重置存档数据
    /// </summary>
    public void ResetSaveData()
    {
        saveData = new SaveData();
        SaveGameData();
    }

    /// <summary>
    /// 重置所有数据
    /// </summary>
    public void ResetAllData()
    {
        PlayerPrefsMgr.Instance.DeleteAll();
        saveData = new SaveData();
        settingData = new SettingData();
        achievementData = new AchievementData();
    }
    #endregion


    #region 内部数据存取（这里无需关心）

    //Inspector右键可随时保存数据
    [ContextMenu("Save Game Data")]
    private void SaveGameData()
    {
        PlayerPrefsMgr.Instance.SaveObject(saveData, "SaveData");
    }

    private void SaveSettingData()
    {
        PlayerPrefsMgr.Instance.SaveObject(settingData, "SettingData");
    }

    private void SaveAchievementData()
    {
        PlayerPrefsMgr.Instance.SaveObject(achievementData, "AchievementData");
    }

    //第一次调用时加载所有数据
    private void LoadAllData()
    {
        if (PlayerPrefsMgr.Instance.HasObject("SaveData", typeof(SaveData)))
        {
            saveData = PlayerPrefsMgr.Instance.LoadObject<SaveData>("SaveData");
        }
        else
        {
            SaveGameData();
        }

        if (PlayerPrefsMgr.Instance.HasObject("SettingData", typeof(SettingData)))
        {
            settingData = PlayerPrefsMgr.Instance.LoadObject<SettingData>("SettingData");
        }
        else
        {
            SaveSettingData();
        }

        if (PlayerPrefsMgr.Instance.HasObject("AchievementData", typeof(AchievementData)))
        {
            achievementData = PlayerPrefsMgr.Instance.LoadObject<AchievementData>("AchievementData");
        }
        else
        {
            SaveAchievementData();
        }
    }
    public void SaveAll()
    {
        SaveGameData();
        SaveSettingData();
        SaveAchievementData();
    }
    #endregion

    #region 调试功能
    //在Inspector面板显示当前存档数据（仅调试用）
    [ContextMenu("Print Save Data")]
    private void PrintSaveData()
    {
        string levelListStr = string.Join(", ", saveData.levelList);
        Debug.Log($"HP: {saveData.hp}/{saveData.maxhp}, MP: {saveData.mp}/{saveData.maxmp}, Level: {saveData.level}, Exp: {saveData.exp}, Attack: {saveData.attack}, Defense: {saveData.defense}, CurrentScene: {saveData.currentScene}, PassedLevelCount: {saveData.passedLevelCount}, LevelList: [{levelListStr}], HealthPotions: {saveData.healthPotionCount}");
    }
    [ContextMenu("Print Setting Data")]
    private void PrintSettingData()
    {
        Debug.Log($"Music Volume: {settingData.musicVolume}, Sound Volume: {settingData.soundVolume}");
    }
    [ContextMenu("Print Achievement Data")]
    private void PrintAchievementData()
    {
        Debug.Log($"IsFirstClear: {achievementData.isFirstClear}, IsFirstBossKill: {achievementData.isFirstBossKill}, TotalKillCount: {achievementData.totalKillCount}");
    }
    //重置所有数据
    [ContextMenu("Reset All Data")]
    private void ResetAllDataContextMenu()
    {
        ResetAllData();
        Debug.Log("所有数据已重置");
    }

    //重置存档数据
    [ContextMenu("Reset Save Data")]
    private void ResetSaveDataContextMenu()
    {
        ResetSaveData();
        Debug.Log("存档数据已重置");
    }



    #endregion
}
