using UnityEngine;
using UnityEngine.AI;

public class Robot01RunningState : Robot01BaseState
{
    readonly NavMeshAgent agent;
    readonly PlayerDetector detector;
    private Transform player;
    public Robot01RunningState(Enemy enemy, Animator animator,NavMeshAgent agent, PlayerDetector detector) : base(enemy, animator)
    {
        this.detector = detector;
        this.agent = agent;
    }

    public override void OnEnter()
    {
        Debug.Log("Chase");
        agent.speed = 5f;
        agent.angularSpeed = 550;
        agent.acceleration = 150;
        player = detector.Player;
        enemy.EnemySpotted();
        animator.CrossFade(RunningAnimation, crossFadeDuration);
    }

    public override void Update()
    {
        agent.SetDestination(player.position);
    }
}
