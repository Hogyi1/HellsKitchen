
// Use strategy for every interaction like TakeObject, PlaceObject, etc.
public abstract class BaseCounterStrategy : InteractionStrategySO
{
    public sealed override InteractionResult Execute(PlayerController context, IInteractable interactable)
    {
        if (interactable is CounterController counter)
            return ExecuteOnCounter(context, counter);

        return InteractionResult.Fail("Not a Counter");
    }
    public sealed override bool CanExecute(PlayerController context, IInteractable interactable)
    {
        if (interactable is CounterController counter)
            return CanExecuteOnCounter(context, counter);

        return false;
    }

    public abstract InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter);
    public abstract bool CanExecuteOnCounter(PlayerController context, CounterController counter);
}
