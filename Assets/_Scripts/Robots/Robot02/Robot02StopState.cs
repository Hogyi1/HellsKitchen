using UnityEngine;
using UnityEngine.AI;

public class Robot02StopState : Robot02BaseState
{
    private readonly NavMeshAgent agent;
    public Robot02StopState(Enemy2 enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
    {
        this.agent = agent;
    }
}
