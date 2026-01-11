using UnityEngine;

public abstract class Robot01BaseState:IState
{
    protected readonly Enemy enemy;
    protected readonly Animator animator;

    protected const float crossFadeDuration = 0.1f;

    public static int WalkAnimation = Animator.StringToHash("Walking");
    public static int JumpScareAnimation = Animator.StringToHash("Jumpscare");
    public static int StopAnimation = Animator.StringToHash("stop");
    public static int DeathAnimation = Animator.StringToHash("Death");

    public static int RunningAnimation = Animator.StringToHash("Running");
    public static int IdleAnimation = Animator.StringToHash("idle");

    public Robot01BaseState(Enemy enemy,Animator animator)
    {
        this.enemy = enemy;
        this.animator = animator;
    }

    public virtual void OnEnter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnExit() { }
}
