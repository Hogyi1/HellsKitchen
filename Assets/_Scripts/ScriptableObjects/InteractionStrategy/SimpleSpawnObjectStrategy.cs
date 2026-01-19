using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleSpawnStrategy", menuName = "Game/Strategy/CounterStrategy/SpawnStrategy/SimpleSpawn")]
public class SimpleSpawnObjectStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var playerKo = context.TryGetKitchenObject();
        return (playerKo == null && counter is ISpawner<KitchenObjectController>);
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var playerKo = context.TryGetKitchenObject();
        if (playerKo != null)
            return InteractionResult.Fail("Player has no space");

        var spawner = counter as ISpawner<KitchenObjectController>;

        spawner.SpawnObject(context, context.GetTransform());
        return InteractionResult.Ok("Object successfully spawned");
    }
}