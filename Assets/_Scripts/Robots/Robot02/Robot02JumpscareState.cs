using UnityEngine;
using UnityEngine.AI;

public class Robot02JumpscareState : Robot02BaseState
{
    NavMeshAgent agent;
    public Robot02JumpscareState(Enemy2 enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
    }
    
    public override void OnEnter()
    {
        agent.isStopped = true;

        animator.CrossFade(JumpScareAnimation, 0.15f);

        KillPlayer();
    }

    private void KillPlayer()
    {
        //stops every action from the player
        Debug.Log("Meghaltál");
    }
}
