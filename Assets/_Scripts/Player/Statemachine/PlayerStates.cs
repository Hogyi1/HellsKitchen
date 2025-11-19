public class GroundedState : PlayerBaseState
{
    public GroundedState(PlayerMovementController controller) : base(controller) { }
}

public class JumpingState : PlayerBaseState
{
    public JumpingState(PlayerMovementController controller) : base(controller) { }
}

public class FallingState : PlayerBaseState
{
    CountDownTimer timer;
    public FallingState(PlayerMovementController controller, ref CountDownTimer coyoteTimer) : base(controller)
    {
        this.timer = coyoteTimer;
    }
    public override void OnEnter()
    {
        timer.Start();
        controller.verticalVelocity = 0;
    }
    public override void OnExit() => timer.Stop();
}

public class RisingState : PlayerBaseState
{
    public RisingState(PlayerMovementController controller) : base(controller) { }
    public override void OnExit() => controller.verticalVelocity = 0;
}

public class SlidingState : PlayerBaseState
{
    public SlidingState(PlayerMovementController controller) : base(controller) { }
}

