using UnityEngine;
using UnityEngine.AI;

public class Robot02CrawlState : Robot02BaseState
{
    private readonly NavMeshAgent agent;
    private readonly Transform player;
    private LoopTimer timer;
    public Robot02CrawlState(Enemy2 enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    }

    public override void OnEnter()
    {
        Debug.Log("crawling");
        timer = new LoopTimer(1, 99999);
        timer.OnLoop += SetDestination;
        agent.speed = 0.6f;
        agent.angularSpeed = 200;
        agent.acceleration = 9;
        animator.CrossFade(CrawlAnimation, crossFadeDuration);
        timer.Start();
    }

    private void SetDestination(int i)
    {
        agent.SetDestination(player.position);
    }

    public override void OnExit()
    {
        timer.Stop();
    }
}
