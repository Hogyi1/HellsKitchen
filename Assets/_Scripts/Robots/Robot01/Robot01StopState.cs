using UnityEngine;
using UnityEngine.AI;

public class Robot01StopState : Robot01BaseState
{
    private readonly NavMeshAgent agent;
    public Robot01StopState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        agent.enabled = false;
        animator.CrossFade(StopAnimation,crossFadeDuration);
    }
    public override void OnExit()
    {
        agent.enabled = true;
    }
}
