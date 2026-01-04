using UnityEngine;

[CreateAssetMenu(fileName = "NewProgressiveStrategy", menuName = "Game/Strategy/CounterStrategy/Progressive")]
public class ProgressiveInteractionStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var ownKo = counter.GetModel().GetChild();
        var playerKo = context.TryGetKitchenObject();
        return (ownKo != null && playerKo == null && counter is IProgressiveInteraction);
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var progressiveCounter = counter as IProgressiveInteraction;
        progressiveCounter.OnAction();
        return InteractionResult.Ok($"Progress: {progressiveCounter.Progress() * 100f}%");
    }
}
