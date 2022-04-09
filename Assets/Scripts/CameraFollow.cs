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

    void Update()
    {
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, CalculateCameraOffset(), speed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (cameraTransform is null || bounds == Vector4.zero) return;
        Vector2 pos = cameraTransform.position;
        
        //Draw green rect
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos+new Vector2(bounds.x, bounds.y), pos+new Vector2(bounds.z, bounds.y));
        Gizmos.DrawLine(pos+new Vector2(bounds.x, bounds.w), pos+new Vector2(bounds.x, bounds.y));
        Gizmos.DrawLine(pos+new Vector2(bounds.z, bounds.y), pos+new Vector2(bounds.z, bounds.w));
        Gizmos.DrawLine(pos+new Vector2(bounds.x, bounds.w), pos+new Vector2(bounds.z, bounds.w));
        
        //Draw yellow rect
        Gizmos.color = Color.yellow;
        var camComponent = cameraTransform.GetComponent<Camera>();
        var vHSeen = camComponent.orthographicSize;
        var vWSeen = vHSeen * camComponent.aspect;
        Gizmos.DrawLine(pos+new Vector2(bounds.x-vWSeen, bounds.y+vHSeen), pos+new Vector2(bounds.z+vWSeen, bounds.y+vHSeen));
        Gizmos.DrawLine(pos+new Vector2(bounds.x-vWSeen, bounds.w-vHSeen), pos+new Vector2(bounds.x-vWSeen, bounds.y+vHSeen));
        Gizmos.DrawLine(pos+new Vector2(bounds.z+vWSeen, bounds.y+vHSeen), pos+new Vector2(bounds.z+vWSeen, bounds.w-vHSeen));
        Gizmos.DrawLine(pos+new Vector2(bounds.x-vWSeen, bounds.w-vHSeen), pos+new Vector2(bounds.z+vWSeen, bounds.w-vHSeen));
    }
}
