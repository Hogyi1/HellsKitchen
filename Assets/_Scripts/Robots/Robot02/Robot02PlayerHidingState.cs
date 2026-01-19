using UnityEngine;
using UnityEngine.AI;

public class Robot02PlayerHidingState : Robot02BaseState
{
    private readonly NavMeshAgent agent;
    private readonly Transform exitPos;
    public Robot02PlayerHidingState(Enemy2 enemy, Animator animator, NavMeshAgent agent, Transform exitPos) : base(enemy, animator)
    {
        this.agent = agent;
        this.exitPos = exitPos;
    }
    public override void OnEnter()
    {
        agent.speed = 2f;
        agent.SetDestination(exitPos.position);
        agent.angularSpeed = 160;
        agent.acceleration = 9;
        animator.CrossFade(WalkAnimation, crossFadeDuration);
    }
    public override void Update()
    {
        if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.isStopped = true;
            enemy.transform.rotation = exitPos.rotation;
            enemy.OnArrival();
        }
    }
}
