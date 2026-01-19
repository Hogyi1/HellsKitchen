using UnityEngine;

public abstract class Robot02BaseState:IState
{
    protected readonly Enemy2 enemy;
    protected readonly Animator animator;

    protected const float crossFadeDuration = 0.1f;

    public static int WalkAnimation = Animator.StringToHash("Walking");
    public static int JumpScareAnimation = Animator.StringToHash("Jumpscare");
    public static int StopAnimation = Animator.StringToHash("stop");
    public static int DeathAnimation = Animator.StringToHash("Death");

    public static int CrawlAnimation = Animator.StringToHash("crawl");
    public static int CrawlToStandAnimation = Animator.StringToHash("CawltoStand");
    public static int FlashedAnimation = Animator.StringToHash("flashed");
    public static int CrawlStunnedAnimation = Animator.StringToHash("crawlstunned");
    public static int CrawlJumpscareAnimation = Animator.StringToHash("crawljumpscare");
    public static int CrawlRetreatAnimation = Animator.StringToHash("FastCrawling");
    public static int RetreatAnimation = Animator.StringToHash("FastWalking");

    public Robot02BaseState(Enemy2 enemy, Animator animator)
    {
        this.enemy = enemy;
        this.animator = animator;
    }

    public virtual void FixedUpdate()
    {
     
    }

    public virtual void OnEnter()
    {
        
    }

    public virtual void OnExit()
    {
        
    }

    public virtual void Update()
    {
        
    }
}
