public abstract class StoveBaseState : IState
{
    protected StoveCounter stoveCounter;
    public StoveBaseState(StoveCounter stoveCounter) => this.stoveCounter = stoveCounter;

    public virtual void FixedUpdate() { }
    public virtual void Update() { }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract InteractionResult TryInteract(PlayerController context);
}