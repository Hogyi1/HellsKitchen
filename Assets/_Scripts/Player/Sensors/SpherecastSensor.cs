using UnityEngine;

public class SpherecastSensor : Sensor
{
    float radius;
    public SpherecastSensor(Transform targetTransform) : base(targetTransform)
    {
        this.targetTransform = targetTransform;
    }

    public void SetSphereRadius(float newRadius) => radius = newRadius;

    public override void Cast()
    {
        hitInfo = new RaycastHit();

        Vector3 worldOrigin = targetTransform.TransformPoint(origin);
        Vector3 worldDirection = GetRaycastDirection();

        Physics.SphereCast(worldOrigin, radius, worldDirection, out hitInfo, raycastLength, hitLayerMask, QueryTriggerInteraction.Ignore);
    }

    // Chatgpt jóvoltából debug
    /// <summary>
    /// Kirajzolja a ray-t, a sugarat és a találati pontot (ha van).
    /// Scene nézetben látszik Play közben.
    /// </summary>
    public override void DrawDebug()
    {
        if (targetTransform == null) return;

        Vector3 worldOrigin = targetTransform.TransformPoint(origin);
        Vector3 worldDirection = GetRaycastDirection().normalized;

        bool hit = HasDetectedHit();
        Color color = hit ? Color.red : Color.green;

        float drawLength = hit ? hitInfo.distance : raycastLength;
        Vector3 endPoint = worldOrigin + worldDirection * drawLength;

        // Folyamatos vonal az origin-től a végpontig
        Debug.DrawLine(worldOrigin, endPoint, color);

        // Kiindulási gömb
        DrawWireSphere(worldOrigin, radius, color);

        // Ha talált, rajzoljuk a pontot és a normált
        if (hit)
        {
            // Találati pont kiemelése
            DrawWireSphere(hitInfo.point, 0.05f, Color.yellow);

            // Normál iránya
            Debug.DrawRay(hitInfo.point, hitInfo.normal * 0.5f, Color.cyan);
        }

        // Opcionális: a ray végpontján egy kis gömb, ha nincs találat
        else
        {
            DrawWireSphere(endPoint, 0.05f, Color.gray);
        }
    }

    void DrawWireSphere(Vector3 position, float sphereRadius, Color color)
    {
        int segments = 12;
        float step = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float a0 = step * i * Mathf.Deg2Rad;
            float a1 = step * (i + 1) * Mathf.Deg2Rad;

            // XY sík
            Vector3 p0 = position + new Vector3(Mathf.Cos(a0), Mathf.Sin(a0), 0) * sphereRadius;
            Vector3 p1 = position + new Vector3(Mathf.Cos(a1), Mathf.Sin(a1), 0) * sphereRadius;
            Debug.DrawLine(p0, p1, color);

            // YZ sík
            p0 = position + new Vector3(0, Mathf.Cos(a0), Mathf.Sin(a0)) * sphereRadius;
            p1 = position + new Vector3(0, Mathf.Cos(a1), Mathf.Sin(a1)) * sphereRadius;
            Debug.DrawLine(p0, p1, color);

            // ZX sík
            p0 = position + new Vector3(Mathf.Cos(a0), 0, Mathf.Sin(a0)) * sphereRadius;
            p1 = position + new Vector3(Mathf.Cos(a1), 0, Mathf.Sin(a1)) * sphereRadius;
            Debug.DrawLine(p0, p1, color);
        }
    }
}