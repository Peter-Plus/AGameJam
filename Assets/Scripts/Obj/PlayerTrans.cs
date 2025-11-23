using UnityEngine;

public class PlayerTrans : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("远近缩放设置")]
    [SerializeField] private float scaleSpeed = 0.1f;  // 缩放速度

    [Header("移动范围限制")]
    [SerializeField] private float minY = -2f;
    [SerializeField] private float maxY = 2f;

    [Header("组件引用")]
    public SpriteRenderer spriteRenderer;

    private Vector3 movement;
    private float baseYPosition;  // 记录初始Y坐标作为基准
    private Vector3 baseScale;    // 记录初始缩放


    //外部API-获取面朝向IsFacingRight
    public bool IsFacingRight()
    {
        if (spriteRenderer != null)
        {
            return !spriteRenderer.flipX;
        }
        return true; // 默认朝右
    }

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // 记录初始状态
        baseYPosition = transform.position.y;
        baseScale = transform.localScale;
    }

    private void Update()
    {
        HandleInput();
        UpdateScale();
        UpdateSortingOrder();
        FlipSprite();
    }

    private void FlipSprite()
    {
        if (movement.x != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = movement.x < 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movement = new Vector3(horizontal, vertical, 0f).normalized;
    }

    private void MovePlayer()
    {
        Vector3 newPosition = transform.position + movement * moveSpeed * Time.fixedDeltaTime;
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        transform.position = newPosition;
    }

    private void UpdateScale()
    {
        // 计算相对于初始位置的Y偏移
        float yOffset = transform.position.y - baseYPosition;

        // 往下走变大，往上走变小
        float scaleFactor = Mathf.Exp(-yOffset * scaleSpeed);

        // 基于初始缩放进行调整
        transform.localScale = baseScale * scaleFactor;
    }

    private void UpdateSortingOrder()
    {
        if (spriteRenderer != null)
        {
            // Y值越小（越近）渲染层级越高
            spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
        }
    }
}