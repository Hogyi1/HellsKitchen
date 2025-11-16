public class ClearCounter : BaseCounter
{
    private void Awake()
    {
        counterTop = counterTop != null ? counterTop : transform.Find("CounterTop");
    }

    public override InteractionResult TryInteract(PlayerController context)
    {
        var ko = context.TryGetKitchenObject();
        bool hasChild = HasChild();
        var ownKo = GetChild();

        if (!CanInteract(context))
            return InteractionResult.Fail("Cannot interact right now");

        if (hasChild && ko == null)
        {
            ownKo.SetParent(context);
            return InteractionResult.Ok("Player picked up item from counter");
        }
        else if (!hasChild && ko != null)
        {
            ko.SetParent(this);
            return InteractionResult.Ok("Player placed item on counter");
        }
        else if (hasChild && ko != null)
        {
            ownKo.SwapParent(ko);
            return InteractionResult.Ok("Switched items with counter");
        }

        return InteractionResult.Fail("You need a kitchen object to interact");
    }
}