using UnityEngine;

/// <summary>
/// 改进版全屏下雨特效
/// 使用自定义形状创建更明显的长方形雨滴
/// </summary>
public class RainEffect : MonoBehaviour
{
    [Header("雨滴设置")]
    [SerializeField] private int maxParticles = 2000;
    [SerializeField] private float emissionRate = 200f;
    [SerializeField] private float rainSpeed = 15f;
    [SerializeField] private float windForce = 0f; // 风力，可以做斜雨

    [Header("雨滴外观")]
    [SerializeField] private float rainWidth = 0.02f; // 雨滴宽度
    [SerializeField] private float rainLength = 0.3f; // 雨滴长度
    [SerializeField] private Color rainColor = new Color(0.9f, 0.9f, 1f, 0.7f);
    [SerializeField] private float brightnessVariation = 0.3f; // 亮度变化

    [Header("范围设置")]
    [SerializeField] private float spawnWidth = 40f;
    //[SerializeField] private float spawnHeight = 25f;

    private ParticleSystem rainParticleSystem;
    private Texture2D rainTexture;

    void Start()
    {
        CreateRainTexture();
        CreateRainSystem();
    }

    /// <summary>
    /// 创建一个长方形纹理用于雨滴
    /// </summary>
    void CreateRainTexture()
    {
        int width = 4;
        int height = 32; // 高宽比 8:1，创造细长效果

        rainTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        rainTexture.filterMode = FilterMode.Bilinear;
        rainTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[width * height];

        // 创建渐变效果的长方形
        for (int y = 0; y < height; y++)
        {
            float alpha = 1f;

            // 顶部和底部渐变
            if (y < 3)
                alpha = y / 3f;
            else if (y > height - 4)
                alpha = (height - y) / 3f;

            for (int x = 0; x < width; x++)
            {
                float edgeAlpha = 1f;

                // 边缘渐变
                if (x == 0 || x == width - 1)
                    edgeAlpha = 0.5f;

                pixels[y * width + x] = new Color(1f, 1f, 1f, alpha * edgeAlpha);
            }
        }

        rainTexture.SetPixels(pixels);
        rainTexture.Apply();
    }

    void CreateRainSystem()
    {
        // 创建粒子系统GameObject
        GameObject rainObject = new GameObject("RainParticles");
        rainObject.transform.SetParent(transform);
        rainObject.transform.localPosition = Vector3.zero;

        // 添加粒子系统组件
        rainParticleSystem = rainObject.AddComponent<ParticleSystem>();

        // 主模块
        var main = rainParticleSystem.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(2.5f, 4f);
        main.startSpeed = rainSpeed;
        main.startSize3D = true;
        main.startSizeX = new ParticleSystem.MinMaxCurve(rainWidth, rainWidth * 1.2f);
        main.startSizeY = new ParticleSystem.MinMaxCurve(rainLength, rainLength * 1.5f);
        main.startColor = new ParticleSystem.MinMaxGradient(
            rainColor * (1f - brightnessVariation),
            rainColor * (1f + brightnessVariation)
        );
        main.maxParticles = maxParticles;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = true;
        main.gravityModifier = 1f; // 重力效果

        // 发射模块
        var emission = rainParticleSystem.emission;
        emission.rateOverTime = emissionRate;

        // 形状模块 - 从顶部矩形区域发射
        var shape = rainParticleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Rectangle;
        shape.scale = new Vector3(spawnWidth, 0.1f, 1f);

        // 速度模块 - 添加风力效果
        var velocity = rainParticleSystem.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = windForce;

        // 旋转模块 - 让雨滴保持垂直
        var rotation = rainParticleSystem.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = 0;

        // 渲染模块
        var renderer = rainParticleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.alignment = ParticleSystemRenderSpace.World;
        renderer.sortingOrder = 100;

        // 创建材质
        Material rainMat = new Material(Shader.Find("Particles/Standard Unlit"));
        rainMat.SetTexture("_MainTex", rainTexture);
        rainMat.SetColor("_Color", Color.white);
        rainMat.SetFloat("_Mode", 3); // Transparent mode
        rainMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        rainMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        rainMat.SetInt("_ZWrite", 0);
        rainMat.DisableKeyword("_ALPHATEST_ON");
        rainMat.EnableKeyword("_ALPHABLEND_ON");
        rainMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        rainMat.renderQueue = 3000;

        renderer.material = rainMat;
    }

    /// <summary>
    /// 调整下雨强度
    /// </summary>
    public void SetRainIntensity(float intensity)
    {
        if (rainParticleSystem != null)
        {
            var emission = rainParticleSystem.emission;
            emission.rateOverTime = emissionRate * intensity;
        }
    }

    /// <summary>
    /// 设置风力（斜雨效果）
    /// </summary>
    public void SetWindForce(float wind)
    {
        windForce = wind;
        if (rainParticleSystem != null)
        {
            var velocity = rainParticleSystem.velocityOverLifetime;
            velocity.x = windForce;
        }
    }

    /// <summary>
    /// 开始下雨
    /// </summary>
    public void StartRain()
    {
        if (rainParticleSystem != null && !rainParticleSystem.isPlaying)
        {
            rainParticleSystem.Play();
        }
    }

    /// <summary>
    /// 停止下雨
    /// </summary>
    public void StopRain()
    {
        if (rainParticleSystem != null && rainParticleSystem.isPlaying)
        {
            rainParticleSystem.Stop();
        }
    }

    /// <summary>
    /// 渐变停止
    /// </summary>
    public void FadeOutRain(float duration = 2f)
    {
        if (rainParticleSystem != null)
        {
            StartCoroutine(FadeOutCoroutine(duration));
        }
    }

    private System.Collections.IEnumerator FadeOutCoroutine(float duration)
    {
        float elapsed = 0f;
        var emission = rainParticleSystem.emission;
        float originalRate = emission.rateOverTime.constant;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            emission.rateOverTime = Mathf.Lerp(originalRate, 0f, t);
            yield return null;
        }

        StopRain();
    }

    void OnDestroy()
    {
        if (rainTexture != null)
        {
            Destroy(rainTexture);
        }
    }
}
