using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Robot01JumpscareState : Robot01BaseState
{
    private readonly PlayerDetector player;
    //private readonly Action onPlayerKilled;
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

        animator.CrossFade(JumpScareAnimation, 0.15f);

        KillPlayer();
    }

    private void KillPlayer()
    {
        //stops every action from the player

        //PlayerController.KillPlayer();
        
        Debug.Log("Meghaltál");
    }
}
