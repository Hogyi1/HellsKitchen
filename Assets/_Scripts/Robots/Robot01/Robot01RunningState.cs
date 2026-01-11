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
        agent.speed = 5.5f;
        agent.angularSpeed = 400;
        agent.acceleration = 9;
        player = detector.Player;
        animator.CrossFade(RunningAnimation, crossFadeDuration);
    }

    public override void Update()
    {
        agent.SetDestination(player.position);
    }
}
