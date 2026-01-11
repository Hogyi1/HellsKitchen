using UnityEngine;
using UnityEngine.AI;

public class Robot02CrawlRetreatState : Robot02BaseState
{
    Transform retreatTarget;
    NavMeshAgent agent;
    public Robot02CrawlRetreatState(Enemy2 enemy, Animator animator, Transform retreatTarget, NavMeshAgent agent) : base(enemy, animator)
    {
        this.retreatTarget = retreatTarget;
        this.agent = agent;
    }

    public override void OnEnter()
    {
        Debug.Log("CrawlRetreating");
        agent.speed = 1f;
        agent.angularSpeed = 160;
        agent.acceleration = 9;
        agent.SetDestination(retreatTarget.position);
        animator.CrossFade(CrawlAnimation, crossFadeDuration);
    }
}