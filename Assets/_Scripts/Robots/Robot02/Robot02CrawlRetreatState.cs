using UnityEngine;
using UnityEngine.AI;

public class Robot02CrawlRetreatState : Robot02BaseState
{
    private Transform retreatTarget;
    private NavMeshAgent agent;
    public Robot02CrawlRetreatState(Enemy2 enemy, Animator animator, Transform retreatTarget, NavMeshAgent agent) : base(enemy, animator)
    {
        this.retreatTarget = retreatTarget;
        this.agent = agent;
    }
     
    public override void OnEnter()
    {
        Debug.Log("Retreating");
        agent.speed = 6f;
        agent.angularSpeed = 250;
        agent.acceleration = 9;
        agent.autoBraking = true;
        agent.autoRepath = true;
        agent.SetDestination(retreatTarget.position);
        animator.CrossFade(CrawlAnimation, crossFadeDuration);
    }
    public override void Update()
    {

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {

            enemy.MoveDown();
        }
    }
}