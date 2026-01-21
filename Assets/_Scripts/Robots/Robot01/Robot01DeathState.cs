using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RobotDeathState : Robot01BaseState
{
    private readonly NavMeshAgent agent;
    public RobotDeathState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
    }

    public override void OnEnter()
    {
        Debug.Log("Dead");
        agent.isStopped = true;
        animator.CrossFade(DeathAnimation, 0.15f);
        enemy.StartCoroutine(DestroyAfterSeconds(10f));
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        enemy.OnDeath();
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(enemy.gameObject);
    }

}
