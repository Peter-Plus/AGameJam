using UnityEngine;

/// <summary>
/// 玩家核心战斗系统
/// 负责玩家属性、攻击、技能、受伤、升级等核心逻辑
/// </summary>
public class PlayerCore : MonoBehaviour
{
    #region 成员属性
    [Header("技能设置")]
    [SerializeField] private int skillManaCost = 20; // 技能消耗蓝量
    [SerializeField] private float skillDamageMultiplier = 2.0f; // 技能伤害倍率
    [SerializeField] private float skillCooldown = 2.0f; // 技能冷却时间
    private float lastSkillTime = -999f; // 上次使用技能的时间

    [Header("血瓶设置")]
    [SerializeField] private int healthPotionHealAmount = 30; // 血瓶回复量
    [SerializeField] private float potionCooldown = 1.0f; // 血瓶冷却时间
    private float lastPotionTime = -999f; // 上次使用血瓶的时间

    [Header("升级设置")]
    [SerializeField] private int baseExpRequired = 100; // 基础升级所需经验
    [SerializeField] private int expGrowthPerLevel = 50; // 每级经验成长（线性增长）
    [SerializeField] private int hpGrowthPerLevel = 20; // 每级生命成长
    [SerializeField] private int mpGrowthPerLevel = 10; // 每级法力成长
    [SerializeField] private int attackGrowthPerLevel = 5; // 每级攻击成长
    [SerializeField] private int defenseGrowthPerLevel = 2; // 每级防御成长

    [Header("战斗设置")]
    [SerializeField] private float attackCooldown = 0.5f; // 普通攻击冷却
    //是否启用攻击特效
    [SerializeField] private bool enableAttackEffect = true;
    private float lastAttackTime = -999f; // 上次攻击时间

    [Header("无敌帧设置")]
    [SerializeField] private float invincibleDuration = 1.0f; // 受伤后无敌时间
    private float invincibleEndTime = -999f; // 无敌结束时间
    private bool isInvincible = false; // 是否处于无敌状态

    // 当前属性缓存(避免频繁访问DataManager)
    private int currentHp;
    private int currentMaxHp;
    private int currentMp;
    private int currentMaxMp;
    private int currentLevel;
    private int currentExp;
    private int currentAttack;
    private int currentDefense;
    #endregion

    #region 公开API
    /// <summary>
    /// 普通攻击
    /// </summary>
    /// <returns>攻击伤害值,如果攻击失败返回0</returns>
    public int Attack()
    {
        // 检查攻击冷却
        if (Time.time < lastAttackTime + attackCooldown)
        {
            return 0;
        }
        lastAttackTime = Time.time;
        
        if (enableAttackEffect)
        {
            //获取面向方向
            bool facingRight = true;
            PlayerTrans playerTrans = GetComponent<PlayerTrans>();
            if (playerTrans != null)
            {
                facingRight = playerTrans.IsFacingRight();
            }
            // 显示攻击特效
            float angle = 0f;
            angle = facingRight ? 0f : 180f;
            CrescentSlashEffect.Instance.PlayCrescentSlash(transform.position, angle);
        }
        // 返回当前攻击力
        return currentAttack;
    }

    /// <summary>
    /// 使用技能攻击
    /// </summary>
    /// <returns>技能伤害值,如果技能使用失败返回0</returns>
    public int UseSkill()
    {
        // 检查技能冷却
        if (Time.time < lastSkillTime + skillCooldown)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowTextTip("技能冷却中!");
            return 0;
        }

        // 检查蓝量是否足够
        if (currentMp < skillManaCost)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowTextTip("法力不足!");
            return 0;
        }

        // 消耗蓝量
        lastSkillTime = Time.time;
        ConsumeMana(skillManaCost);

        // 计算技能伤害
        int skillDamage = Mathf.RoundToInt(currentAttack * skillDamageMultiplier);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip($"使用技能! 造成 {skillDamage} 点伤害!");

        return skillDamage;
    }

    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="damage">原始伤害值</param>
    public void TakeDamage(int damage)
    {
        // 检查无敌状态
        if (isInvincible)
        {
            return;
        }

        // 计算实际伤害(伤害 - 防御)
        int actualDamage = Mathf.Max(1, damage - currentDefense);

        // 扣除生命值
        currentHp = Mathf.Max(0, currentHp - actualDamage);
        DataManager.Instance.SetHp(currentHp);

        // 显示伤害提示
        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip($"-{actualDamage} HP");

        // 进入无敌状态
        StartInvincible();

        // 检查是否死亡
        if (currentHp <= 0)
        {
            OnPlayerDeath();
        }
    }

    /// <summary>
    /// 使用血瓶回复生命
    /// </summary>
    public void UseHealthPotion()
    {
        // 检查冷却时间
        if (Time.time < lastPotionTime + potionCooldown)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowTextTip("血瓶冷却中!");
            return;
        }

        // 检查是否有血瓶
        int potionCount = DataManager.Instance.GetHealthPotionCount();
        if (potionCount <= 0)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowTextTip("没有血瓶了!");
            return;
        }

        // 检查血量是否已满
        if (currentHp >= currentMaxHp)
        {
            if (UIManager.Instance != null)
                UIManager.Instance.ShowTextTip("生命值已满!");
            return;
        }

        // 使用血瓶
        lastPotionTime = Time.time;
        DataManager.Instance.SetHealthPotionCount(potionCount - 1);

        // 回复生命值
        int healAmount = Mathf.Min(healthPotionHealAmount, currentMaxHp - currentHp);
        currentHp += healAmount;
        DataManager.Instance.SetHp(currentHp);

        // 显示回复提示
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTextTip($"+{healAmount} HP");
            UIManager.Instance.OnUsedPotion();
        }
    }

    /// <summary>
    /// 拾取血瓶*1
    /// </summary>
    public void PickupHealthPotion()
    {
        int potionCount = DataManager.Instance.GetHealthPotionCount();
        DataManager.Instance.SetHealthPotionCount(potionCount + 1);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip("拾取了一个血瓶!");
    }

    /// <summary>
    /// 获得经验值
    /// </summary>
    /// <param name="exp">获得的经验值</param>
    public void GainExperience(int exp)
    {
        currentExp += exp;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip($"+{exp} EXP");

        // 检查是否升级（可能连续升级）
        CheckLevelUp();

        // 保存经验值
        DataManager.Instance.SetExp(currentExp);
    }

    /// <summary>
    /// 恢复生命值(非血瓶,备用)
    /// </summary>
    /// <param name="amount">恢复量</param>
    public void RestoreHealth(int amount)
    {
        if (currentHp >= currentMaxHp)
        {
            return;
        }

        int healAmount = Mathf.Min(amount, currentMaxHp - currentHp);
        currentHp += healAmount;
        DataManager.Instance.SetHp(currentHp);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip($"+{healAmount} HP");
    }

    /// <summary>
    /// 恢复法力值
    /// </summary>
    /// <param name="amount">恢复量</param>
    public void RestoreMana(int amount)
    {
        if (currentMp >= currentMaxMp)
        {
            return;
        }

        int restoreAmount = Mathf.Min(amount, currentMaxMp - currentMp);
        currentMp += restoreAmount;
        DataManager.Instance.SetMp(currentMp);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip($"+{restoreAmount} MP");
    }

    /// <summary>
    /// 获取当前生命值
    /// </summary>
    public int GetCurrentHp() => currentHp;

    /// <summary>
    /// 获取当前法力值
    /// </summary>
    public int GetCurrentMp() => currentMp;

    /// <summary>
    /// 获取当前等级
    /// </summary>
    public int GetCurrentLevel() => currentLevel;

    /// <summary>
    /// 获取当前经验值
    /// </summary>
    public int GetCurrentExp() => currentExp;

    /// <summary>
    /// 获取当前等级升级所需的最大经验值
    /// </summary>
    public int GetMaxExpForCurrentLevel()
    {
        return CalculateMaxExpForLevel(currentLevel);
    }

    /// <summary>
    /// 是否处于无敌状态
    /// </summary>
    public bool IsInvincible() => isInvincible;
    #endregion

    #region 生命周期
    private void Start()
    {
        // 在Start中加载属性，确保DataManager和UIManager已初始化
        RefreshPlayerStats();
    }

    private void Update()
    {
        // 更新无敌状态
        UpdateInvincibleState();

        // 处理输入(开发测试用)
        HandleDebugInput();
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 刷新玩家属性(从DataManager同步)
    /// </summary>
    private void RefreshPlayerStats()
    {
        // 检查DataManager是否存在
        if (DataManager.Instance == null)
        {
            Debug.LogError("[PlayerCore] DataManager.Instance为空! 请确保场景中有DataManager");
            return;
        }

        currentHp = DataManager.Instance.GetHp();
        currentMaxHp = DataManager.Instance.GetMaxHp();
        currentMp = DataManager.Instance.GetMp();
        currentMaxMp = DataManager.Instance.GetMaxMp();
        currentLevel = DataManager.Instance.GetLevel();
        currentExp = DataManager.Instance.GetExp();
        currentAttack = DataManager.Instance.GetAttack();
        currentDefense = DataManager.Instance.GetDefense();

        Debug.Log($"[PlayerCore] 玩家属性加载 - Lv.{currentLevel} HP:{currentHp}/{currentMaxHp} EXP:{currentExp}/{GetMaxExpForCurrentLevel()}");
    }

    /// <summary>
    /// 消耗法力值
    /// </summary>
    private void ConsumeMana(int amount)
    {
        currentMp = Mathf.Max(0, currentMp - amount);
        DataManager.Instance.SetMp(currentMp);
    }

    /// <summary>
    /// 开始无敌状态
    /// </summary>
    private void StartInvincible()
    {
        isInvincible = true;
        invincibleEndTime = Time.time + invincibleDuration;
    }

    /// <summary>
    /// 更新无敌状态
    /// </summary>
    private void UpdateInvincibleState()
    {
        if (isInvincible && Time.time >= invincibleEndTime)
        {
            isInvincible = false;
        }
    }

    /// <summary>
    /// 检查是否升级
    /// </summary>
    private void CheckLevelUp()
    {
        // 循环检查，因为可能一次获得足够多的经验连续升级
        while (true)
        {
            int maxExp = GetMaxExpForCurrentLevel();

            // 如果当前经验达到升级要求
            if (currentExp >= maxExp)
            {
                // 计算溢出的经验（超过升级线的部分）
                int overflowExp = currentExp - maxExp;

                // 执行升级
                LevelUp();

                // 升级后经验设为溢出的经验
                currentExp = overflowExp;
            }
            else
            {
                // 经验不足，退出循环
                break;
            }
        }
    }

    /// <summary>
    /// 升级
    /// </summary>
    private void LevelUp()
    {
        // 等级提升
        currentLevel++;

        // 提升属性
        currentMaxHp += hpGrowthPerLevel;
        currentMaxMp += mpGrowthPerLevel;
        currentAttack += attackGrowthPerLevel;
        currentDefense += defenseGrowthPerLevel;

        // 升级时回满血蓝
        currentHp = currentMaxHp;
        currentMp = currentMaxMp;

        // 保存所有属性到DataManager
        DataManager.Instance.SetLevel(currentLevel);
        DataManager.Instance.SetMaxHp(currentMaxHp);
        DataManager.Instance.SetMaxMp(currentMaxMp);
        DataManager.Instance.SetAttack(currentAttack);
        DataManager.Instance.SetDefense(currentDefense);
        DataManager.Instance.SetHp(currentHp);
        DataManager.Instance.SetMp(currentMp);

        // 显示升级提示
        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip($"升级! 达到 Lv.{currentLevel}");

        Debug.Log($"[PlayerCore] 升级! Lv.{currentLevel} - HP:{currentMaxHp} MP:{currentMaxMp} ATK:{currentAttack} DEF:{currentDefense} 下一级需要:{GetMaxExpForCurrentLevel()}EXP");
    }

    /// <summary>
    /// 计算指定等级升级所需的最大经验值
    /// 公式: baseExpRequired + (level - 1) * expGrowthPerLevel
    /// 例如: 100 + (1-1)*50 = 100 (1级满经验)
    ///       100 + (2-1)*50 = 150 (2级满经验)
    ///       100 + (3-1)*50 = 200 (3级满经验)
    /// </summary>
    private int CalculateMaxExpForLevel(int level)
    {
        return baseExpRequired + (level - 1) * expGrowthPerLevel;
    }

    /// <summary>
    /// 玩家死亡处理
    /// </summary>
    private void OnPlayerDeath()
    {
        Debug.Log("玩家死亡!");

        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip("你死了...");

        // TODO: 游戏结束逻辑
        // 可以显示游戏结束面板,重新开始等
    }

    /// <summary>
    /// 处理调试输入(开发测试用)
    /// </summary>
    private void HandleDebugInput()
    {
        // 按J键普通攻击(测试)
        if (Input.GetKeyDown(KeyCode.J))
        {
            int damage = Attack();
            if (damage > 0)
            {
                Debug.Log($"普通攻击造成 {damage} 点伤害");
            }
        }

        // 按K键使用技能(测试)
        if (Input.GetKeyDown(KeyCode.K))
        {
            int damage = UseSkill();
            if (damage > 0)
            {
                Debug.Log($"技能攻击造成 {damage} 点伤害");
            }
        }

        // 按H键使用血瓶(测试)
        if (Input.GetKeyDown(KeyCode.H))
        {
            UseHealthPotion();
        }

        // 按L键获得经验(测试)
        if (Input.GetKeyDown(KeyCode.L))
        {
            GainExperience(50);
        }

        // 按O键受到伤害(测试)
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeDamage(15);
        }

        // 按P键查看当前状态(测试)
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log($"===== 玩家状态 =====");
            Debug.Log($"等级: Lv.{currentLevel}");
            Debug.Log($"生命: {currentHp}/{currentMaxHp}");
            Debug.Log($"法力: {currentMp}/{currentMaxMp}");
            Debug.Log($"经验: {currentExp}/{GetMaxExpForCurrentLevel()}");
            Debug.Log($"攻击: {currentAttack}");
            Debug.Log($"防御: {currentDefense}");
            Debug.Log($"==================");
        }
    }
    #endregion
}