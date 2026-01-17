using UnityEngine;

[CreateAssetMenu(fileName = "NewComplexSpawnStrategy", menuName = "Game/Strategy/CounterStrategy/SpawnStrategy/ComplexSpawn")]
public class ComplexSpawnObjectStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        return counter is ISpawner<KitchenObjectController>;
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var playerKo = context.GetChild() as KitchenObjectController;
        var spawner = counter as ISpawner<KitchenObjectController>;
        var spawnerKo = spawner.GetSpawnerObject();

        if (playerKo == null && context.CanPickUpItem(spawnerKo))
        {
            spawner.SpawnObject(context, context.GetTransform());
            return InteractionResult.Ok("Took an item from spawner.");
        }

        // Check if spawned object can act on player's object (e.g. plate taking ingredient)
        foreach (var interaction in spawnerKo.Interactions)
        {
            if (interaction.CanExecute(playerKo))
            {
                interaction.Execute(playerKo);
                spawner.SpawnObject(context, context.GetTransform());
                return InteractionResult.Ok("Spawned object interacted with player's item.");
            }
        }

        // Check if player's object can act on spawned object (e.g. knife cutting cabbage)
        foreach (var interaction in playerKo.Interactions)
        {
            if (interaction.CanExecute(spawnerKo))
            {
                interaction.Execute(spawnerKo);
                return InteractionResult.Ok("Player's item interacted with spawned object.");
            }
        }

        return InteractionResult.Fail("Player's item cannot interact with spawned object.");
    }
}