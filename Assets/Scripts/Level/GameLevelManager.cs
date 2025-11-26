using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance { get; private set; }

    //===================
    //涓荤晫闈㈡槸MainScene锛屽垵濮嬩氦浜掑叧鍗℃槸Level0,鎴樻枟鍏冲崱鏄疞evel1-Leveln锛孊oss鎴块棿鏄疞evelBoss
    //鎸夊満鏅悕绉板姞杞藉満鏅?
    //===================

    private int currentLevelIndex = 0; //褰撳墠鎴樻枟鍏冲崱绱㈠紩锛堟敞鎰忚繖閲屼笉鏄満鏅储寮曪紝鏄叧鍗＄紪鍙凤級,-1鏄疊oss鎴块棿锛?鏄垵濮嬩氦浜掑叧鍗″拰涓荤晫闈?
    private int totalLevels = 0; //鎬诲叧鍗℃暟锛屼笉鍖呮嫭Boss鎴块棿銆佷富鐣岄潰鍜屽垵濮嬩氦浜掔晫闈?
    //public List<Scene> scenes = new List<Scene>(); //宸插姞杞界殑鍦烘櫙鍒楄〃

    #region 鍦烘櫙鍔犺浇浜嬩欢 - 瀛樺偍宸插畬鎴愬叧鍗?
    // 娉ㄥ唽鍦烘櫙鍔犺浇浜嬩欢
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 娉ㄩ攢鍦烘櫙鍔犺浇浜嬩欢
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 澶勭悊鍦烘櫙鍔犺浇瀹屾垚浜嬩欢-璁板綍宸插畬鎴愬叧鍗?
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //鑾峰彇閫氳繃鍏冲崱鏁?
        totalLevels = DataManager.Instance.GetTotalLevelCount();
        if (scene == null) { return; }
        //濡傛灉鏄垬鏂楀叧鍗★紝璁板綍褰撳墠鍏冲崱绱㈠紩
        if (scene.name.StartsWith("Level") && scene.name != "LevelBoss" && scene.name != "Level0")
        {
            //鎻愬彇鍏冲崱绱㈠紩
            string indexStr = scene.name.Replace("Level", "");
            if (int.TryParse(indexStr, out int index))
            {
                currentLevelIndex = index;
            }
            //璁板綍宸插畬鎴愮殑鍏冲崱
            DataManager.Instance.AddLevel(currentLevelIndex);
        }
        else if(scene.name == "Level0" || scene.name == "MainScene")
        {
            currentLevelIndex = 0; //闈炴垬鏂楀叧鍗★紝閲嶇疆绱㈠紩
        }
        else if(scene.name == "LevelBoss")
        {
            currentLevelIndex = -1; //Boss鍏冲崱锛岀壒娈婃爣璁?
        }
        else
        {
            currentLevelIndex = -2; //鍏朵粬娴嬭瘯鍦烘櫙锛岀壒娈婃爣璁?
        }
    }
    #endregion

    #region API
    //鍔犺浇涓嬩竴涓叧鍗PI
    public void LoadNextLevel()
    {
        if (DataManager.Instance.CanReachBossRoom())
        {
            LoadFinalLevel();
            //鎾斁boss闊充箰
            AudioManager.Instance.PlayBossMusic();
        }
        else
        {
            LoadLevel(Random.Range(1, totalLevels + 1));//鍔犺浇1鍒皌otalLevels涔嬮棿鐨勯殢鏈哄叧鍗?
            //鎾斁鎴樻枟闊充箰
            AudioManager.Instance.PlayBattleMusic();
        }
    }

    //鍔犺浇鍒濆浜や簰鍏冲崱API
    public void LoadInitialLevel()
    {
        //鎸夊満鏅悕鍔犺浇鍒濆浜や簰鍏冲崱
        SceneManager.LoadScene("Level0");
    }

    //鎴樻枟澶辫触杩斿洖涓荤晫闈?-================鍒鍚嶅瓧璇浜嗭紝鑳滃埄涔熷彲浠ヨ皟鐢ㄨ繖涓狝PI==================
    public void FailAndReturnToMainMenu()
    {
        //鎸夊満鏅悕鍔犺浇涓荤晫闈?
        SceneManager.LoadScene("MainScene");
        //娓呯┖瀛樻。鏁版嵁
        DataManager.Instance.ResetSaveData();
        //鎾斁涓荤晫闈㈤煶涔?
        AudioManager.Instance.PlayerMainMusic();
    }

    //鑾峰彇褰撳墠鍏冲崱绱㈠紩API
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    #endregion

    #region 鍐呴儴
    //鍔犺浇鏈€缁坆oss鍏冲崱
    private void LoadFinalLevel()
    {
        //鎸夊満鏅悕鍔犺浇boss鎴块棿
        SceneManager.LoadScene("LevelBoss");
    }
    //鍔犺浇鎴樻枟鍦烘櫙
    private void LoadLevel(int index)
    {
        //鎸夊満鏅悕绉板姞杞藉満鏅?
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
