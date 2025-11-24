using UnityEngine;

public class CrescentSlashEffect : MonoBehaviour
{
    public static CrescentSlashEffect Instance;

    [Header("特效材质")]
    public Material slashMaterial;

    [Header("月牙参数")]
    public Color slashColor = new Color(0.8f, 0.9f, 1f);
    public float crescentRadius = 1.5f;
    public float crescentThickness = 0.3f;
    public float duration = 0.4f;
    public int segments = 30;

    void Awake()
    {
        Instance = this;
    }

    public void PlayCrescentSlash(Vector3 position, float angle)
    {
        StartCoroutine(CrescentCoroutine(position, angle));
    }

    private System.Collections.IEnumerator CrescentCoroutine(Vector3 pos, float angle)
    {
        GameObject slashObj = new GameObject("CrescentSlash");
        slashObj.transform.position = pos;
        slashObj.transform.rotation = Quaternion.Euler(0, 0, angle);

        MeshFilter mf = slashObj.AddComponent<MeshFilter>();
        MeshRenderer mr = slashObj.AddComponent<MeshRenderer>();

        mr.material = slashMaterial;
        mr.sortingLayerName = "Effects"; // 确保在正确的层级
        mr.sortingOrder = 100;

        mf.mesh = CreateCrescentMesh();

        // 动画：放大+淡出
        float elapsed = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        Color startColor = slashColor;
        Color endColor = new Color(slashColor.r, slashColor.g, slashColor.b, 0);

        MaterialPropertyBlock props = new MaterialPropertyBlock();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 缓动曲线
            float scaleCurve = Mathf.Sin(t * Mathf.PI * 0.5f);
            slashObj.transform.localScale = Vector3.Lerp(startScale, endScale, scaleCurve);

            // 颜色淡出
            Color currentColor = Color.Lerp(startColor, endColor, t);
            props.SetColor("_Color", currentColor);
            mr.SetPropertyBlock(props);

            yield return null;
        }

        Destroy(slashObj);
    }

    private Mesh CreateCrescentMesh()
    {
        Mesh mesh = new Mesh();

        // 月牙是两个弧形之间的差
        float innerRadius = crescentRadius - crescentThickness;
        float outerRadius = crescentRadius;

        // 月牙弧度范围（-60度到60度，共120度）
        float startAngle = -60f * Mathf.Deg2Rad;
        float endAngle = 60f * Mathf.Deg2Rad;

        int vertCount = segments * 2 + 2;
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];
        int[] triangles = new int[segments * 6];

        // 生成顶点
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(startAngle, endAngle, t);

            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            // 外圈顶点
            vertices[i * 2] = new Vector3(cos * outerRadius, sin * outerRadius, 0);
            uv[i * 2] = new Vector2(t, 1);

            // 内圈顶点
            vertices[i * 2 + 1] = new Vector3(cos * innerRadius, sin * innerRadius, 0);
            uv[i * 2 + 1] = new Vector2(t, 0);
        }

        // 生成三角形
        for (int i = 0; i < segments; i++)
        {
            int idx = i * 6;
            int v = i * 2;

            // 第一个三角形
            triangles[idx] = v;
            triangles[idx + 1] = v + 2;
            triangles[idx + 2] = v + 1;

            // 第二个三角形
            triangles[idx + 3] = v + 1;
            triangles[idx + 4] = v + 2;
            triangles[idx + 5] = v + 3;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}