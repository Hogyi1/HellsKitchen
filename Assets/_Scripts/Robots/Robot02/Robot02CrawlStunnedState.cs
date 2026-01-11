using UnityEngine;
using UnityEngine.AI;

public class Robot02CrawlStunnedState : Robot02BaseState
{
    NavMeshAgent agent;
    private readonly CountDownTimer coolDownTimer;
    public Robot02CrawlStunnedState(Enemy2 enemy, Animator animator,NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
        coolDownTimer = new CountDownTimer(6);
    }

    public override void OnEnter()
    {
        agent.isStopped = true;
        coolDownTimer.Start();
        animator.CrossFade(CrawlStunnedAnimation, crossFadeDuration);
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
