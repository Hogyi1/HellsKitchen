public class Trashbin : BaseCounter
{
    private void Awake()
    {
        counterTop = counterTop != null ? counterTop : transform.Find("CounterTop");

        var hasItemPredicate = new ContextualPredicate<PlayerController>((context) => context.TryGetKitchenObject() != null);
        predicateList.Add(hasItemPredicate);
    }
}
