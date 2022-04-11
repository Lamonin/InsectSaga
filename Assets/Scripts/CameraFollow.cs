using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float speed;
    public Vector2 offset;
    [Tooltip("Зеленая рамка - границы центра камеры, желтая - границы видимости камеры")]
    public Vector4 bounds;
    public Transform targetTransform;
    public Transform cameraTransform;
    
    private Vector3 CalculateCameraOffset()
    {
        Vector3 target = new Vector3(targetTransform.position.x + offset.x, targetTransform.position.y + offset.y, -10);
        target.x = Mathf.Clamp(target.x, bounds.x, bounds.z);
        target.y = Mathf.Clamp(target.y, bounds.w, bounds.y);
        return target;
    }
    
    private void Awake()
    {
        cameraTransform.position = CalculateCameraOffset();
    }

    void LateUpdate()
    {
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, CalculateCameraOffset(), speed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform is null || bounds == Vector4.zero) return;

        //Draw green rect
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(bounds.x, bounds.y), new Vector2(bounds.z, bounds.y));
        Gizmos.DrawLine(new Vector2(bounds.x, bounds.w), new Vector2(bounds.x, bounds.y));
        Gizmos.DrawLine(new Vector2(bounds.z, bounds.y), new Vector2(bounds.z, bounds.w));
        Gizmos.DrawLine(new Vector2(bounds.x, bounds.w), new Vector2(bounds.z, bounds.w));
        
        //Draw yellow rect
        Gizmos.color = Color.yellow;
        var camComponent = cameraTransform.GetComponent<Camera>();
        var vHSeen = camComponent.orthographicSize;
        var vWSeen = vHSeen * camComponent.aspect;
        Gizmos.DrawLine(new Vector2(bounds.x-vWSeen, bounds.y+vHSeen), new Vector2(bounds.z+vWSeen, bounds.y+vHSeen));
        Gizmos.DrawLine(new Vector2(bounds.x-vWSeen, bounds.w-vHSeen), new Vector2(bounds.x-vWSeen, bounds.y+vHSeen));
        Gizmos.DrawLine(new Vector2(bounds.z+vWSeen, bounds.y+vHSeen), new Vector2(bounds.z+vWSeen, bounds.w-vHSeen));
        Gizmos.DrawLine(new Vector2(bounds.x-vWSeen, bounds.w-vHSeen), new Vector2(bounds.z+vWSeen, bounds.w-vHSeen));
    }
}
