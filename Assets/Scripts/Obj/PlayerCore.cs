using System.Collections;
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
    [SerializeField] private int baseExpRequired = 80; // 基础升级所需经验
    [SerializeField] private int expGrowthPerLevel = 20; // 每级经验成长（线性增长）
    [SerializeField] private int hpGrowthPerLevel = 20; // 每级生命成长
    [SerializeField] private int mpGrowthPerLevel = 10; // 每级法力成长
    [SerializeField] private int attackGrowthPerLevel = 5; // 每级攻击成长
    [SerializeField] private int defenseGrowthPerLevel = 2; // 每级防御成长

    [Header("战斗设置")]
    [SerializeField] private float attackCooldown = 0.5f; // 普通攻击冷却
    //是否启用攻击特效
    [SerializeField] private bool enableAttackEffect = true;
    [SerializeField] private bool canMove = true; // 玩家是否可以移动，另一种暂停方式
    private float lastAttackTime = -999f; // 上次攻击时间
    private bool isLive = true; // 玩家是否存活


    // 当前属性缓存
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
    // 设置玩家能否移动
    public void SetCanMove(bool value)
    {
        canMove = value;
    }
    // 玩家是否可以移动
    public bool CanMove()
    {
        if(!isLive) return false;
        if(!canMove) return false;
        if(Time.timeScale==0f) return false;// 游戏暂停时不可移动
        return true;
    }
    // 玩家是否存活
    public bool IsLive() => isLive;
    // 获取技能冷却剩余时间
    public float GetSkillCooldownRemaining()
    {
        return Mathf.Max(0, skillCooldown - (Time.time - lastSkillTime));
    }
    // 获取血瓶冷却剩余时间
    public float GetPotionCooldownRemaining()
    {
        return Mathf.Max(0, potionCooldown - (Time.time - lastPotionTime));
    }
    // 获取技能冷却总时长
    public float GetSkillCooldownDuration() => skillCooldown;
    // 获取血瓶冷却总时长
    public float GetPotionCooldownDuration() => potionCooldown;
    // 普通攻击
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

    // 使用技能攻击
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

    // 玩家受到伤害 - 初始伤害
    public void TakeDamage(int damage)
    {

        // 计算实际伤害(伤害 - 防御)
        int actualDamage = Mathf.Max(1, damage - currentDefense);

        // 扣除生命值
        currentHp = Mathf.Max(0, currentHp - actualDamage);
        DataManager.Instance.SetHp(currentHp);

        // 显示伤害提示
        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip($"-{actualDamage} HP");
        
        // 检查是否死亡
        if (currentHp <= 0)
        {
            OnPlayerDeath();
        }
    }

    /// 使用血瓶回复生命
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

        // 显示相关UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTextTip($"+{healAmount} HP");
            UIManager.Instance.OnUsedPotion();
        }
    }

    // 拾取血瓶*1
    public void PickupHealthPotion()
    {
        int potionCount = DataManager.Instance.GetHealthPotionCount();
        DataManager.Instance.SetHealthPotionCount(potionCount + 1);

        if (UIManager.Instance != null)
            UIManager.Instance.ShowTextTip("拾取了一个血瓶!");
    }

    // 获得经验值
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

    // 恢复生命值(非血瓶,备用)
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

    // 恢复法力值
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

    // 获取当前生命值
    public int GetCurrentHp() => currentHp;

    // 获取当前法力值
    public int GetCurrentMp() => currentMp;

    // 获取当前等级
    public int GetCurrentLevel() => currentLevel;

    // 获取当前经验值
    public int GetCurrentExp() => currentExp;

    // 获取当前等级升级所需的最大经验值
    public int GetMaxExpForCurrentLevel()
    {
        return CalculateMaxExpForLevel(currentLevel);
    }

    #endregion

    #region 生命周期
    private void Start()
    {
        // 在Start中加载属性，确保DataManager和UIManager已初始化
        RefreshPlayerStats();
        // 注册到HUD
        UIManager.Instance.RegisterPlayer(this);
    }

    private void Update()
    {
        if(!isLive || !canMove) return;
        // 处理调试输入(开发测试用)
        HandleDebugInput();
    }
    #endregion

    #region 私有方法
    // 刷新玩家属性
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
    }

    // 消耗法力值
    private void ConsumeMana(int amount)
    {
        currentMp = Mathf.Max(0, currentMp - amount);
        DataManager.Instance.SetMp(currentMp);
    }

    //检查是否升级
    private void CheckLevelUp()
    {
        // 循环检查，因为可能一次获得足够多的经验连续升级
        while (true)
        {
            int maxExp = GetMaxExpForCurrentLevel();

            // 如果当前经验达到升级要求
            if (currentExp >= maxExp)
            {
                // 计算溢出的经验
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

    // 升级
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
    }

    /// <summary>
    /// 计算指定等级升级所需的最大经验值
    /// 公式: baseExpRequired + level* expGrowthPerLevel
    /// </summary>
    private int CalculateMaxExpForLevel(int level)
    {
        return baseExpRequired + (level) * expGrowthPerLevel;
    }

    // 玩家死亡处理
    private void OnPlayerDeath()
    {
        //玩家死亡时取消所有输入控制
        isLive = false;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowTip("你已死亡!", () =>
            {
                // 重置玩家数据
                DataManager.Instance.ResetSaveData();
                // 显示加载界面，等待2s返回主界面
                UIManager.Instance.ShowLoadingPanel(true);
                StartCoroutine(ReturnToMainMenuAfterDelay(2f));
            });
    }
    private IEnumerator ReturnToMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        GameLevelManager.Instance.FailAndReturnToMainMenu();
    }

    // 处理调试输入(开发测试用)
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
            TakeDamage(35);
        }

    }
    #endregion
}
