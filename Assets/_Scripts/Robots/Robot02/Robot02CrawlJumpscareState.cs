using UnityEngine;
using UnityEngine.AI;

public class Robot02CrawlJumpscare : Robot02BaseState
{

    NavMeshAgent agent;
    public Robot02CrawlJumpscare(Enemy2 enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
    }
    
    public override void OnEnter()
    {
        agent.isStopped = true;

        //Vector3 lookDir = player.transform.position - enemy.transform.position;
        //lookDir.y = 0;
        //enemy.transform.rotation = Quaternion.LookRotation(lookDir);

        animator.CrossFade(CrawlJumpscareAnimation, 0.15f);

        KillPlayer();
    }

    private void KillPlayer()
    {
        //stops every action from the player
        Debug.Log("Meghaltál");
    }
}
