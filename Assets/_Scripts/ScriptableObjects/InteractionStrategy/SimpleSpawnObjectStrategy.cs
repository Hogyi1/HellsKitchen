using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleSpawnStrategy", menuName = "Game/Strategy/CounterStrategy/SpawnStrategy/SimpleSpawn")]
public class SimpleSpawnObjectStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        return (counter is ISpawner<KitchenObjectController> spawner && context.CanPickUpItem(spawner.GetSpawnerObject()));
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        if (context.CanPickUpItem(counter.GetChild()))
            return InteractionResult.Fail("Player has no space");

        var spawner = counter as ISpawner<KitchenObjectController>;

        spawner.SpawnObject(context, context.GetTransform());
        return InteractionResult.Ok("Object successfully spawned");
    }
}