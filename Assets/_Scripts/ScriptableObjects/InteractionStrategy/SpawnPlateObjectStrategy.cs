using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleSpawnStrategy", menuName = "Game/Strategy/CounterStrategy/SpawnStrategy/PlateSpawn")]
public class SpawnPlateObjectStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var playerKo = context.TryGetKitchenObject();

        if (counter is not ISpawner<PlateObject> || counter is not PlateDispenser)
            return false;

        if (playerKo == null)
            return true;

        if (playerKo is Ingredient)
            return true;

        return false;
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, BaseCounter counter)
    {
        var playerKo = context.TryGetKitchenObject();
        var spawner = counter as ISpawner<PlateObject>;
        var plateDispenser = counter as PlateDispenser;

        if (spawner == null || plateDispenser == null)
        {
            return InteractionResult.Fail("Counter is not a valid plate dispenser.");
        }

        if (playerKo != null)
        {
            var plate = spawner.SpawnObject(plateDispenser);

            if (plate.CanAddIngredient(playerKo))
            {
                plate.AddIngredient(playerKo);

                plate.SetParent(context);
                return InteractionResult.Ok("Placed ingredient on new plate.");
            }
            else
            {
                plate.DestroySelf();
                return InteractionResult.Fail("Cannot add this ingredient to the plate.");
            }
        }
        else
        {
            spawner.SpawnObject(context);
            return InteractionResult.Ok("Took a plate.");
        }
    }
}
