using UnityEngine;

[CreateAssetMenu(fileName = "NewComplexPlaceStrategy", menuName = "Game/Strategy/CounterStrategy/PlaceStrategy/ComplexPlaceStrategy")]
public class PlaceWithoutParentingStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var ownKo = counter.GetChild();
        var playerKo = context.TryGetKitchenObject();
        if (playerKo != null && ownKo == null && counter.CanPlace(playerKo))
            return true;

        return false;
    }
    public override InteractionResult ExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var ownKo = counter.GetChild();
        var playerKo = context.TryGetKitchenObject();

        if (playerKo == null)
            return InteractionResult.Fail("Player has no valid item");

        playerKo.SetParent(null);
        counter.OnPlace(counter.GetChild());
        return InteractionResult.Ok("Player placed item on counter");
    }
}
