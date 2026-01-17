using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleDisposeStrategy", menuName = "Game/Strategy/CounterStrategy/DisposeStrategy")]
public class SimpleDisposeStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var playerKo = context.GetChild() as KitchenObjectController;
        return playerKo is IDisposable && counter is IDisposer<KitchenObjectController>;
    }
    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var playerKo = context.GetChild() as KitchenObjectController;
        if (playerKo == null)
            return InteractionResult.Fail("Player has no valid item to dispose");

        (counter as IDisposer<KitchenObjectController>)?.OnDispose(playerKo);
        return InteractionResult.Ok("Player disposed of item");
    }
}