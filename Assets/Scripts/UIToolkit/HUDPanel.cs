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
    private float skillCooldownTime = 5f; // 技能冷却时间5秒
    private float currentSkillCooldown = 0f;
    private bool isVisible = false;

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

        // 开始技能冷却循环
        StartCoroutine(SkillCooldownLoop());
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

    #region 更新UI方法
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
    /// <summary>
    /// 技能冷却循环（5秒循环）
    /// </summary>
    private IEnumerator SkillCooldownLoop()
    {
        while (isVisible)
        {
            // 开始冷却
            currentSkillCooldown = skillCooldownTime;

            while (currentSkillCooldown > 0)
            {
                currentSkillCooldown -= Time.deltaTime;
                UpdateSkillCooldownUI();
                yield return null;
            }

            // 冷却完成
            currentSkillCooldown = 0;
            UpdateSkillCooldownUI();

            // 等待下一次循环开始
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 更新技能冷却UI
    /// </summary>
    private void UpdateSkillCooldownUI()
    {
        if (currentSkillCooldown > 0)
        {
            // 显示冷却时间
            int cooldownSeconds = Mathf.CeilToInt(currentSkillCooldown);
            skillCooldownText.text = cooldownSeconds.ToString();

            // 更新冷却遮罩高度（从下往上）
            float cooldownPercent = currentSkillCooldown / skillCooldownTime;
            skillCooldownOverlay.style.height = Length.Percent(cooldownPercent * 100);
        }
        else
        {
            // 冷却完成
            skillCooldownText.text = "";
            skillCooldownOverlay.style.height = Length.Percent(0);
        }
    }
    #endregion

    #region 血瓶冷却相关（可选，暂未实现）
    /// <summary>
    /// 使用血瓶时调用，显示冷却动画
    /// </summary>
    public void OnUsedPotion()
    {
        UpdatePotionUI();
        //可以在这里添加血瓶使用后的冷却逻辑
    }
    #endregion
}