using UnityEngine;

// Ha szeretnél egy másik fajta raycastot akkor erősen javaslom, hogy ezt az osztályt használd fel hozzá
// Sphere/Raycast osztályok már léteznek
public abstract class Sensor : MonoBehaviour, ISensor
{
    [SerializeField] protected Transform targetTransform;
    [SerializeField] protected LayerMask hitLayerMask;
    [SerializeField] protected float raycastLength;

    protected Vector3 origin = Vector3.zero;
    protected RaycastHit hitInfo;

    protected RaycastDirection direction;

    // A targettransform lehet saját maga is ez lesz alapból a rayOrigin
    public Sensor(Transform targetTransform)
    {
        if (targetTransform != null)
            this.targetTransform = targetTransform;
        else
            Debug.LogError("Sensor needs a target transform to work properly!");
    }

    // Manual casting for better performance
    // Ezt kell meghívnod akár Updateben akár gombnyomásra, hogy kiértékeld a raycastot
    public abstract void Cast();

    #region Getters
    public bool HasDetectedHit() => hitInfo.collider != null;
    public float GetDistance() => hitInfo.distance;
    public Vector3 GetNormal() => hitInfo.normal;
    public Vector3 GetPosition() => hitInfo.point;
    public Transform GetTransform() => hitInfo.transform;
    #endregion

    #region Setters
    public void SetRaycastDirection(RaycastDirection newDirection) => direction = newDirection;
    public void SetRaycastLength(float newLength) => raycastLength = newLength;
    public void SetCastOrigin(Vector3 pos) => origin = targetTransform.InverseTransformPoint(pos); // Ha az alap transformhoz képest szeretnéd eltolni a raycastot
    public void SetLayerMask(int layerMask) => hitLayerMask = layerMask;
    #endregion

    public enum RaycastDirection
    {
        Forward,
        Right,
        Up,
        Backward,
        Left,
        Down
    }
    protected Vector3 GetRaycastDirection()
    {
        return direction switch
        {
            RaycastDirection.Forward => targetTransform.forward,
            RaycastDirection.Right => targetTransform.right,
            RaycastDirection.Up => targetTransform.up,
            RaycastDirection.Backward => -targetTransform.forward,
            RaycastDirection.Left => -targetTransform.right,
            RaycastDirection.Down => -targetTransform.up,
            _ => Vector3.one
        };
    }

    public virtual void DrawDebug() { }
}