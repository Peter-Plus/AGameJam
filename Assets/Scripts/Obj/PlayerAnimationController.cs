using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

/// <summary>
/// 专门管理非战斗动画 如walk run idle等
/// </summary>

public class PlayerAnimationController : MonoBehaviour
{
    #region 字段

    //组件引用
    [SerializeField] private PlayerCore playerCore;
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private PlayerTrans playerTrans;
    [SerializeField] private PlayerComboSystem comboSystem;
    [SerializeField] private Spine.AnimationState animationState;

    //动画名称
    private const string ANIM_IDLE = "idle";
    private const string ANIM_WALK = "walk";
    private const string ANIM_RUN = "run";

    //状态变量
    private string currentAnimation = "";

    #endregion

    #region 生命周期
    private void Awake()
    {
        animationState = skeletonAnimation.AnimationState;
    }
    private void Start()
    {
        PlayAnimation(ANIM_IDLE, true);
    }

    private void Update()
    {
        //普攻连击中暂停移动动画更新
        if(comboSystem.IsAttacking())
        {
            return;
        }
        if (!playerCore.IsLive() || !InputManager.Instance.CanPlayerMove())
        {
            if (currentAnimation != ANIM_IDLE)
            {
                PlayAnimation(ANIM_IDLE, true);
            }
            return;
        }
        //根据移动状态更新动画
        UpdateMovementAnimation();
    }
    #endregion

    #region 内部

    private void UpdateMovementAnimation()
    {
        string targetAnimation = ANIM_IDLE;
        if(playerTrans.IsMoving())
        {
            if (playerTrans.IsRunning())
            {
                targetAnimation = ANIM_RUN;
            }
            else
            {
                targetAnimation = ANIM_WALK;
            }
        }
        else
        {
            targetAnimation = ANIM_IDLE;
        }
        //只在动画切换时设置新动画
        if(currentAnimation!=targetAnimation)
        {
            PlayAnimation(targetAnimation, true);
        }
    }
    private void PlayAnimation(string animationName, bool loop)
    {
        animationState.SetAnimation(0, animationName, loop);
        currentAnimation = animationName;
    }
    #endregion

    #region 公开API
    public string GetCurrentAnimation() => currentAnimation;

    #endregion
}
