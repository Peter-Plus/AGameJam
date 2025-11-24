using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField] private Transform target;  // Player的Transform

    [Header("边界限制")]
    public float BoundaryL = -10;  // 左边界
    public float BoundaryR = 10;   // 右边界

    private Camera cam;
    private float cameraHalfWidth;

    private void Awake()
    {
        cam = GetComponent<Camera>();//必须在Awake里获取而非直接赋值
        // 计算摄像机视野的一半宽度
        cameraHalfWidth = cam.orthographicSize * cam.aspect;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 获取当前位置
        Vector3 newPosition = transform.position;

        // 跟随玩家的X坐标
        newPosition.x = target.position.x;

        // 限制摄像机边界
        // 左边缘不能小于BoundaryL
        float minX = BoundaryL + cameraHalfWidth;
        // 右边缘不能大于BoundaryR
        float maxX = BoundaryR - cameraHalfWidth;

        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

        transform.position = newPosition;
    }
}