using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameLevelManager : MonoBehaviour
{
    public static GameLevelManager Instance { get; private set; }
    
    public int currentLevelIndex = 0;
    public List<Scene> scenes = new List<Scene>();
    
    private Dictionary<string, int> completedScenes = new Dictionary<string, int>();
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
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (completedScenes.Count == 5 && CompletedScenesCount >= 10)
        {
            LoadFinalLevel();
        }
        else
        {
            LoadLevel(Random.Range(1,6));
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
        
        // 统计进入次数
        if (completedScenes.ContainsKey(scene.name))
            completedScenes[scene.name]++;
        else
            completedScenes.Add(scene.name, 1);

        Debug.Log($"已进入场景 {scene.name} (第 {completedScenes[scene.name]} 次)");
    }
}