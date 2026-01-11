using UnityEngine;

[CreateAssetMenu(fileName = "NewSimplePlaceStrategy", menuName = "Game/Strategy/CounterStrategy/PlaceStrategy/SimplePlace")]
public class SimplePlaceStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var holder = counter as IObjectHolder<KitchenObjectController>;
        var playerKo = context.TryGetKitchenObject();

        if (holder == null)
            return false;

        if (playerKo != null && !counter.HasChild() && holder.CanPlace(playerKo))
            return true;

        return false;
    }
    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var ownKo = counter.GetModel().GetChild();
        var playerKo = context.TryGetKitchenObject();
        var holder = counter as IObjectHolder<KitchenObjectController>;

        if (playerKo == null)
            return InteractionResult.Fail("Player has no valid item");

        playerKo.SetParent(counter);
        holder.OnPlace(playerKo);
        return InteractionResult.Ok("Player placed item on counter");
    }
}
