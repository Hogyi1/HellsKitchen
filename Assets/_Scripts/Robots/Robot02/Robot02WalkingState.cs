using UnityEngine;
using UnityEngine.AI;

public class Robot02WalkingState : Robot02BaseState
{
    private readonly NavMeshAgent agent;
    private readonly Transform player;
    private LoopTimer timer;
    public Robot02WalkingState(Enemy2 enemy, Animator animator, NavMeshAgent agent,Transform player) : base(enemy, animator)
    {
        this.agent = agent;
        this.player = player;
    } 
    public override void OnEnter()
    {
        Debug.Log("walking");
        timer = new LoopTimer(1, 99999);
        timer.OnLoop += SetDestination;
        agent.speed = 1f;
        agent.angularSpeed = 160;
        agent.acceleration = 9;
        animator.CrossFade(WalkAnimation, crossFadeDuration);
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
