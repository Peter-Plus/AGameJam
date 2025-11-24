using UnityEngine;

public class PlayerTrans : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("远近缩放设置")]
    [SerializeField] private float scaleSpeed = 0.1f;  // 缩放速度

    [Header("移动范围限制")]
    [SerializeField] private float minZ = 0f;
    [SerializeField] private float maxZ = 0f;

    [Header("组件引用")]
    public SpriteRenderer spriteRenderer;

    private Vector3 movement;
    private float baseZPosition;  // 记录初始Y坐标作为基准
    private Vector3 baseScale;    // 记录初始缩放
    private Rigidbody rb;

    #region API
    //外部API-获取面朝向IsFacingRight
    public bool IsFacingRight()
    {
        if (spriteRenderer != null)
        {
            return !spriteRenderer.flipX;
        }
        return true; // 默认朝右
    }

    #endregion

    #region 生命周期
    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        rb = GetComponent<Rigidbody>();
        // 记录初始状态
        baseZPosition = transform.position.z;
        baseScale = transform.localScale;
    }

    private void Update()
    {
        //通过PlayerCore获取存活状态isLive
        PlayerCore playerCore = GetComponent<PlayerCore>();
        if(!playerCore.IsLive()||!playerCore.CanMove())
        {
            //直接静止玩家刚体，防止有残留移动
            movement = Vector3.zero;
            return;
        }
        HandleInput();
        UpdateScale();
        FlipSprite();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    #endregion

    #region 内部
    private void FlipSprite()
    {
        if (movement.x != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = movement.x < 0;
        }
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement = new Vector3(horizontal, 0f , vertical).normalized;
    }

    private void MovePlayer()
    {
        if(movement==Vector3.zero)
        {
            //静止刚体
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        //目标位置
        Vector3 targetPos = rb.position+ movement * moveSpeed * Time.fixedDeltaTime;
        targetPos.z = Mathf.Clamp(targetPos.z, minZ, maxZ);
        rb.MovePosition(targetPos);

    }

    // 根据Z位置调整缩放
    private void UpdateScale()
    {
        // 计算相对于初始位置的Y偏移
        float yOffset = transform.position.z - baseZPosition;
        float scaleFactor = Mathf.Exp(-yOffset * scaleSpeed);
        transform.localScale = baseScale * scaleFactor;
    }
    #endregion
}