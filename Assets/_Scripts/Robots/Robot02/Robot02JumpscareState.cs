using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Robot02JumpscareState : Robot02BaseState
{
    private NavMeshAgent agent;
    private CinemachineCamera cm;
    public Robot02JumpscareState(Enemy2 enemy, Animator animator, NavMeshAgent agent,CinemachineCamera cm) : base(enemy, animator)
    {
        this.agent = agent;
        this.cm = cm;   
    }
    
    public override void OnEnter()
    {
        agent.isStopped = true;
        KillPlayer();
        animator.CrossFade(JumpScareAnimation, 0.15f); 
    }

    private void KillPlayer()
    {
        Debug.Log("Meghaltál");

        PlayerController.Instance.DisableMovement();
        CameraController.Instance.RequestFocus(cm);
        enemy.OnKill();

    }
}
