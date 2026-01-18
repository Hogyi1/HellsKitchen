using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Robot01JumpscareState : Robot01BaseState
{
    private readonly PlayerDetector player;
    private readonly NavMeshAgent agent;
    private readonly CinemachineCamera jumpscareCamera;

    public Robot01JumpscareState(Enemy enemy, Animator animator,NavMeshAgent agent,PlayerDetector player,CinemachineCamera jumpscareCamera) : base(enemy, animator)
    {
        this.player = player;  
        this.agent = agent;
        this.jumpscareCamera = jumpscareCamera;
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
        CameraController.Instance.RequestFocus(jumpscareCamera);
        enemy.OnKill();
    }
}
