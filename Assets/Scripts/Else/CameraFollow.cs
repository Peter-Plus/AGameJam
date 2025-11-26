using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField] private Transform target;

    [Header("跟随设置")]
    [SerializeField] private float smoothDampTime = 0.3f;  // 延迟平滑时间

    [Header("边界限制")]
    public float BoundaryL = -10;
    public float BoundaryR = 10;

    private Camera cam;
    private float cameraHalfWidth;
    private float velocityX;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cameraHalfWidth = cam.orthographicSize * cam.aspect;
    }

    // LateUpdate在玩家Update移动之后执行，确保同一帧内完成
    private void LateUpdate()
    {
        if (target == null) return;

        float minX = BoundaryL + cameraHalfWidth;
        float maxX = BoundaryR - cameraHalfWidth;
        float targetX = Mathf.Clamp(target.position.x, minX, maxX);

        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.SmoothDamp(transform.position.x, targetX, ref velocityX, smoothDampTime);
        transform.position = newPosition;
    }
}