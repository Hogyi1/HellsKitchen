using UnityEngine;

[CreateAssetMenu(fileName = "NewSimplePlaceStrategy", menuName = "Game/Strategy/CounterStrategy/PlaceStrategy/SimplePlace")]
public class SimplePlaceStrategy : BaseCounterStrategy
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

        playerKo.SetParent(counter);
        counter.OnPlace(counter.GetChild());
        return InteractionResult.Ok("Player placed item on counter");
    }
}
