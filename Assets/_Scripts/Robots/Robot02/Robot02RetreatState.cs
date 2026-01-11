using UnityEngine;
using UnityEngine.AI;

public class Robot02RetreatState : Robot02BaseState
{
    Transform retreatTarget;
    NavMeshAgent agent;
    public Robot02RetreatState(Enemy2 enemy, Animator animator,Transform retreatTarget, NavMeshAgent agent) : base(enemy, animator)
    {
        this.retreatTarget = retreatTarget;
        this.agent = agent;
    }

    public override void OnEnter()
    {
        Debug.Log("Retreating");
        agent.speed = 10.5f;
        agent.angularSpeed = 160;
        agent.acceleration = 9;
        agent.SetDestination(retreatTarget.position);
        animator.CrossFade(RetreatAnimation, crossFadeDuration);
    }

    public override void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            enemy.MoveDown();
        }
    }
}
