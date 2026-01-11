using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PlayerDetector))]
public class Enemy : MonoBehaviour, IShotable
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] public Transform root;
    [SerializeField] public Transform waypointParent;
    [SerializeField] public Transform lightsOutWaypointParent;
    [SerializeField] PlayerDetector playerDetector;
    private Transform[] waypoints;
    private Transform[] lightsOutWaypoints;

    private bool isSpawned = true;
    private bool isHit = false;
    public bool isIdle = false;
    private bool isJumpscare = false;

    public bool isRetreating = false; 

    private bool lightsOut = false;

    StateMachine stateMachine;

    CinemachineCamera jumpscareCam;

    public event Action Despawned = delegate { };

    void Awake()
    {
        jumpscareCam = GetComponentInChildren<CinemachineCamera>();
    }
    void Start()
    {
        int n = waypointParent.childCount;
        int o = lightsOutWaypointParent.childCount;
        waypoints = new Transform[n];
        lightsOutWaypoints = new Transform[o];
        for (int i = 0; i < n; i++) waypoints[i] = waypointParent.GetChild(i);
        for (int i = 0; i < o; i++) lightsOutWaypoints[i] = lightsOutWaypointParent.GetChild(i);

        stateMachine = new StateMachine();
        var wanderstate = new Robot01WalkingState(this,animator,agent,waypoints,0.01f);
        var lightsoutwanderstate = new Robot01WalkingState(this, animator, agent, lightsOutWaypoints, 0.01f);
        var chasestate = new Robot01RunningState(this, animator, agent, playerDetector);
        var deathstate = new RobotDeathState(this, animator, agent);
        var idlestate = new Robot01IdleState(this,animator,agent);
        var jumpscarestate = new Robot01JumpscareState(this, animator, agent, playerDetector,jumpscareCam);
        var stopstate = new Robot01StopState(this,animator,agent);
        var retreatstate = new Robot01RetreatState(this, animator, waypoints[waypoints.Length-1],agent);

        //start
        At(stopstate, wanderstate, new FunctionPredicate(() => isSpawned));

        //chase
        At(wanderstate,chasestate, new FunctionPredicate(() => playerDetector.CanDetectPlayer()));
        At(chasestate, wanderstate, new FunctionPredicate(() => !playerDetector.CanDetectPlayer()));
        At(lightsoutwanderstate, chasestate, new FunctionPredicate(() => playerDetector.CanDetectPlayer() && lightsOut));
        At(chasestate, lightsoutwanderstate, new FunctionPredicate(() => !playerDetector.CanDetectPlayer() && lightsOut));

        //idle
        At(wanderstate, idlestate, new FunctionPredicate(() => isIdle));
        At(lightsoutwanderstate, idlestate, new FunctionPredicate(() => isIdle && lightsOut));
        At(idlestate, wanderstate, new FunctionPredicate(() => !isIdle));
        At(idlestate, lightsoutwanderstate, new FunctionPredicate(() => !isIdle && lightsOut));
        At(idlestate, chasestate, new FunctionPredicate(() => playerDetector.CanDetectPlayer()));

        //lightsOut
        At(wanderstate, lightsoutwanderstate, new FunctionPredicate(() => lightsOut));
        At(lightsoutwanderstate, wanderstate, new FunctionPredicate(() => !lightsOut));

        //death
        At(wanderstate, deathstate, new FunctionPredicate(() => isHit));
        At(lightsoutwanderstate, deathstate, new FunctionPredicate(() => isHit));
        At(chasestate, deathstate, new FunctionPredicate(() => isHit));
        At(idlestate, deathstate, new FunctionPredicate(() => isHit));

        //jumpscare
        At(wanderstate, jumpscarestate, new FunctionPredicate(() => isJumpscare));
        At(lightsoutwanderstate, jumpscarestate, new FunctionPredicate(() => isJumpscare && lightsOut));
        At(chasestate, jumpscarestate, new FunctionPredicate(() => isJumpscare));

        //retreat
        At(wanderstate, retreatstate, new FunctionPredicate(() => isRetreating));
        At(idlestate, retreatstate, new FunctionPredicate(() => isRetreating));

        At(retreatstate, chasestate, new FunctionPredicate(() => playerDetector.CanDetectPlayer()));
        At(chasestate,retreatstate, new FunctionPredicate(() => !playerDetector.CanDetectPlayer() && isRetreating));

        At(retreatstate,stopstate, new FunctionPredicate(() => !isSpawned));

        stateMachine.SetState(stopstate);
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void MainCond(IState to, IPredicate condition) => stateMachine.AddMainTransition(to, condition);

    void Update()
    {
        stateMachine.Update();
    }
    void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    public void OnHit()
    {
      isHit = true; 
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isJumpscare = true;
        }
        
    }

    public void MoveUp(Transform spawn)
    {
        isSpawned = true;
        transform.position = spawn.position;
    }

    public void MoveDown()
    {
        isSpawned = false;
        transform.position = waypoints[waypoints.Length-1].position - new Vector3(0,100,0);
        Despawned?.Invoke();
    }
}
