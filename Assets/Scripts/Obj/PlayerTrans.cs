using UnityEngine;

public class PlayerTrans : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("远近缩放设置")]
    [SerializeField] private float scaleSpeed = 0.1f;

    [Header("移动范围限制")]
    [SerializeField] private float minZ = 0f;
    [SerializeField] private float maxZ = 0f;

    [Header("组件引用")]
    public SpriteRenderer spriteRenderer;

    private Vector3 movement;
    private float baseZPosition;
    private Vector3 baseScale;

    #region API
    public bool IsFacingRight()
    {
        if (spriteRenderer != null)
        {
            return !spriteRenderer.flipX;
        }
        return true;
    }
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
        if (movement.x != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = movement.x < 0;// 向左移动时翻转精灵
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
    }

    private void MovePlayer()
    {
        if (movement == Vector3.zero) return;

        Vector3 newPos = transform.position + movement * moveSpeed * Time.deltaTime;
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