using UnityEngine;

/// <summary>
/// Sprite发光效果控制器
/// 挂载到带有SpriteRenderer的物体上，配合SpriteGlow shader使用
/// </summary>
public class SpriteGlowController : MonoBehaviour
{
    #region 成员属性
    [Header("发光设置")]
    [SerializeField] private Color glowColor = new Color(1f, 0.9f, 0.3f, 1f); // 默认金黄色
    [SerializeField] private float maxGlowIntensity = 1.5f; // 最大发光强度
    [SerializeField] private float glowWidth = 0.02f; // 发光宽度

    [Header("动画设置")]
    [SerializeField] private float fadeInSpeed = 8f;  // 淡入速度
    [SerializeField] private float fadeOutSpeed = 5f; // 淡出速度
    [SerializeField] private float pulseSpeed = 3f;   // 脉冲速度
    [SerializeField] private float pulseAmount = 0.3f; // 脉冲幅度

    private SpriteRenderer spriteRenderer;
    private Material glowMaterial;
    private Material originalMaterial;
    private float currentIntensity = 0f;
    private bool isGlowing = false;

    // Shader属性ID缓存
    private static readonly int GlowColorID = Shader.PropertyToID("_GlowColor");
    private static readonly int GlowIntensityID = Shader.PropertyToID("_GlowIntensity");
    private static readonly int GlowWidthID = Shader.PropertyToID("_GlowWidth");
    private static readonly int PulseSpeedID = Shader.PropertyToID("_PulseSpeed");
    private static readonly int PulseAmountID = Shader.PropertyToID("_PulseAmount");
    #endregion

    #region 生命周期
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("[SpriteGlowController] 未找到SpriteRenderer组件!");
            enabled = false;
            return;
        }
        InitializeMaterial();
    }

    private void Update()
    {
        UpdateGlowIntensity();
    }

    private void OnDestroy()
    {
        // 清理动态创建的材质
        if (glowMaterial != null)
        {
            Destroy(glowMaterial);
        }
    }
    #endregion

    #region API
    /// <summary>
    /// 开启发光效果
    /// </summary>
    public void EnableGlow()
    {
        isGlowing = true;
    }

    /// <summary>
    /// 关闭发光效果
    /// </summary>
    public void DisableGlow()
    {
        isGlowing = false;
    }

    /// <summary>
    /// 设置发光颜色
    /// </summary>
    public void SetGlowColor(Color color)
    {
        glowColor = color;
        if (glowMaterial != null)
        {
            glowMaterial.SetColor(GlowColorID, glowColor);
        }
    }

    /// <summary>
    /// 立即设置发光强度（跳过渐变）
    /// </summary>
    public void SetGlowIntensityImmediate(float intensity)
    {
        currentIntensity = intensity;
        if (glowMaterial != null)
        {
            glowMaterial.SetFloat(GlowIntensityID, currentIntensity);
        }
    }
    #endregion

    #region 内部方法
    private void InitializeMaterial()
    {
        // 从Resources加载材质
        Material glowMatPrefab = Resources.Load<Material>("Eff/SpriteGlowMat");
        if (glowMatPrefab == null)
        {
            Debug.LogError("[SpriteGlowController] 未找到 Resources/Eff/SpriteGlowMat 材质!");
            enabled = false;
            return;
        }

        // 创建材质实例
        glowMaterial = new Material(glowMatPrefab);
        glowMaterial.SetColor(GlowColorID, glowColor);
        glowMaterial.SetFloat(GlowIntensityID, 0f);

        // 应用材质
        spriteRenderer.material = glowMaterial;
    }

    private void UpdateGlowIntensity()
    {
        if (glowMaterial == null) return;

        // 根据状态平滑过渡发光强度
        float targetIntensity = isGlowing ? maxGlowIntensity : 0f;
        float speed = isGlowing ? fadeInSpeed : fadeOutSpeed;

        currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, speed * Time.deltaTime);
        glowMaterial.SetFloat(GlowIntensityID, currentIntensity);
    }
    #endregion
}