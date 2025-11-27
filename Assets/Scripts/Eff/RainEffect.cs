using UnityEngine;

/// <summary>
/// 改进版全屏下雨特效 - 带水花效果
/// </summary>
public class RainEffect : MonoBehaviour
{
    [Header("雨滴设置")]
    [SerializeField] private int maxParticles = 2000;
    [SerializeField] private float emissionRate = 200f;
    [SerializeField] private float rainSpeed = 15f;
    [SerializeField] private float windForce = 0f;

    [Header("雨滴外观")]
    [SerializeField] private float rainWidth = 0.02f;
    [SerializeField] private float rainLength = 0.3f;
    [SerializeField] private Color rainColor = new Color(0.9f, 0.9f, 1f, 0.7f);
    [SerializeField] private float brightnessVariation = 0.3f;

    [Header("范围设置")]
    [SerializeField] private float spawnWidth = 40f;

    [Header("水花设置")]
    [SerializeField] private bool enableSplash = true;
    [SerializeField] private int maxSplashParticles = 500;
    [SerializeField] private float splashEmissionRate = 50f; // 水花生成频率
    [SerializeField] private float splashSize = 0.08f; // 水花大小
    [SerializeField] private Color splashColor = new Color(0.8f, 0.9f, 1f, 0.6f);
    [SerializeField] private int sWidth = 32;  // 水花纹理宽度
    [SerializeField] private int sHeight = 12; // 水花纹理高度

    [Header("相机引用")]
    [SerializeField] private Camera rainCamera; // 拖入RainCamera

    private ParticleSystem rainParticleSystem;
    private ParticleSystem splashParticleSystem;
    private Texture2D rainTexture;
    private Texture2D splashTexture;
    private float splashYMin; // 水花生成的最低Y坐标（0%）
    private float splashYMax; // 水花生成的最高Y坐标（25%）

    void Start()
    {
        CreateRainTexture();
        CreateSplashTexture();
        CreateRainSystem();

        if (enableSplash)
        {
            CalculateSplashPosition();
            CreateSplashSystem();
        }
    }

    /// <summary>
    /// 计算水花生成位置范围（相机视口0-25%区间）
    /// </summary>
    void CalculateSplashPosition()
    {
        if (rainCamera == null)
        {
            Debug.LogError("RainCamera引用为空！请在Inspector中拖入RainCamera");
            return;
        }

        // 获取相机视口下边界（0%）和25%处的世界坐标
        Vector3 viewportPointMin = new Vector3(0.5f, 0f, rainCamera.nearClipPlane + 5f);
        Vector3 viewportPointMax = new Vector3(0.5f, 0.25f, rainCamera.nearClipPlane + 5f);

        Vector3 worldPointMin = rainCamera.ViewportToWorldPoint(viewportPointMin);
        Vector3 worldPointMax = rainCamera.ViewportToWorldPoint(viewportPointMax);

        splashYMin = worldPointMin.y;
        splashYMax = worldPointMax.y;
    }

    void CreateRainTexture()
    {
        int width = 4;
        int height = 32;

        rainTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        rainTexture.filterMode = FilterMode.Bilinear;
        rainTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            float alpha = 1f;

            if (y < 3)
                alpha = y / 3f;
            else if (y > height - 4)
                alpha = (height - y) / 3f;

            for (int x = 0; x < width; x++)
            {
                float edgeAlpha = 1f;

                if (x == 0 || x == width - 1)
                    edgeAlpha = 0.5f;

                pixels[y * width + x] = new Color(1f, 1f, 1f, alpha * edgeAlpha);
            }
        }

        rainTexture.SetPixels(pixels);
        rainTexture.Apply();
    }

    /// <summary>
    /// 创建空心椭圆环纹理用于水花（适配2.5D视角）
    /// </summary>
    void CreateSplashTexture()
    {
        int width = sWidth;
        int height = sHeight;  // 高度小于宽度，形成扁椭圆

        splashTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        splashTexture.filterMode = FilterMode.Bilinear;
        splashTexture.wrapMode = TextureWrapMode.Clamp;

        Color[] pixels = new Color[width * height];

        Vector2 center = new Vector2(width / 2f, height / 2f);
        float outerRadiusX = width / 2f - 1f;      // 外椭圆水平半径
        float outerRadiusY = height / 2f - 1f;     // 外椭圆垂直半径
        float lineWidth = 2f;                       // 圆环线宽
        float innerRadiusX = outerRadiusX - lineWidth;
        float innerRadiusY = outerRadiusY - lineWidth;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 计算当前像素到中心的椭圆距离
                float dx = (x - center.x) / outerRadiusX;
                float dy = (y - center.y) / outerRadiusY;
                float outerDistance = Mathf.Sqrt(dx * dx + dy * dy);

                dx = (x - center.x) / innerRadiusX;
                dy = (y - center.y) / innerRadiusY;
                float innerDistance = Mathf.Sqrt(dx * dx + dy * dy);

                float alpha = 0f;

                // 如果在椭圆环范围内
                if (innerDistance >= 1f && outerDistance <= 1f)
                {
                    // 计算到环边界的距离用于柔和边缘
                    float distToOuter = 1f - outerDistance;
                    float distToInner = innerDistance - 1f;
                    float edgeDist = Mathf.Min(distToOuter, distToInner);

                    // 创建平滑边缘
                    float fadeWidth = 0.3f;
                    alpha = Mathf.Clamp01(edgeDist / fadeWidth);
                    alpha = Mathf.Pow(alpha, 0.5f); // 柔和边缘
                }

                pixels[y * width + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        splashTexture.SetPixels(pixels);
        splashTexture.Apply();
    }

    void CreateRainSystem()
    {
        GameObject rainObject = new GameObject("RainParticles");
        rainObject.transform.SetParent(transform);
        rainObject.transform.localPosition = Vector3.zero;
        rainObject.layer = LayerMask.NameToLayer("Rain"); // 设置为Rain层

        rainParticleSystem = rainObject.AddComponent<ParticleSystem>();

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
        main.gravityModifier = 1f;

        var emission = rainParticleSystem.emission;
        emission.rateOverTime = emissionRate;

        var shape = rainParticleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Rectangle;
        shape.scale = new Vector3(spawnWidth, 0.1f, 1f);

        var velocity = rainParticleSystem.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.x = windForce;

        var rotation = rainParticleSystem.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = 0;

        var renderer = rainParticleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.alignment = ParticleSystemRenderSpace.World;
        renderer.sortingOrder = 100;

        Material rainMat = new Material(Shader.Find("Particles/Standard Unlit"));
        rainMat.SetTexture("_MainTex", rainTexture);
        rainMat.SetColor("_Color", Color.white);
        rainMat.SetFloat("_Mode", 3);
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
    /// 创建水花粒子系统 - 平面散开的圆形效果
    /// </summary>
    void CreateSplashSystem()
    {
        GameObject splashObject = new GameObject("SplashParticles");
        splashObject.transform.SetParent(transform);
        // 水花的Y坐标设为范围中心
        float centerY = (splashYMin + splashYMax) / 2f;
        splashObject.transform.position = new Vector3(0, centerY, 0);
        splashObject.layer = LayerMask.NameToLayer("Rain");

        splashParticleSystem = splashObject.AddComponent<ParticleSystem>();

        var main = splashParticleSystem.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.3f, 0.5f); // 水波纹扩散时间
        main.startSpeed = new ParticleSystem.MinMaxCurve(0f, 0f); // 不需要速度，靠大小变化实现扩散
        main.startSize = new ParticleSystem.MinMaxCurve(splashSize * 0.3f, splashSize * 0.5f); // 初始较小
        main.startColor = splashColor;
        main.maxParticles = maxSplashParticles;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.loop = true;
        main.gravityModifier = 0f; // 不受重力

        var emission = splashParticleSystem.emission;
        emission.rateOverTime = splashEmissionRate;

        // 形状模块 - 在0-25%区间内随机生成
        var shape = splashParticleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        float heightRange = splashYMax - splashYMin;
        shape.scale = new Vector3(spawnWidth, heightRange, 0.1f);

        // 大小变化 - 圆环向外扩散（关键！）
        var sizeOverLifetime = splashParticleSystem.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0.5f);    // 初始较小
        sizeCurve.AddKey(0.5f, 2.5f);  // 快速扩大（模拟水波纹扩散）
        sizeCurve.AddKey(1f, 4f);      // 继续扩大到最大
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // 水波纹扩散时逐渐消失
        var colorOverLifetime = splashParticleSystem.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0.0f),
                new GradientColorKey(Color.white, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),   // 起始完全不透明
                new GradientAlphaKey(0.7f, 0.5f),   // 扩散中期保持可见
                new GradientAlphaKey(0.0f, 1.0f)    // 扩散到最大时完全消失
            }
        );
        colorOverLifetime.color = gradient;

        var renderer = splashParticleSystem.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = 101;

        // 使用自定义的圆形纹理
        Material splashMat = new Material(Shader.Find("Particles/Standard Unlit"));
        splashMat.SetTexture("_MainTex", splashTexture);
        splashMat.SetColor("_Color", Color.white);
        splashMat.SetFloat("_Mode", 3);
        splashMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        splashMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        splashMat.SetInt("_ZWrite", 0);
        splashMat.DisableKeyword("_ALPHATEST_ON");
        splashMat.EnableKeyword("_ALPHABLEND_ON");
        splashMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        splashMat.renderQueue = 3000;

        renderer.material = splashMat;
    }

    public void SetRainIntensity(float intensity)
    {
        if (rainParticleSystem != null)
        {
            var emission = rainParticleSystem.emission;
            emission.rateOverTime = emissionRate * intensity;
        }

        if (splashParticleSystem != null)
        {
            var emission = splashParticleSystem.emission;
            emission.rateOverTime = splashEmissionRate * intensity;
        }
    }

    public void SetWindForce(float wind)
    {
        windForce = wind;
        if (rainParticleSystem != null)
        {
            var velocity = rainParticleSystem.velocityOverLifetime;
            velocity.x = windForce;
        }
    }

    public void StartRain()
    {
        if (rainParticleSystem != null && !rainParticleSystem.isPlaying)
        {
            rainParticleSystem.Play();
        }
        if (splashParticleSystem != null && !splashParticleSystem.isPlaying)
        {
            splashParticleSystem.Play();
        }
    }

    public void StopRain()
    {
        if (rainParticleSystem != null && rainParticleSystem.isPlaying)
        {
            rainParticleSystem.Stop();
        }
        if (splashParticleSystem != null && splashParticleSystem.isPlaying)
        {
            splashParticleSystem.Stop();
        }
    }

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
        var rainEmission = rainParticleSystem.emission;
        var splashEmission = splashParticleSystem != null ? splashParticleSystem.emission : default;
        float originalRainRate = rainEmission.rateOverTime.constant;
        float originalSplashRate = splashEmission.rateOverTime.constant;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rainEmission.rateOverTime = Mathf.Lerp(originalRainRate, 0f, t);
            if (splashParticleSystem != null)
            {
                splashEmission.rateOverTime = Mathf.Lerp(originalSplashRate, 0f, t);
            }
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
        if (splashTexture != null)
        {
            Destroy(splashTexture);
        }
    }
}