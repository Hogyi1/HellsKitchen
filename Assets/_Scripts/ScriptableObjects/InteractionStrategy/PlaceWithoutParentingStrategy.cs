using UnityEngine;

[CreateAssetMenu(fileName = "NewComplexPlaceStrategy", menuName = "Game/Strategy/CounterStrategy/PlaceStrategy/ComplexPlaceStrategy")]
public class PlaceWithoutParentingStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var ownKo = counter.GetModel().GetChild();
        var playerKo = context.TryGetKitchenObject();
        if (playerKo != null && ownKo == null)
            return true;
        if (counter is IObjectHolder<KitchenObjectController> holder)
            return holder.CanPlace(playerKo);

        return false;
    }
    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var ownKo = counter.GetModel().GetChild();
        var playerKo = context.TryGetKitchenObject();

        if (playerKo == null)
            return InteractionResult.Fail("Player has no valid item");

        playerKo.SetParent(null);

        if (counter is IObjectHolder<KitchenObjectController> holder)
            holder.OnPlace(counter.GetModel().GetChild());

        return InteractionResult.Ok("Player placed item on counter");
    }
}
