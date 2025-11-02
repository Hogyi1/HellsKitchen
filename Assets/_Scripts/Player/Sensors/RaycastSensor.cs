using UnityEngine;

public class RaycastSensor : Sensor
{
    public RaycastSensor(Transform targetTransform) : base(targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    public override void Cast()
    {
        Vector3 worldOrigin = targetTransform.TransformPoint(origin);
        Vector3 worldDirection = GetRaycastDirection();

        Physics.Raycast(worldOrigin, worldDirection, out hitInfo, raycastLength, hitLayerMask, QueryTriggerInteraction.Ignore);
    }

    // GPT kedves gyors munkáját láthatjuk
    public override void DrawDebug()
    {
        if (targetTransform == null) return;

        Vector3 worldOrigin = targetTransform.TransformPoint(origin);
        Vector3 worldDirection = GetRaycastDirection().normalized;

        bool hit = HasDetectedHit();
        Color color = hit ? Color.red : Color.green;

        float drawLength = hit ? hitInfo.distance : raycastLength;
        Vector3 endPoint = worldOrigin + worldDirection * drawLength;

        Debug.DrawLine(worldOrigin, endPoint, color);

        if (hit)
        {
            Debug.DrawRay(hitInfo.point, hitInfo.normal * 0.5f, Color.cyan);
            DrawCross(hitInfo.point, 0.05f, Color.yellow);
        }
        else
        {
            DrawCross(endPoint, 0.05f, Color.gray);
        }
    }

    void DrawCross(Vector3 position, float size, Color color)
    {
        Debug.DrawLine(position + Vector3.up * size, position - Vector3.up * size, color);
        Debug.DrawLine(position + Vector3.right * size, position - Vector3.right * size, color);
        Debug.DrawLine(position + Vector3.forward * size, position - Vector3.forward * size, color);
    }
}
