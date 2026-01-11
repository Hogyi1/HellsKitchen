using UnityEngine;

[CreateAssetMenu(fileName = "NewSimpleSpawnStrategy", menuName = "Game/Strategy/CounterStrategy/SpawnStrategy/PlateSpawn")]
public class SpawnPlateObjectStrategy : BaseCounterStrategy
{
    public override bool CanExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var playerKo = context.TryGetKitchenObject();

        if (counter is not ISpawner<PlateObjectController> || counter.GetModel() is not DispenserModel)
            return false;

        if (playerKo == null)
            return true;

        if (playerKo is IngredientController)
            return true;

        return false;
    }

    public override InteractionResult ExecuteOnCounter(PlayerController context, CounterController counter)
    {
        var playerKo = context.TryGetKitchenObject();
        var spawner = counter as ISpawner<PlateObjectController>;
        var plateDispenser = counter.GetModel() as DispenserModel;

        if (spawner == null || plateDispenser == null)
        {
            return InteractionResult.Fail("Counter is not a valid plate dispenser.");
        }

        //TODO
        if (playerKo != null)
        {
            var plate = spawner.GetSpawnerObject();

            if (plate.CanAddIngredient(playerKo))
            {
                plate.AddIngredient(playerKo);

                plate.SetParent(context);
                return InteractionResult.Ok("Placed ingredient on new plate.");
            }
            else
            {
                return InteractionResult.Fail("Cannot add this ingredient to the plate.");
            }
        }
        else
        {
            spawner.SpawnObject(context, context.GetTransform());
            return InteractionResult.Ok("Took a plate.");
        }
    }
}
