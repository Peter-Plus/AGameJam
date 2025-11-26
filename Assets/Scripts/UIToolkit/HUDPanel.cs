using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

/// <summary>
/// HUD面板  使用UI Toolkit实现
/// UITOOKIT!!!!!!!!!!!!!!!!!!!!!!!!!!!!
/// </summary>
public class HUDPanel : MonoBehaviour
{
    [Header("UI Document")]
    public UIDocument uiDocument;

    // UI元素引用
    private VisualElement root;
    private Label levelLabel;
    private Label hpLabel;
    private Label mpLabel;
    private Label expLabel;
    private VisualElement hpBarFill;
    private VisualElement mpBarFill;
    private VisualElement expBarFill;
    private VisualElement potionIcon;
    private Label potionCount;
    private VisualElement potionCooldown;
    private Label skillCooldownText;
    private VisualElement skillCooldownOverlay;

    // 冷却相关
    private bool isVisible = false; // HUD是否可见
    private PlayerCore registeredPlayer = null;


    #region 更新UI方法

    /// <summary>
    /// 显示HUD
    /// </summary>
    [ContextMenu("show")]
    public void Show()
    {
        if (root == null) return;

        root.style.display = DisplayStyle.Flex;
        isVisible = true;

        // 从DataManager获取数据并更新UI
        RefreshAllUI();

    }

    /// <summary>
    /// 隐藏HUD
    /// </summary>
    public void Hide()
    {
        if (root == null) return;

        root.style.display = DisplayStyle.None;
        isVisible = false;
        StopAllCoroutines();
    }

    /// <summary>
    /// 更新血条UI
    /// </summary>
    public void UpdateHpUI()
    {
        if (DataManager.Instance == null) return;

        int hp = DataManager.Instance.GetHp();
        int maxHp = DataManager.Instance.GetMaxHp();

        hpLabel.text = $"{hp}/{maxHp}";

        float hpPercent = maxHp > 0 ? (float)hp / maxHp : 0f;
        hpBarFill.style.width = Length.Percent(hpPercent * 100);
    }

    /// <summary>
    /// 更新蓝条UI
    /// </summary>
    public void UpdateMpUI()
    {
        if (DataManager.Instance == null) return;

        int mp = DataManager.Instance.GetMp();
        int maxMp = DataManager.Instance.GetMaxMp();

        mpLabel.text = $"{mp}/{maxMp}";

        float mpPercent = maxMp > 0 ? (float)mp / maxMp : 0f;
        mpBarFill.style.width = Length.Percent(mpPercent * 100);
    }

    /// <summary>
    /// 更新经验条UI
    /// </summary>
    public void UpdateExpUI()
    {
        if (DataManager.Instance == null) return;

        int exp = DataManager.Instance.GetExp();
        int level = DataManager.Instance.GetLevel();
        int maxExp = level * 20 + 80;

        expLabel.text = $"{exp}/{maxExp}";

        float expPercent = maxExp > 0 ? (float)exp / maxExp : 0f;
        expBarFill.style.width = Length.Percent(expPercent * 100);
    }

    /// <summary>
    /// 更新等级UI
    /// </summary>
    public void UpdateLevelUI()
    {
        if (DataManager.Instance == null) return;

        int level = DataManager.Instance.GetLevel();
        levelLabel.text = $"Lv.{level}";
    }

    /// <summary>
    /// 更新血瓶UI
    /// </summary>
    public void UpdatePotionUI()
    {
        if (DataManager.Instance == null) return;

        int count = DataManager.Instance.GetHealthPotionCount();
        potionCount.text = count.ToString();
    }
    #endregion

    #region 技能冷却相关
    //注册玩家信息
    public void RegisterPlayer(PlayerCore player)
    {
        if (player == null) return;
        registeredPlayer = player;
    }

    private void Update()
    {
        if (!isVisible || registeredPlayer == null) return;
        // 更新技能冷却
        UpdateSkillCDUI();
        UpdatePotionCDUI();
    }

    private void UpdateSkillCDUI()
    {
        float remaining = registeredPlayer.GetSkillCooldownRemaining();//获取剩余冷却时间
        float duration = registeredPlayer.GetSkillCooldownDuration();// 获取冷却总时间

        if (remaining > 0)
        {
            skillCooldownText.text = Mathf.CeilToInt(remaining).ToString();// 显示剩余时间（向上取整）
            //Mathf.CeilToInt()这个API的作用是将一个浮点数向上取整为最接近的整数。
            skillCooldownOverlay.style.height = Length.Percent((remaining / duration) * 100);// 更新遮罩高度
            //Length.Percent()这个API的作用是创建一个表示百分比长度的Length对象，通常用于UI布局中指定元素的尺寸或位置。
        }
        else
        {
            skillCooldownText.text = "";
            skillCooldownOverlay.style.height = Length.Percent(0);
        }
    }

    private void UpdatePotionCDUI()
    {
        float remaining = registeredPlayer.GetPotionCooldownRemaining();
        float duration = registeredPlayer.GetPotionCooldownDuration();
        if (remaining > 0)
        {
            potionCooldown.style.height = Length.Percent((remaining / duration) * 100);
        }
        else
        {
            potionCooldown.style.height = Length.Percent(0);
        }
    }
    #endregion

    #region 内部
    private void Awake()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        // 获取根元素
        root = uiDocument.rootVisualElement;

        // 获取所有UI元素引用
        levelLabel = root.Q<Label>("LevelLabel");
        hpLabel = root.Q<Label>("HpLabel");
        mpLabel = root.Q<Label>("MpLabel");
        expLabel = root.Q<Label>("ExpLabel");
        hpBarFill = root.Q<VisualElement>("HpBarFill");
        mpBarFill = root.Q<VisualElement>("MpBarFill");
        expBarFill = root.Q<VisualElement>("ExpBarFill");
        potionIcon = root.Q<VisualElement>("PotionIcon");
        potionCount = root.Q<Label>("PotionCount");
        potionCooldown = root.Q<VisualElement>("PotionCooldown");
        skillCooldownText = root.Q<Label>("SkillCooldown");
        skillCooldownOverlay = root.Q<VisualElement>("SkillCooldownOverlay");

        // 加载血瓶图标
        LoadPotionIcon();

        // 初始隐藏
        Hide();
    }

    /// <summary>
    /// 加载血瓶图标
    /// </summary>
    private void LoadPotionIcon()
    {
        Texture2D texture = Resources.Load<Texture2D>("Texture/UI/HealthPots");
        if (texture != null)
        {
            potionIcon.style.backgroundImage = new StyleBackground(texture);
        }
        else
        {
            Debug.LogWarning("HUDPanel: 未找到血瓶图标 Resources/Texture/UI/HealthPots.png");
        }
    }

    /// <summary>
    /// 刷新所有UI
    /// </summary>
    private void RefreshAllUI()
    {
        UpdateHpUI();
        UpdateMpUI();
        UpdateExpUI();
        UpdateLevelUI();
        UpdatePotionUI();
    }


    #endregion 
}
