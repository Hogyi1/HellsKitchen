using UnityEngine;

[CreateAssetMenu(fileName = "NewComplexSpawnStrategy", menuName = "Game/Strategy/CounterStrategy/SpawnStrategy/ComplexSpawn")]
public class ComplexSpawnObjectStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        if (counter is not ISpawner<KitchenObject>)
            return false;

        return true;
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        if (counter is not ISpawner<KitchenObject> spawner)
        {
            return InteractionResult.Fail("Counter is not a valid spawner.");
        }

        var playerKo = context.TryGetKitchenObject();

        if (playerKo == null)
        {
            spawner.SpawnObject(context);
            return InteractionResult.Ok("Took an item from spawner.");
        }

        var spawnedKo = spawner.SpawnObject(counter);

        // Check if spawned object can act on player's object (e.g. plate taking ingredient)
        foreach (var interaction in spawnedKo.Interactions)
        {
            if (interaction.CanExecute(playerKo))
            {
                interaction.Execute(playerKo);
                spawnedKo.SetParent(context);
                return InteractionResult.Ok("Spawned object interacted with player's item.");
            }
        }

        // Check if player's object can act on spawned object (e.g. knife cutting cabbage)
        foreach (var interaction in playerKo.Interactions)
        {
            if (interaction.CanExecute(spawnedKo))
            {
                interaction.Execute(spawnedKo);
                return InteractionResult.Ok("Player's item interacted with spawned object.");
            }
        }

        spawnedKo.DestroySelf();
        return InteractionResult.Fail("Player's item cannot interact with spawned object.");
    }
}