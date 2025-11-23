using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance { get; private set; }
    
    public int currentLevelIndex = 0;
    public List<Scene> scenes = new List<Scene>();
    
    private int CompletedScenesCount = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    public void LoadFinalLevel()
    {
        if (DataManager.Instance.CanReachBossRoom())
        {
            LoadFinalLevel();
        }
        else
        {
            LoadLevel(Random.Range(1,5));
        }
    }

    public void LoadLevel(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ReloadCurrentLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        currentLevelIndex = scene.buildIndex;
        
        DataManager.Instance.AddLevel(currentLevelIndex);

        Debug.Log($"已进入场景 {scene.name} (第 次)");
    }
}