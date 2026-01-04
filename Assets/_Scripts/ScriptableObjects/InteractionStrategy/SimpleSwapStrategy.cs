using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleSwapStrategy", menuName = "Game/Strategy/CounterStrategy/SwapStrategy")]
public class SimpleSwapStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var ownKo = counter.GetModel().GetChild();
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

        if (counter is IObjectHolder<KitchenObject> holder)
            return holder.CanRelease() && holder.CanPlace(playerKo);

        return false;
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var ownKo = counter.GetModel().GetChild();
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

        if (counter is IObjectHolder<KitchenObject> holder)
            holder.OnPlace(counter.GetModel().GetChild());
        return InteractionResult.Ok("Switched items with counter");
    }
}