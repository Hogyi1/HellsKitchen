public class EmptyAndEmptyPredicate : ContextualPredicate<PlayerController>
{
    public EmptyAndEmptyPredicate(CounterController counter) : base(
        (context) =>
        {
            if (context.GetChild() as KitchenObjectController == null && !counter.GetModel().HasChild())
            {
                return false;
            }

            return true;
        })
    { }
}