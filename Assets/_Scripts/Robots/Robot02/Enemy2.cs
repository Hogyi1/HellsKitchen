using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy2:MonoBehaviour, IShotable, ICrawlable,IStunnable
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform root;
    [SerializeField] private Transform retreatTarget;
    [SerializeField] private AudioSO retreatConfig;
    [SerializeField] private AudioSource retreatAudio;
    [SerializeField] private CinemachineCamera jumpscareCam;
    [SerializeField] private Transform wardrobeJumpscarePos;
    [SerializeField] private AudioSO jumpscareSound;

    private bool isSpawned = false;

    public bool isStunned = false;
    private bool isHit = false;
    private bool isJumpscare = false;
    private bool isCrawling = false;
    private bool isRetreating = false;
    public bool isPlayerHiding = false;
    public bool isAtKillingPos = false;
    private int flashes = 0;
    private Transform player;
    private CountDownTimer flashedCooldown;

    public event Action Despawned = delegate { };
    public event Action OnKillPlayer = delegate { };
    public event Action HidingSpotArrival = delegate { };

    StateMachine stateMachine;
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }
    void Start()
    {
        flashedCooldown = new CountDownTimer(10);
        stateMachine = new StateMachine();

        var wanderstate = new Robot02WalkingState(this,animator,agent,player);
        var deathstate = new Robot02DeathState(this, animator, agent);
        var jumpscarestate = new Robot02JumpscareState(this, animator,agent,jumpscareCam, jumpscareSound, retreatAudio);
        var stopstate = new Robot02StopState(this, animator, agent);
        var crawlstate = new Robot02CrawlState(this, animator,agent,player);
        var crawlstunnedstate = new Robot02CrawlStunnedState(this, animator,agent);
        var crawljumpscarestate = new Robot02CrawlJumpscare(this,animator,agent,jumpscareCam,jumpscareSound, retreatAudio);
        var stunnedstate = new Robot02StunnedState(this,animator,agent);
        var retreatstate = new Robot02RetreatState(this, animator,retreatTarget,agent);
        var crawlretreatstate = new Robot02CrawlRetreatState(this, animator, retreatTarget, agent);
        var playerhidingstate = new Robot02PlayerHidingState(this,animator,agent,wardrobeJumpscarePos);

        //Start
        At(stopstate, wanderstate, new FunctionPredicate(() => isSpawned));
        //Crawling
        At(wanderstate, crawlstate, new FunctionPredicate(() => isCrawling));
        At(crawlstate, wanderstate, new FunctionPredicate(() => !isCrawling));
        //PlayerHiding
        At(wanderstate, playerhidingstate, new FunctionPredicate(() => isPlayerHiding));
        At(playerhidingstate, wanderstate, new FunctionPredicate(() => !isPlayerHiding));
        //Stunned
        At(wanderstate, stunnedstate, new FunctionPredicate(() => isStunned));
        At(stunnedstate, wanderstate, new FunctionPredicate(() => !isStunned));
        //CrawlStunned
        At(crawlstate,crawlstunnedstate , new FunctionPredicate(() => isStunned));
        At(crawlstunnedstate, crawlstate, new FunctionPredicate(() => !isStunned));
        //Death/jumpscare
        At(wanderstate, jumpscarestate, new FunctionPredicate(() => isJumpscare));
        At(crawlstate, crawljumpscarestate, new FunctionPredicate(() => isJumpscare));
        At(playerhidingstate, jumpscarestate, new FunctionPredicate(() => isAtKillingPos));
        At(wanderstate, deathstate, new FunctionPredicate(() => isHit));
        //retreat
        At(wanderstate,retreatstate, new FunctionPredicate( () => isRetreating));
        At(crawlstate, crawlretreatstate, new FunctionPredicate(() => isRetreating));
        At(retreatstate, crawlretreatstate, new FunctionPredicate(() => isCrawling));
        At(crawlretreatstate, retreatstate, new FunctionPredicate(() => !isCrawling));
        //despawn
        At(retreatstate,stopstate, new FunctionPredicate(() => !isSpawned));
        At(crawlretreatstate, stopstate, new FunctionPredicate(() => isCrawling && !isSpawned));
        stateMachine.SetState(stopstate);
    }

    void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
    void MainCond(IState to, IPredicate condition) => stateMachine.AddMainTransition(to, condition);

    void Update()
    {
        stateMachine.Update();
        flashedCooldown.Tick();

        if(flashes > 4)
        {
            isRetreating = true;
        }
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
        if (other.CompareTag("Player"))
        {
            isJumpscare = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isJumpscare = false;
        }
    }

    public void OnVentEnter()
    {
        isCrawling = !isCrawling;
    }

    public void OnFlashed()
    {
        if (!flashedCooldown.IsRunning) 
        {
            flashedCooldown.Start();
            flashes++;
            isStunned = true;
        }
    }
    public void OnKill()
    {
        OnKillPlayer?.Invoke();
    }

    public void OnArrival()
    {
        HidingSpotArrival?.Invoke();
    }

    public void MoveUp(Transform spawn)
    {
        transform.position = spawn.position;
        isSpawned = true;
        flashes = 0;
    }
    

    public void MoveDown()
    {
        Debug.Log("Dispawned");
        agent.isStopped = true;
        agent.enabled = false;
        AudioManager.Instance.PlaySFX(retreatConfig, retreatAudio);
        transform.position = retreatTarget.position - new Vector3(0, 100, 0);
        Despawned.Invoke();
        isSpawned = false;
    }
}
