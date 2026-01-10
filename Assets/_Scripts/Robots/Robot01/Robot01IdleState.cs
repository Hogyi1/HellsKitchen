using UnityEngine;
using UnityEngine.AI;

public class Robot01IdleState : Robot01BaseState
{
    private readonly NavMeshAgent agent;
    private readonly CountDownTimer coolDownTimer;
    public Robot01IdleState(Enemy enemy, Animator animator,NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
        coolDownTimer = new CountDownTimer(8);
    }

    public override void OnEnter()
    {
        coolDownTimer.Start();
        animator.CrossFade(IdleAnimation, crossFadeDuration);
    }
    public override void Update()
    {
        coolDownTimer.Tick();
        if(coolDownTimer.IsFinished)
        {
            enemy.isIdle = false;
        }
    }

    public override void OnExit()
    {
        coolDownTimer.Reset();
    }
}
