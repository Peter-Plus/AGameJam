using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractItem : MonoBehaviour
{
    #region 变量
    [Header("基础属性")]
    [SerializeField] protected string itemName; //物品名称
    [SerializeField] protected string itemDescription; //物品描述
    [SerializeField] protected SpriteRenderer itemIcon; //物品图标，直接指定
    protected GameObject player;//玩家对象,不太方便在场景中指定，可以通过标签查找

    [Header("交互设置")]
    [SerializeField] protected float interactRange = 2.0f; //交互距离
    [SerializeField] protected bool isInteractable = true; //是否可交互  交互方式：左键点击物品通过射线检测触发交互事件
    [SerializeField] protected int maxInteractCount = 1; //最大交互次数 -1为无限次
    [SerializeField] protected int currentInteractCount = 0; //当前交互次数

    [Header("高亮设置")]
    [SerializeField] protected bool isHighlightable = true;
    [SerializeField] protected Color glowColor = new Color(1f, 0.85f, 0.2f, 1f);
    protected SpriteGlowController glowController;
    #endregion

    #region 生命周期
    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        glowController = itemIcon.gameObject.AddComponent<SpriteGlowController>();
        glowController.SetGlowColor(glowColor);
    }
    #endregion

    #region API

    //调用交互的方法，允许子类重写
    public virtual bool TryInteract()
    {
        if (!isInteractable) return false;
        if (maxInteractCount != -1 && currentInteractCount >= maxInteractCount)
        {
            OnMaxInteractReached();
            return false;
        }
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > interactRange)
        {
            OnMaxDistenceReached();
            return false;
        }
        currentInteractCount++;
        StartCoroutine(OnInteract());
        return true;
    }
    //下面是一些可能用到但不太可能用到的API
    //重置交互次数
    public void ResetInteractCount()
    {
        currentInteractCount = 0;
    }
    //设置可否交互
    public void SetInteractable(bool value)
    {
        isInteractable = value;
    }
    //获取当前交互次数
    public int GetCurrentInteractCount() => currentInteractCount;
    #endregion

    #region 子类重写

    protected abstract IEnumerator OnInteract();//交互时调用的方法，必须由子类实现，可以通过currentInteractCount判断当前是第几次交互
    protected virtual void OnMaxDistenceReached() { }//可以写一些提示信息，表示距离过远无法交互
    protected virtual void OnMaxInteractReached() { }//可以写一些提示信息，表示交互次数已达上限

    protected virtual void OnMouseEnter()
    {
        if (isHighlightable && isInteractable)
        {
            glowController.EnableGlow();
        }
    }
    protected virtual void OnMouseExit()
    {
        if (isHighlightable)
        {
            glowController.DisableGlow();
        }
    }
    #endregion
}
