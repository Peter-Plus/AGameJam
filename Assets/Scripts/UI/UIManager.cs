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
    public SpeakPanel speakPanel; //对话气泡面板

    [Header("Loading UI")]
    public LoadingPanel loadingPanel; //加载面板
    public CGPanel cgPanel; //CG插画面板

    [Header("Game UI")]
    public HUDPanel HUDPanel; //游戏内HUD面板
    public GameCGPanel gameCGPanel; //游戏CG面板 层级更低

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
    public void ShowLoadingPanel(bool show)
    {
        if (show)
        {
            loadingPanel.ShowLoading();
        }
        else
        {
            loadingPanel.HideLoading();
        }
    }

    #endregion

    #region 游戏HUD面板
    /// <summary>
    /// 显示/隐藏游戏内HUD
    /// </summary>
    public void ShowGameUI(bool show)
    {
        if (HUDPanel == null)
        {
            Debug.LogError("UIManager: HUDPanel未配置!");
            return;
        }

        if (show)
        {
            HUDPanel.Show();
        }
        else
        {
            HUDPanel.Hide();
        }
    }

    /// <summary>
    /// 更新血量显示
    /// </summary>
    public void UpdateHpUI()
    {
        if (HUDPanel == null) return;
        HUDPanel.UpdateHpUI();
    }

    /// <summary>
    /// 更新蓝量显示
    /// </summary>
    public void UpdateMpUI()
    {
        if (HUDPanel == null) return;
        HUDPanel.UpdateMpUI();
    }

    /// <summary>
    /// 更新经验显示
    /// </summary>
    public void UpdateExpUI()
    {
        if (HUDPanel == null) return;
        HUDPanel.UpdateExpUI();
    }

    /// <summary>
    /// 更新等级显示
    /// </summary>
    public void UpdateLevelUI()
    {
        if (HUDPanel == null) return;
        HUDPanel.UpdateLevelUI();
    }

    /// <summary>
    /// 更新血瓶数量显示
    /// </summary>
    public void UpdatePotionUI()
    {
        if (HUDPanel == null) return;
        HUDPanel.UpdatePotionUI();
    }

    /// <summary>
    /// 使用血瓶时调用
    /// </summary>
    public void OnUsedPotion()
    {
        if (HUDPanel == null) return;
        HUDPanel.OnUsedPotion();
    }
    #endregion

    #region 游戏菜单面板
    //显示设置面板
    public void ShowSettingsPanel()
    {
        settingsPanel.Show();
    }
    //显示暂停面板
    public void ShowPausePanel()
    {
        pausePanel.Show();
    }
    //显示开始面板
    public void ShowBeginPanel()
    {
        beginPanel.Show();
    }
    //隐藏所有菜单面板
    public void HideAllMenuPanels()
    {
        settingsPanel.Hide();
        pausePanel.Hide();
        beginPanel.Hide();
    }
    //隐藏设置面板
    public void HideSettingsPanel()
    {
        settingsPanel.Hide();
    }
    public void HidePausePanel()
    {
        pausePanel.Hide();
    }
    public void HideBeginPanel()
    {
        beginPanel.Hide();
    }
    #endregion

    #region CG面板相关
    public void ShowCGPanel(Sprite cgSprite, Action onComplete = null)
    {
        cgPanel.ShowCG(cgSprite, onComplete);
    }

    public void ShowCGPanel(Sprite cgSprite, float displayTime, Action onComplete = null)
    {
        cgPanel.ShowCG(cgSprite, displayTime, onComplete);
    }

    public void HideCGPanel(Action onComplete = null)
    {
        cgPanel.HideCG(onComplete);
    }

    public void ShowCGPanelInstant(Sprite cgSprite, float displayTime, Action onComplete = null)
    {
        cgPanel.ShowCGInstant(cgSprite, displayTime, onComplete);
    }
    #endregion

    #region 游戏CG面板相关

    public void ShowGameCGPanel(Sprite cgSprite, Action onComplete = null)
    {
        Debug.Log("UIManager: ShowGameCGPanel called");
        gameCGPanel.ShowCG(cgSprite, onComplete);
    }
    public void ShowGameCGPanel(Sprite cgSprite, float displayTime, Action onComplete = null)
    {
        gameCGPanel.ShowCG(cgSprite, displayTime, onComplete);
    }
    public void HideGameCGPanel(Action onComplete = null)
    {
        gameCGPanel.HideCG(onComplete);
    }
    public void ShowGameCGPanelInstant(Sprite cgSprite, float displayTime, Action onComplete = null)
    {
        gameCGPanel.ShowCGInstant(cgSprite, displayTime, onComplete);
    }
    public void ShowGameCGPanelInstant(Sprite cgSprite,  Action onComplete = null)
    {
        gameCGPanel.ShowCGInstant(cgSprite, onComplete);
    }
    #endregion

    #region 对话泡泡相关
    public void ShowSpeakPanel(string text, Vector3 worldPosition, Action onComplete = null)
    {
        speakPanel.ShowSpeak(text, worldPosition, onComplete);
    }

    public void ShowSpeakPanelAtScreen(string text, Vector2 screenPosition, Action onComplete = null)
    {
        speakPanel.ShowSpeakAtScreenPosition(text, screenPosition, onComplete);
    }

    public void HideSpeakPanel(Action onComplete = null)
    {
        speakPanel.HideSpeak(onComplete);
    }
    #endregion
}