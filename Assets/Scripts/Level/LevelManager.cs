using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public List<Enemy> Enemys = new();
    public float levelTimer = 0f;
    
    private bool isLevelActive = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Update()
    {
        if (isLevelActive)
        {
            levelTimer += Time.deltaTime;
        }
    }
    
    private bool CheckWinCondition()
    {
        if (Enemys.Count == 0)
        {
            return true;
        }
        return false;
    }
    
    public void LevelComplete()
    {
        if (!isLevelActive) return;
        isLevelActive = false; // 停止计时等

        Debug.Log($"关卡完成！耗时: {levelTimer}秒");
        
        Invoke(nameof(CallNextLevel), 2.0f);
    }
    
    public void LevelFailed()
    {
        isLevelActive = false;
        Debug.Log("关卡失败！");
        GameLevelManager.Instance.ReloadCurrentLevel();
    }
    
    private void CallNextLevel()
    {
        GameLevelManager.Instance.LoadLevel(Random.Range(1,5));
    }
}