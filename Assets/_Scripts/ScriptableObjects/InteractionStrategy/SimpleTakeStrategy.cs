using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleTakeStrategy", menuName = "Game/Strategy/CounterStrategy/TakeStrategy")]
public class SimpleTakeStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var ownKo = counter.GetModel().GetChild();
        var playerKo = context.TryGetKitchenObject();

        if (playerKo == null && ownKo != null)
            return true;

        return false;
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var ownKo = counter.GetModel().GetChild();
        var playerKo = context.TryGetKitchenObject();

        if (ownKo == null)
            return InteractionResult.Fail("Could not execute SimpleTakeStrategy");

        if (counter is IObjectHolder<KitchenObject> holder)
            holder.OnRelease();
        ownKo.SetParent(context);
        return InteractionResult.Ok("Player picked up item from counter");
    }
}
