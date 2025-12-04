using Spine.Unity;
using UnityEngine;

public class PlayerTrans : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;

    [Header("远近缩放设置")]
    [SerializeField] private float scaleSpeed = 0.1f;

    [Header("移动范围限制")]
    [SerializeField] private float minZ = 0f;
    [SerializeField] private float maxZ = 0f;

    [Header("组件引用")]
    //public SpriteRenderer spriteRenderer;
    //改用动画渲染
    [SerializeField] private SkeletonAnimation skeletonAnimation;
    [SerializeField] private PlayerAnimationController animController;
    [SerializeField] private PlayerComboSystem comboSystem;
    private Vector3 movement;
    private float baseZPosition;
    private Vector3 baseScale;
    private float currentMoveSpeed;
    private bool isRunning = false;
    private bool isFacingRight = true;


    #region API
    // 判断角色是否面向右侧
    //public bool IsFacingRight()
    //{
    //    if (spriteRenderer != null)
    //    {
    //        return !spriteRenderer.flipX;
    //    }
    //    return true;
    //}
    public bool IsFacingRight() => isFacingRight;

    public bool IsMoving() => movement.magnitude >0.1f;//0.1f作为移动阈值
    public bool IsRunning() => isRunning;

    #endregion

    #region 生命周期
    private void Awake()
    {
        baseZPosition = transform.position.z;// 记录初始Z位置
        baseScale = transform.localScale;
    }

    private void Update()
    {
        PlayerCore playerCore = GetComponent<PlayerCore>();
        if (!playerCore.IsLive() || !playerCore.CanMove())
        {
            movement = Vector3.zero;
            isRunning = false;
            return;
        }
        //攻击时不允许移动
        if (comboSystem.IsAttacking())
        {
            movement = Vector3.zero;
            isRunning = false;
            return;
        }
        HandleInput();
        MovePlayer();  // 移动也放在Update中
        UpdateScale();
        FlipSprite();
    }
    #endregion

    #region 内部
    private void FlipSprite()
    {
        if (movement.x != 0)
        {
            bool shouldFaceRight = movement.x > 0;

            if (shouldFaceRight != isFacingRight)  // 只在方向改变时翻转
            {
                isFacingRight = shouldFaceRight;
                skeletonAnimation.Skeleton.ScaleX = isFacingRight ? 1 : -1;
            }
        }
    }

    private void HandleInput()
    {
        // 直接检测，不通过InputManager封装
        if (!InputManager.Instance.CanPlayerMove())
        {
            movement = Vector3.zero;
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement = new Vector3(horizontal, 0f, vertical).normalized;

        //检测跑步输入 按住左Shift键跑步
        if (movement.magnitude>0.1f&&Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
            currentMoveSpeed = runSpeed;
        }
        else
        {
            isRunning = false;
            currentMoveSpeed = moveSpeed;
        }
    }

    private void MovePlayer()
    {
        if (movement == Vector3.zero) return;

        Vector3 newPos = transform.position + movement * currentMoveSpeed * Time.deltaTime;
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);
        transform.position = newPos;
    }

    private void UpdateScale()
    {
        float yOffset = transform.position.z - baseZPosition;
        float scaleFactor = Mathf.Exp(-yOffset * scaleSpeed);
        transform.localScale = baseScale * scaleFactor;
    }
    #endregion
}