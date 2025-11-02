using UnityEngine;

public interface ISensor
{
    void Cast();
    float GetDistance();
    Vector3 GetNormal();
    Vector3 GetPosition();
    Transform GetTransform();
    bool HasDetectedHit();
    void SetCastOrigin(Vector3 pos);
    void SetRaycastDirection(Sensor.RaycastDirection newDirection);
    void SetRaycastLength(float newLength);
}