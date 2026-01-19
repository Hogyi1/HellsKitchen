public class EmptyAndEmptyPredicate : ContextualPredicate<PlayerController>
{
    public EmptyAndEmptyPredicate(CounterController counter) : base(
        (context) =>
        {
            if (context.TryGetKitchenObject() == null && !counter.GetModel().HasChild())
            {
                return false;
            }

            return true;
        })
    { }
}