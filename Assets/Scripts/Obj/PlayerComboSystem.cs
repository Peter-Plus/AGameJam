using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Spine;
using UnityEngine;

/// <summary>
/// 玩家连击系统
/// </summary>
public class PlayerComboSystem : MonoBehaviour
{
    // 成员变量
    #region 成员变量
    //组件引用
    [Header("组件引用")]
    [SerializeField] private PlayerCore playerCore;
    [SerializeField] private PlayerTrans playerTrans;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private Spine.AnimationState animationState;
    ////连击数据
    //[Header("连击数据")]
    //[SerializeField] private float comboResetTime = 1.0f; // 连击重置时间

    [System.Serializable]
    public class AttackMoveConfig
    {
        public string animationName;
        public MoveType moveType;
        public float distance;
        public float duration;
    }

    public enum MoveType
    {
        Instant, // 瞬移
        Smooth // 匀速
    }

    [Header("攻击位移数据")]
    [SerializeField] private AttackMoveConfig[] attackMoveConfigs;
    private Dictionary<string, AttackMoveConfig> attackMoveDict; 

    //动画名称
    private const string ANIM_J1 = "J1";
    private const string ANIM_J2 = "J2";
    private const string ANIM_J3 = "J3";
    private const string ANIM_A4 = "A4";
    private const string ANIM_AA5 = "J3";
    private const string ANIM_AB5 = "J3";
    private const string ANIM_B4 = "A4";
    private const string ANIM_BA5 = "J3";
    private const string ANIM_BB5 = "J3";

    //spine事件名称
    private const string EVENT_HIT_CHECK = "Hit_Check";
    private const string EVENT_INPUT_START = "Input_Start";
    private const string EVENT_COMBO_POINT = "Combo_Point";
    private const string EVENT_CAN_MOVE = "Can_Move";
    private const string EVENT_ATTACK_MOVE = "Attack_Move";

    //状态变量
    private int currentComboStep = 0; // 当前连击段数 0为无连击
    private bool isAttacking = false; // 是否正在攻击
    private bool canCombo = false; // 是否可以续接连击切换动画，只有缓冲阶段允许切换 Combo_Point D区
    private bool canMoveCancel = false;//是否可以打断并移动 E区
    private bool canInputNextAttack = false;//是否可以输入下段攻击,后摇和缓冲阶段允许输入缓存 C和D区

    //输入缓冲
    private bool attackInputBuffered = false;// 攻击输入缓冲
    private bool jumpInputBuffered = false;// 跳跃输入缓冲,B4需要

    //J4分支记录
    private bool isAirBranch = false;//是否进入了空中分支B4

    #endregion

    //生命周期
    #region 生命周期

    private void Awake()
    {
        //全在inspector绑定
        animationState = skeletonAnimation.AnimationState;
        //绑定事件
        animationState.Event += HandleSpineEvent;
        animationState.Complete += HandleAnimationComplete;
        //初始化攻击位移字典
        attackMoveDict = new Dictionary<string, AttackMoveConfig>();
        foreach(var config in attackMoveConfigs)
        {
            attackMoveDict[config.animationName] = config;
        }
    }

    private void Update()
    {
        if(!playerCore.IsLive()||!InputManager.Instance.CanPlayerMove())
        {
            ResetCombo();
            return;
        }
        //处理攻击输入  跳跃检测一并处理
        HandleAttackInput();
        // 处理移动输入
        HandleMovementInput();
    }
    private void OnDestroy()
    {
        //解绑事件
        if(animationState!=null)
        {
            animationState.Event -= HandleSpineEvent;
            animationState.Complete -= HandleAnimationComplete;
        }
    }
    #endregion

    //输入处理
    #region 输入处理

    //攻击输入处理（含跳跃检测）
    private void HandleAttackInput()
    {
        //检测攻击输入
        if(InputManager.Instance.GetAttackInput())
        {
            if(!isAttacking)
            {
                StartCombo();//没在攻击，直接开始第一段
            }
            else if(canInputNextAttack)
            {
                attackInputBuffered = true; // 在Input_Start或Combo_Point阶段,缓冲输入
                //如果在D区，立即执行
                if(canCombo)
                {
                    ExecuteNextCombo();
                }
            }
            //E区不响应攻击输入
        }
        //跳跃检测
        if (Input.GetKeyDown(KeyCode.Space)&&currentComboStep==3&&isAttacking)
        {
            jumpInputBuffered = true;
        }
    }

    // 移动输入处理
    private void HandleMovementInput()
    {
        // 只有在Can_Move事件后才能移动
        if(canMoveCancel&&isAttacking)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(horizontal) > 0.1f)//0.1f为死区
            {
                ResetCombo();
                //给AnimationController接管
            }
        }
    }

    #endregion

    #region 连击逻辑
    // 开始连击
    private void StartCombo()
    {
        currentComboStep = 1;
        isAttacking = true;
        isAirBranch = false;
        PlayAttackAnimation(ANIM_J1);
    }
    // 续接连击
    private void ExecuteNextCombo()
    {
        if (!attackInputBuffered) return;
        canCombo = false;
        canInputNextAttack = false;
        attackInputBuffered = false;
        currentComboStep++;
        string nextAnim = GetNextComboAnimation();
        if(!string.IsNullOrEmpty(nextAnim))
        {
            PlayAttackAnimation(nextAnim);
        }
        else
        {
            //打完五段，重置连击
            ResetCombo();
        }
    }

    // 获取连击动画名称
    private string GetNextComboAnimation()
    {
        switch(currentComboStep)
        {
            case 2:
                return ANIM_J2;
            case 3:
                return ANIM_J3;
            case 4:
                //检测是否跳跃分支
                if(jumpInputBuffered)
                {
                    isAirBranch = true;
                    jumpInputBuffered = false;
                    return ANIM_B4;
                }
                else
                {
                    isAirBranch= false;
                    return ANIM_A4;
                }
            case 5:
                //根据分支和移动输入决定后续攻击
                return GetFifthComboAnimation();
            default:
                return null;
        }
    }

    // 获取第五段连击动画名称
    private string GetFifthComboAnimation()
    {
        //检测移动输入方向
        float horizontal = Input.GetAxisRaw("Horizontal");
        bool isFacingRight = playerTrans.IsFacingRight();
        // 判断是否反向移动
        bool isReverseInput = false;
        if(isFacingRight&&horizontal<-0.1f) isReverseInput = true;
        if(!isFacingRight&&horizontal>0.1f) isReverseInput = true;
        if(isAirBranch)
        {
            //B4分支
            return isReverseInput ? ANIM_BB5 : ANIM_BA5;
        }
        else
        {
            //A4分支
            return isReverseInput? ANIM_AB5 : ANIM_AA5;
        }
    }

    //重置连击状态
    private void ResetCombo()
    {
        //重置状态
        currentComboStep = 0;
        isAttacking = false;
        canCombo = false;
        canMoveCancel = false;
        canInputNextAttack = false;
        attackInputBuffered = false;
        jumpInputBuffered = false;
        isAirBranch = false;
    }
    #endregion

    #region Spine事件处理
    private void HandleSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        switch (e.Data.Name)
        { 
            case EVENT_HIT_CHECK:
                OnHitCheck();
                break;
            case EVENT_INPUT_START:
                //后摇阶段
                canInputNextAttack = true;
                canCombo = false;//可以输入缓存但不切换动画
                canMoveCancel= false;
                break;
            case EVENT_COMBO_POINT:
                canCombo = true;
                if(attackInputBuffered)
                {
                    ExecuteNextCombo();
                }
                break;
            case EVENT_CAN_MOVE:
                //回正E阶段，可以移动打断剩余后摇，但不再接受攻击输入
                canMoveCancel = true;
                canInputNextAttack = false;
                canCombo=false;
                attackInputBuffered = false;
                break;
            case EVENT_ATTACK_MOVE:
                //获取当前动画名称
                string currentAnim = trackEntry.Animation.Name;
                if(attackMoveDict.TryGetValue(currentAnim,out var config))
                {
                    ExecuteMove(config);
                }
                break;
        }
    }

    //动画播放完成处理
    private void HandleAnimationComplete(TrackEntry trackEntry)
    {
        if(trackEntry.Loop) return;//循环动画如walk/run也会调用该函数，避免影响攻击
        //后续动画种类更复杂了可以考虑用trackEntry.Animation.Name判断
        //此时E区播放完毕
        if (isAttacking)
        {
            ResetCombo();
            //给AnimationController接管
        }
    }

    private void OnHitCheck()
    {
        int damage = playerCore.GetAttackDamage();
        //音效、震动相关
        string currentAnim = animationState.GetCurrent(0).Animation.Name;
        switch (currentAnim)
        {
            case ANIM_J1:
                AudioManager.Instance.PlaySound("J1");
                break;
            case ANIM_J2:
                AudioManager.Instance.PlaySound("J2");
                CameraController.Instance.Shake(0.05f,0.05f);
                break;
            case ANIM_J3:
                AudioManager.Instance.PlaySound("J3");
                break;
            case ANIM_A4:
                AudioManager.Instance.PlaySound("A4");
                CameraController.Instance.Shake(0.08f, 0.07f);
                break;  
        }
        // 特效什么的也可以放在这里
        // TODO:实现攻击碰撞检测

    }

    private void ExecuteMove(AttackMoveConfig config)
    {
        Vector3 moveDir = playerTrans.IsFacingRight() ? Vector3.right : Vector3.left;

        switch (config.moveType)
        {
            case MoveType.Instant:
                transform.position += moveDir * config.distance;
                break;
            case MoveType.Smooth:
                StartCoroutine(SmoothMove(moveDir*config.distance,config.duration));
                break;
            default:
                break;
        }
    }

    private IEnumerator SmoothMove(Vector3 offset, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + offset;
        float elapsed = 0f;// 已过时间
        while(elapsed<duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }
    #endregion

    #region 动画播放

    private void PlayAttackAnimation(string animName)
    {
        PlayAnimation(animName,false);
    }

    private void PlayAnimation(string animName, bool loop)
    {
        animationState.SetAnimation(0, animName, loop);
    }

    #endregion

    #region 公开API
    public bool IsAttacking() => isAttacking;
    public int GetCurrentComboStep() => currentComboStep;

    #endregion
}
