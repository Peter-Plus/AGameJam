using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Canvas Reference")]
    public Canvas mainCanvas;

    [Header("UI Layers")]
    public Transform gameUI;
    public Transform menuUI;
    public Transform dialogueUI;
    public Transform loadingUI;

    [Header("Dialogue UI")]
    public ChatPanel chatPanel; //对话面板
    public TipPanel tipPanel; //提示面板
    public TextTipPanel textTipPanel; //飘字提示面板

    [Header("Loading UI")]
    public LoadingPanel loadingPanel; //加载面板

    [Header("Game UI")]
    public HUDPanel HUDPanel; //游戏内HUD面板

    [Header("Menu UI")]
    public BeginPanel beginPanel; //开始菜单面板
    public SettingsPanel settingsPanel;
    public PausePanel pausePanel; //暂停菜单面板

    void Awake()
    {
        //单例模式
        Instance = this;
        DontDestroyOnLoad(gameObject);

        //初始化UI层级
        InitializeLayers();
    }

    void InitializeLayers()
    {
        if (mainCanvas == null)
        {
            Debug.LogError("UIManager: Canvas引用未配置!");
            return;
        }

        // 确保所有UI层存在
        if (gameUI == null) gameUI = CreateLayer("GameUI", 0);
        if (menuUI == null) menuUI = CreateLayer("MenuUI", 10);
        if (dialogueUI == null) dialogueUI = CreateLayer("DialogueUI", 20);
        if (loadingUI == null) loadingUI = CreateLayer("LoadingUI", 30);
    }

    Transform CreateLayer(string layerName, int sortOrder)
    {
        GameObject layer = new GameObject(layerName);
        layer.transform.SetParent(mainCanvas.transform, false);

        Canvas layerCanvas = layer.AddComponent<Canvas>();
        layerCanvas.overrideSorting = true;
        layerCanvas.sortingOrder = sortOrder;

        layer.AddComponent<GraphicRaycaster>();

        return layer.transform;
    }

    #region 对话框相关API
    /// <summary>
    /// 显示对话框
    /// </summary>
    public void ShowChat(string dialogue, Action onComplete = null)
    {
        if (chatPanel == null)
        {
            Debug.LogError("UIManager: ChatPanel未配置!");
            return;
        }
        chatPanel.ShowDialogue(dialogue, onComplete);
    }

    /// <summary>
    /// 显示对话框（带角色名）
    /// </summary>
    public void ShowChat(string dialogue, string characterName, Action onComplete = null)
    {
        if (chatPanel == null)
        {
            Debug.LogError("UIManager: ChatPanel未配置!");
            return;
        }
        chatPanel.ShowDialogue(dialogue, characterName, onComplete);
    }

    /// <summary>
    /// 显示对话框（带角色名和立绘）
    /// </summary>
    public void ShowChat(string dialogue, string characterName, Sprite characterSprite, Action onComplete = null)
    {
        if (chatPanel == null)
        {
            Debug.LogError("UIManager: ChatPanel未配置!");
            return;
        }
        chatPanel.ShowDialogue(dialogue, characterName, characterSprite, onComplete);
    }
    #endregion

    #region 提示框相关
    /// <summary>
    /// 显示提示框
    /// </summary>
    public void ShowTip(string tip, Action onConfirm = null)
    {
        if (tipPanel == null)
        {
            Debug.LogError("UIManager: TipPanel未配置!");
            return;
        }
        tipPanel.ShowTip(tip, onConfirm);
    }
    #endregion

    #region 飘字提示
    /// <summary>
    /// 显示飘字提示
    /// </summary>
    public void ShowTextTip(string text)
    {
        if (textTipPanel == null)
        {
            Debug.LogError("UIManager: TextTipPanel未配置!");
            return;
        }
        textTipPanel.ShowTip(text);
    }
    #endregion

    #region 加载界面
    /// <summary>
    /// 显示/隐藏加载界面
    /// </summary>
    public void ShowLoading(bool show)
    {
        //待开发
    }

    #endregion

    #region 游戏HUD面板
    /// <summary>
    /// 显示/隐藏游戏内HUD
    /// </summary>
    public void ShowGameUI(bool show)
    {
        //待开发
    }

    /// <summary>
    /// 更新血量显示
    /// </summary>
    public void UpdateHP()
    {
        //待开发
    }

    /// <summary>
    /// 更新等级显示
    /// </summary>
    public void UpdateLevel(int level)
    {
        //待开发
    }

    /// <summary>
    /// 更新血瓶数量显示
    /// </summary>
    public void UpdatePotion(int count)
    {
        //待开发
    }
    #endregion
}