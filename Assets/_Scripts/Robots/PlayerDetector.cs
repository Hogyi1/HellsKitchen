using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private float detectionAngle = 200f;
    [SerializeField] private float detectionRadius = 1f;
    [SerializeField] private float innerDetectionRadius = 0.5f;
    [SerializeField] private float aggroTime = 6f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Wardrobe wardrobe;

    public Transform Player {  get; private set; }
    private CountDownTimer aggroTimer;
    private bool isHiding = false;

    IdetectionStrategy detectionStrategy;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Start()
    {
        wardrobe.OnHide += WardrobeOnHide;
        aggroTimer = new CountDownTimer(aggroTime);
        detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius,innerDetectionRadius,layerMask);
    }

    private void WardrobeOnHide(bool hidingParam)
    {
        isHiding = hidingParam;
    }
    public bool CanDetectPlayer()
    {
        if(isHiding)
            return false;
        if (aggroTimer.IsRunning)
            return true;

        if (detectionStrategy.Execute(Player, transform))
        {
            aggroTimer.Start();
            return true;
        }

        return false;
    }
    /*    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,detectionRadius);
        Gizmos.DrawWireSphere(transform.position,innerDetectionRadius);

        Vector3 forwarConeDirection = Quaternion.Euler(0, detectionAngle / 2, 0) * transform.forward * detectionRadius;
        Vector3 backwardConeDirection = Quaternion.Euler(0, -detectionAngle / 2, 0) * transform.forward * detectionRadius;

        Gizmos.DrawLine(transform.position, transform.position + forwarConeDirection);
        Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
    }
    */
}
