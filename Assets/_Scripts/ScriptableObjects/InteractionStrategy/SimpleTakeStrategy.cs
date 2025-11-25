using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleTakeStrategy", menuName = "Game/Strategy/CounterStrategy/TakeStrategy")]
public class SimpleTakeStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var ownKo = counter.GetChild();
        var playerKo = context.TryGetKitchenObject();

        if (playerKo == null && ownKo != null && counter.CanRelease())
            return true;

        return false;
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var ownKo = counter.GetChild();
        var playerKo = context.TryGetKitchenObject();

        if (ownKo == null)
            return InteractionResult.Fail("Could not execute SimpleTakeStrategy");

        counter.OnRelease();
        ownKo.SetParent(context);
        return InteractionResult.Ok("Player picked up item from counter");
    }
}
