using UnityEngine;

public abstract class PlayerBaseState : IState
{
    public static int WalkAnimation = Animator.StringToHash("Walk");
    public static int RunAnimation = Animator.StringToHash("Run");
    public static int JumpAnimation = Animator.StringToHash("Jump");
    public static int FallingAnimation = Animator.StringToHash("Falling");
    public static int CrouchingAnimation = Animator.StringToHash("Crouch");

    protected PlayerMovementController controller;

    public PlayerBaseState(PlayerMovementController controller)
    {
        this.controller = controller;
    }

    public virtual void OnEnter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void OnExit() { }
}

public interface IState
{
    public void OnEnter();
    public void Update();
    public void FixedUpdate();
    public void OnExit();
}
