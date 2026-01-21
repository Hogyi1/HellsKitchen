using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Robot02DeathState : Robot02BaseState
{
    private readonly NavMeshAgent agent;
    public Robot02DeathState(Enemy2 enemy, Animator animator,NavMeshAgent agent) : base(enemy, animator)
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