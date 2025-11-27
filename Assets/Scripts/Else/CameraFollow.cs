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

    [Header("远景设置")]
    [SerializeField] private Transform[] parallaxBackgrounds; // 背景数组
    [SerializeField] private float[] parallaxFactors; // 对应的速度系数

    private Camera cam;
    private float cameraHalfWidth;
    private float velocityX;
    private Vector3 lastCameraPosition;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cameraHalfWidth = cam.orthographicSize * cam.aspect;
        lastCameraPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        float minX = BoundaryL + cameraHalfWidth;
        float maxX = BoundaryR - cameraHalfWidth;
        float targetX = Mathf.Clamp(target.position.x, minX, maxX);//左右边界限制镜头

        Vector3 newPosition = transform.position;
        newPosition.x = Mathf.SmoothDamp(transform.position.x, targetX, ref velocityX, smoothDampTime);
        Vector3 cameraDelta = newPosition - lastCameraPosition;
        //利用摄像机移动增量实现远景视差效果
        //每帧都准确计算摄像机的移动增量并给BK层传递这个增量，能保证BK和摄像机的移动完全同步
        UpdateBK(cameraDelta);
        transform.position = newPosition;
        lastCameraPosition = transform.position;
    }

    private void UpdateBK(Vector3 cameraDelta)
    {
        // 参数是摄像机的移动增量
        if(parallaxBackgrounds==null || parallaxFactors==null) return;
        int count = parallaxBackgrounds.Length;//有越界风险
        for(int i=0;i<count;i++)
        {
            Vector3 bgDelta = cameraDelta * parallaxFactors[i];
            parallaxBackgrounds[i].position += bgDelta;
        }
    }
}