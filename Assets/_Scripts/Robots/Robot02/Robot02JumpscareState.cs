using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Robot02JumpscareState : Robot02BaseState
{
    private NavMeshAgent agent;
    private CinemachineCamera cm;
    private AudioSO audio;
    private AudioSource source;
    public Robot02JumpscareState(Enemy2 enemy, Animator animator, NavMeshAgent agent,CinemachineCamera cm,AudioSO audio, AudioSource source) : base(enemy, animator)
    {
        this.agent = agent;
        this.cm = cm;
        this.audio = audio;
        this.source = source;
    }
    
    public override void OnEnter()
    {
        agent.isStopped = true;
        AudioManager.Instance.PlaySFX(audio, source);
        animator.CrossFade(JumpScareAnimation, 0.15f);
        KillPlayer();
       
    }

    private void KillPlayer()
    {
        Debug.Log("Meghaltál");

        PlayerController.Instance.DisableMovement();
        CameraController.Instance.RequestFocus(cm);
        enemy.OnKill();

    }
}
