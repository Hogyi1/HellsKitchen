using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.AI;

public class Robot02CrawlJumpscare : Robot02BaseState
{

    private NavMeshAgent agent;
    private CinemachineCamera cam;
    public Robot02CrawlJumpscare(Enemy2 enemy, Animator animator, NavMeshAgent agent, CinemachineCamera jumpscareCamera) : base(enemy, animator)
    {
        this.agent = agent;
        this.cam = jumpscareCamera;
    }
     
    public override void OnEnter()
    {
        agent.isStopped = true;
        cam.transform.position -= new Vector3(0,1.55f,-0.95f);

        animator.CrossFade(CrawlJumpscareAnimation, 0.15f);
        KillPlayer();
    }

    private void KillPlayer()
    {
        Debug.Log("Meghaltál");
        PlayerController.Instance.DisableMovement();
        
        CameraController.Instance.RequestFocus(cam);
        enemy.OnKill();
    }
}
