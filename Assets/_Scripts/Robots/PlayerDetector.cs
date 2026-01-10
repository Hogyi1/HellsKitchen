using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] float detectionAngle = 200f;
    [SerializeField] float detectionRadius = 1f;
    [SerializeField] float innerDetectionRadius = 0.5f;
    [SerializeField] float detectionCooldown = 1.5f;
    [SerializeField] float aggroTime = 3.5f;
    [SerializeField] LayerMask layerMask;
    

    public Transform Player {  get; private set; }
    CountDownTimer detectionTimer;
    CountDownTimer aggroTimer; 

    IdetectionStrategy detectionStrategy;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    void Start()
    {
        detectionTimer = new CountDownTimer(detectionCooldown);
        aggroTimer = new CountDownTimer(aggroTime);
        detectionStrategy = new ConeDetectionStrategy(detectionAngle, detectionRadius,innerDetectionRadius,layerMask);
    }

   
    void Update()
    {
        detectionTimer.Tick();
        aggroTimer.Tick();
    }

    public bool CanDetectPlayer()
    {
        if (aggroTimer.IsRunning)
            return true;

        if (detectionStrategy.Execute(Player, transform, detectionTimer))
        {
            aggroTimer.Start();
            return true;
        }

        return false;
    }
    
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
}
