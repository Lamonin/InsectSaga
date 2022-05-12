using UnityEngine;

public class PathLine : MonoBehaviour
{
    public Transform[] points;

    void OnDrawGizmos()
    {
        if (points is null) return;
        for (int i = 1; i < points.Length; i++)
        {
            Gizmos.DrawLine(points[i-1].position, points[i].position);
        }
    }
    
    public Transform this[int i] => points[i];
}
