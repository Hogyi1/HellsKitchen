using UnityEngine;
using UnityEngine.AI;

public class Robot02StunnedState : Robot02BaseState
{
    NavMeshAgent agent;
    private readonly CountDownTimer coolDownTimer;
    public Robot02StunnedState(Enemy2 enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
        coolDownTimer = new CountDownTimer(8);
    }

    public override void OnEnter()
    {
        agent.isStopped= true;
        coolDownTimer.Start();
        animator.CrossFade(FlashedAnimation, crossFadeDuration);
    }

    public override void Update()
    {
        coolDownTimer.Tick();
        if (coolDownTimer.IsFinished)
        {
            enemy.isStunned = false;
        }
    }

    public override void OnExit()
    {
        agent.isStopped = false;
    }
}
