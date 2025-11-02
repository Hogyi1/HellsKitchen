public class GroundedState : PlayerBaseState
{
    public GroundedState(PlayerController controller) : base(controller) { }
}

public class JumpingState : PlayerBaseState
{
    public JumpingState(PlayerController controller) : base(controller) { }
}

public class FallingState : PlayerBaseState
{
    CountDownTimer timer;
    public FallingState(PlayerController controller, ref CountDownTimer coyoteTimer) : base(controller)
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
    public RisingState(PlayerController controller) : base(controller) { }
    public override void OnExit() => controller.verticalVelocity = 0;
}

public class SlidingState : PlayerBaseState
{
    public SlidingState(PlayerController controller) : base(controller) { }
}

