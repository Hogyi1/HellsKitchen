using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleDisposeStrategy", menuName = "Game/Strategy/CounterStrategy/DisposeStrategy")]
public class SimpleDisposeStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var playerKo = context.TryGetKitchenObject();
        return playerKo != null && playerKo is IDisposable;
    }
    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var playerKo = context.TryGetKitchenObject();
        if (playerKo == null || !(playerKo is IDisposable disposable))
            return InteractionResult.Fail("Player has no valid item to dispose");

        disposable.Dispose();
        return InteractionResult.Ok("Player disposed of item");
    }
}