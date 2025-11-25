using UnityEngine;

[CreateAssetMenu(fileName = "NewCutStrategy", menuName = "Game/Strategy/CounterStrategy/CutStrategy")]
public class CutInteractionStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var ownKo = counter.GetChild();
        var playerKo = context.TryGetKitchenObject();
        return (ownKo != null && playerKo == null && counter is ICuttingTable cuttable && !cuttable.IsDone());
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var cuttable = counter as ICuttingTable;
        cuttable.Cut();
        return InteractionResult.Ok("Cut item on cutting board " + cuttable.Progress());
    }
}
