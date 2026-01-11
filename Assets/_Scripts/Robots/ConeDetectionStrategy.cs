using Unity.VisualScripting;
using UnityEngine;

public class ConeDetectionStrategy : IdetectionStrategy
{
    readonly float detectionRadius;
    readonly float detectionAngle;
    readonly float innerdetectionRadius;
    readonly LayerMask layerMask;

    public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerdetectionRadius, LayerMask layerMask)
    {
        this.detectionAngle = detectionAngle;
        this.detectionRadius = detectionRadius;
        this.innerdetectionRadius = innerdetectionRadius;
        this.layerMask = layerMask;
    }

    public bool Execute(Transform player, Transform detector, CountDownTimer timer)
    {
        if (timer.IsRunning) return false;
        var directionToPlayer = player.position - detector.position;

        var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);

        if ((!(angleToPlayer < detectionAngle / 2f) || !(directionToPlayer.magnitude < detectionRadius)) 
            && !(directionToPlayer.magnitude < innerdetectionRadius)) return false;

        RaycastHit hit;
        if (Physics.SphereCast(detector.position,0.05f, directionToPlayer.normalized, out hit, detectionRadius, layerMask))
        {
            if (!hit.transform.CompareTag("Player"))
            {
                return false;
            }
        }
       
        

        timer.Start();
        return true;
    }
}
