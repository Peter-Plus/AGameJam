using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region 单例
    public static InputManager Instance;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    #region 变量
    [SerializeField] private bool allowPlayerInput = true;//是否允许玩家输入
    [SerializeField] private bool allowUIInput = true;//是否允许UI输入
    [SerializeField] private bool showDebugInfo = false;//是否显示调试信息
    private Camera mainCamera;//主摄像机引用
    #endregion

    #region API

    //玩家输入相关
    //玩家移动输入相对敏感，在玩家脚本里做按键监听和输入处理
    //玩家是否可以移动
    public bool CanPlayerMove()
    {
        if(Time.timeScale == 0) return false;
        if(!allowPlayerInput) return false;
        return true;
    }
    //玩家战斗相关
    //获取普攻
    public bool GetAttackInput()
    {
        if(!CanPlayerInput()) return false;
        return Input.GetKeyDown(KeyCode.J);// 按J键普攻
    }

    //技能输入
    public bool GetSkillInput()
    {
        if(!CanPlayerInput()) return false;
        return Input.GetKeyDown(KeyCode.Y);// 按Y键释放技能
    }

    //血瓶使用输入 
    public bool GetHealthPotionInput()
    {
        if(!CanPlayerInput()) return false;
        return Input.GetKeyDown(KeyCode.H);// 按H键使用血瓶
    }
    //暂停游戏输入 P键暂停
    public bool GetPauseInput()
    {
        if (!CanPlayerInput()) return false;
        return Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape);
    }

    //UI交互输入


    //跳过对话输入 空格键跳过
    public bool GetSkipInput()
    {
        if (!allowUIInput) return false;
        return Input.GetKeyDown(KeyCode.Space);
    }

    //设置允许玩家输入
    public void SetAllowPlayerInput(bool allow)
    {
        allowPlayerInput = allow;
    }
    //设置允许UI输入
    public void SetAllowUIInput(bool allow)
    {
        allowUIInput = allow;
    }
    #endregion

    #region 生命周期
    private void Start()
    {
        mainCamera = Camera.main;
    }
    void Update()
    {
        // 暂停输入
        HandlePauseInput();
        HandleSkipInput();
        // 物品点击
        DetectItemClick();
    }
    #endregion

    #region 内部
    private bool CanPlayerInput()
    {
        if (Time.timeScale == 0) return false;
        if (!allowPlayerInput) return false;
        return true;
    }

    private void HandlePauseInput()
    {
        if (!allowUIInput) return;
        if (!Input.GetKeyDown(KeyCode.P) && !Input.GetKeyDown(KeyCode.Escape)) return;

        // 调用 LevelManager 的暂停逻辑
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.TogglePause();
        }
    }
    private void HandleSkipInput()
    {
        if (!allowUIInput) return;
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (LevelManager.Instance != null && LevelManager.Instance.allowSkip)
        {
            LevelManager.Instance.skipRequested = true;
        }
    }

    private void DetectItemClick()
    {
        if(!CanPlayerInput()) return;
        if(!Input.GetMouseButtonDown(0)) return;
        //可能切换场景后mainCamera会变，需要重新获取
        if(mainCamera == null)
        {
            mainCamera = Camera.main;
            if(mainCamera == null)
            {
                Debug.LogWarning("InputManager: 主摄像机未找到，无法检测物品点击。");
                return;
            }
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            InteractItem item = hit.collider.GetComponent<InteractItem>();
            if(item != null)
            {
                item.TryInteract();
            }
        }
    }
    #endregion

    #region 调试功能
    private void OnGUI()
    {
        if (!showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label($"Player Input: {allowPlayerInput}");
        GUILayout.Label($"UI Input: {allowUIInput}");
        GUILayout.Label($"Time Scale: {Time.timeScale}");
        GUILayout.Label($"Can Move: {CanPlayerMove()}");
        GUILayout.EndArea();
    }
    #endregion
}
