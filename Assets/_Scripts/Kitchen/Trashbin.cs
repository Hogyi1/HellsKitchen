public class Trashbin : BaseCounter
{
    private void Awake()
    {
        counterTop = counterTop != null ? counterTop : transform.Find("CounterTop");

        var hasItemPredicate = new ContextualPredicate<PlayerController>((context) => context.TryGetKitchenObject() != null);
        predicateList.Add(hasItemPredicate);
    }

    public override InteractionResult TryInteract(PlayerController context)
    {
        bool canInteract = CanInteract(context);
        if (!canInteract)
            return InteractionResult.Fail("You need an item to interact with");

        var item = context.TryGetKitchenObject();
        item.SetParent(null);
        item.DestroySelf();
        return InteractionResult.Ok("Item trashed");
    }
}