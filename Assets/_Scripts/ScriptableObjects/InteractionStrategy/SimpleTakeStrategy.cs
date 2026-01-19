using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleTakeStrategy", menuName = "Game/Strategy/CounterStrategy/TakeStrategy")]
public class SimpleTakeStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        bool canRelease = true;
        if (counter is IObjectHolder<KitchenObjectController> holder)
            canRelease = holder.CanRelease();

        return (context.CanPickUpItem(counter.GetChild()) && counter.HasChild() && canRelease);
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var ownKo = counter.GetModel().GetChild();

        if (counter is IObjectHolder<KitchenObjectController> holder)
            holder.OnRelease();

        ownKo.SetParent(context);
        return InteractionResult.Ok("Player picked up item from counter");
    }
}
