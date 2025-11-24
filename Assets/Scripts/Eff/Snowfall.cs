using UnityEngine;

public class Snowfall : MonoBehaviour
{
    [Header("材质")]
    public Material snowMaterial;

    [Header("基础参数")]
    public int maxSnowflakes = 800;
    public float spawnWidth = 25f;// 生成范围宽度
    public float fallSpeed = 1.5f;// 下落速度
    public Vector2 snowflakeSize = new Vector2(0.1f, 0.2f); // 尺寸

    [Header("风力效果")]
    public bool enableWind = true;
    public float windStrength = 1f;
    public float windChangeSpeed = 0.5f;

    [Header("深度层次")]
    public bool enableDepthLayers = true;

    private ParticleSystem[] snowLayers;
    private float currentWindX = 0f;

    void Start()
    {
        if (enableDepthLayers)
            CreateLayeredSnow();
        else
            CreateSingleLayerSnow();
    }

    void Update()
    {
        if (enableWind)
            UpdateWind();
    }

    void CreateLayeredSnow()
    {
        // 创建3层雪花（近、中、远）
        snowLayers = new ParticleSystem[3];

        for (int i = 0; i < 3; i++)
        {
            GameObject layer = new GameObject($"SnowLayer_{i}");
            layer.transform.SetParent(Camera.main.transform);
            layer.transform.localPosition = new Vector3(0, 5f, 8f + i * 2f);

            ParticleSystem ps = layer.AddComponent<ParticleSystem>();
            snowLayers[i] = ps;

            float depthScale = 1f - i * 0.3f; // 远处的小一些
            float speedScale = 1f - i * 0.2f; // 远处慢一些

            SetupParticleSystem(ps, depthScale, speedScale);
        }
    }

    void CreateSingleLayerSnow()
    {
        GameObject snowObj = new GameObject("Snowfall");
        snowObj.transform.SetParent(Camera.main.transform);
        snowObj.transform.localPosition = new Vector3(0, 5f, 10f);

        ParticleSystem ps = snowObj.AddComponent<ParticleSystem>();
        SetupParticleSystem(ps, 1f, 1f);
    }

    void SetupParticleSystem(ParticleSystem ps, float sizeScale, float speedScale)
    {
        var main = ps.main;
        main.startLifetime = 15f;
        main.startSpeed = fallSpeed * speedScale;
        main.startSize = new ParticleSystem.MinMaxCurve(snowflakeSize.x * sizeScale, snowflakeSize.y * sizeScale);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, 360f * Mathf.Deg2Rad);
        main.startColor = new Color(1f, 1f, 1f, 0.7f);
        main.maxParticles = maxSnowflakes / (enableDepthLayers ? 3 : 1);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = true;
        main.loop = true;

        var emission = ps.emission;
        emission.rateOverTime = (maxSnowflakes / (enableDepthLayers ? 3 : 1)) / 12f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(spawnWidth, 0.1f, 1f);

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.y = -fallSpeed * speedScale;

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.5f * speedScale;
        noise.frequency = 0.3f;
        noise.scrollSpeed = 0.5f;

        var rotation = ps.rotationOverLifetime;
        rotation.enabled = true;
        rotation.z = new ParticleSystem.MinMaxCurve(-45f * Mathf.Deg2Rad, 45f * Mathf.Deg2Rad);

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.material = snowMaterial ?? CreateDefaultSnowMaterial();
        renderer.sortingLayerName = "Effects";
        renderer.sortingOrder = 200;
    }

    void UpdateWind()
    {
        // 风力随时间变化
        currentWindX = Mathf.Sin(Time.time * windChangeSpeed) * windStrength;

        if (snowLayers != null)
        {
            foreach (var ps in snowLayers)
            {
                if (ps != null)
                {
                    var velocity = ps.velocityOverLifetime;
                    velocity.x = currentWindX;
                }
            }
        }
    }

    Material CreateDefaultSnowMaterial()
    {
        Material mat = new Material(Shader.Find("Particles/Standard Unlit"));
        mat.SetColor("_Color", new Color(1, 1, 1, 0.8f));
        mat.EnableKeyword("_ALPHABLEND_ON");
        return mat;
    }
}