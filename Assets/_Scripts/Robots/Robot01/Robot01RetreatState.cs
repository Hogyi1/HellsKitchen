using UnityEngine;
using UnityEngine.AI;

public class Robot01RetreatState : Robot01BaseState
{
    Transform retreatTarget;
    NavMeshAgent agent;
    public Robot01RetreatState(Enemy enemy, Animator animator,Transform retreatTarget, NavMeshAgent agent) : base(enemy, animator)
    {
        this.retreatTarget = retreatTarget;
        this.agent = agent;
    }

    public override void OnEnter()
    {
        Debug.Log("Retreating");
        agent.speed = 8f;
        agent.angularSpeed = 250;
        agent.acceleration = 15;
        agent.autoBraking = true;
        agent.autoRepath = true;
        agent.SetDestination(retreatTarget.position);
        animator.CrossFade(WalkAnimation, crossFadeDuration);
    }

    public override void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            enemy.MoveDown();
        }
    }
}
