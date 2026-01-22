
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Robot01JumpscareState : Robot01BaseState
{
    private readonly PlayerDetector player;
    private readonly NavMeshAgent agent;
    private readonly CinemachineCamera jumpscareCamera;
    private readonly AudioSO jumpscareSound;
    private readonly AudioSource jumpscareSource;
    public Robot01JumpscareState(Enemy enemy, Animator animator, NavMeshAgent agent, PlayerDetector player, CinemachineCamera jumpscareCamera, AudioSO jumpscareSound, AudioSource jumpscareSource) : base(enemy, animator)
    {
        this.player = player;
        this.agent = agent;
        this.jumpscareCamera = jumpscareCamera;
        this.jumpscareSound = jumpscareSound;
        this.jumpscareSource = jumpscareSource;
    }

    public override void OnEnter()
    {
        agent.isStopped = true;
        animator.CrossFade(JumpScareAnimation, 0.15f);
        AudioManager.Instance.PlaySFX(jumpscareSound, jumpscareSource);
        KillPlayer();

    }

    private void KillPlayer()
    {
        Debug.Log("Meghaltál");

        PlayerController.Instance.DisableMovement();
        CameraController.Instance.RequestFocus(jumpscareCamera);
        enemy.OnKill();
    }
}
