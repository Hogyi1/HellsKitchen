using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleSwapStrategy", menuName = "Game/Strategy/CounterStrategy/SwapStrategy")]
public class SimpleSwapStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var ownKo = counter.GetChild();
        var playerKo = context.TryGetKitchenObject();

        if (ownKo == null || playerKo == null)
            return false;

        foreach (var item in ownKo.Interactions)
        {
            if (item.CanExecute(playerKo))
                return true;
        }

        foreach (var item in playerKo.Interactions)
        {
            if (item.CanExecute(ownKo))
                return true;
        }

        return counter.CanRelease() && counter.CanPlace(playerKo);
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var ownKo = counter.GetChild();
        var playerKo = context.TryGetKitchenObject();

        if (ownKo == null || playerKo == null)
            return InteractionResult.Fail("No items to swap");

        foreach (var item in ownKo.Interactions)
        {
            if (item.CanExecute(playerKo))
            {
                item.Execute(playerKo);
                return InteractionResult.Ok("Items interacted");
            }
        }

        foreach (var item in playerKo.Interactions)
        {
            if (item.CanExecute(ownKo))
            {
                item.Execute(ownKo);
                return InteractionResult.Ok("Items interacted");
            }
        }

        ownKo.SwapParent(playerKo);
        counter.OnPlace(counter.GetChild());
        return InteractionResult.Ok("Switched items with counter");
    }
}