using UnityEngine;

public class IdleState : StoveBaseState
{
    public IdleState(StoveCounter stoveCounter) : base(stoveCounter) { }

    public override void OnEnter()
    {
        if (stoveCounter.HasChild())
        {
            var ko = stoveCounter.GetChild();
            ko.DestroySelf();
        }
    }

    public override void OnExit() { }

    public override InteractionResult TryInteract(PlayerController context)
    {
        var ko = context.TryGetKitchenObject();
        var fr = stoveCounter.GetFryingRecipe(ko);
        if (fr != null)
        {
            ko.SetParent(stoveCounter);
            stoveCounter.InitFrying(fr);
            return InteractionResult.Ok("Placed item on stove and started cooking.");
        }

        return InteractionResult.Fail("Item cannot be cooked on the stove.");
    }
}

public class FryingState : StoveBaseState
{
    CountUpTimer cookingTimer;
    public FryingState(StoveCounter stoveCounter, ref CountUpTimer cookingTimer) : base(stoveCounter)
    {
        this.cookingTimer = cookingTimer;
    }
    public override void OnEnter()
    {
        cookingTimer.Start();
    }
    public override void OnExit()
    {
        cookingTimer.Stop();
    }
    public override InteractionResult TryInteract(PlayerController context)
    {
        // If we want to take it and track the progress do it here
        return InteractionResult.Fail("Cannot interact while frying.");
    }
}
public class FriedState : StoveBaseState
{
    CountUpTimer burnTimer;
    public FriedState(StoveCounter stoveCounter, ref CountUpTimer burnTimer) : base(stoveCounter)
    {
        this.burnTimer = burnTimer;
    }
    public override void OnEnter()
    {
        var ownKo = stoveCounter.GetChild();
        ownKo.DestroySelf();
        KitchenObject.SpawnVisual(stoveCounter.currentRecipe.to, stoveCounter);
        burnTimer.Start();
    }
    public override void OnExit()
    {
        burnTimer.Stop();
    }
    public override InteractionResult TryInteract(PlayerController context)
    {
        var ownKo = stoveCounter.GetChild();
        var playerKo = context.TryGetKitchenObject();
        var fr = stoveCounter.GetFryingRecipe(playerKo);

        if (fr != null)
        {
            ownKo.SwapParent(playerKo);
            stoveCounter.InitFrying(fr);
            return InteractionResult.Ok("Swapped item on stove with player's item and started cooking.");
        }

        if (playerKo == null)
        {
            ownKo.SetParent(context);
            stoveCounter.ResetStove();
            return InteractionResult.Ok("Took cooked item from stove.");
        }

        if (playerKo is PlateObject po)
        {
            po.AddIngredient(ownKo);
            stoveCounter.ResetStove();
            return InteractionResult.Ok("Added cooked item to plate.");
        }

        return InteractionResult.Fail("Cannot interact with nonfriable.");
    }
}

public class BurnedState : StoveBaseState
{
    public BurnedState(StoveCounter stoveCounter) : base(stoveCounter) { }
    public override void OnEnter()
    {
        var ownKo = stoveCounter.GetChild();
        ownKo.DestroySelf();
        KitchenObject.SpawnVisual(stoveCounter.currentRecipe.burnt, stoveCounter);
    }
    public override void OnExit() { }
    public override InteractionResult TryInteract(PlayerController context)
    {
        var ownKo = stoveCounter.GetChild();
        var playerKo = context.TryGetKitchenObject();
        var fr = stoveCounter.GetFryingRecipe(playerKo);

        if (fr != null)
        {
            ownKo.SwapParent(playerKo);
            stoveCounter.InitFrying(fr);
            return InteractionResult.Ok("Swapped item on stove with player's item and started cooking.");
        }

        if (playerKo == null)
        {
            ownKo.SetParent(context);
            stoveCounter.ResetStove();
            return InteractionResult.Ok("Took cooked item from stove.");
        }

        return InteractionResult.Ok("Cannot interact with nonfriable.");
    }
}