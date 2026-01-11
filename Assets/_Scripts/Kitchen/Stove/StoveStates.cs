using UnityEngine;

public class IdleState : StoveBaseState
{
    public IdleState(StoveController stoveCounter) : base(stoveCounter) { }

    public override void OnEnter()
    {
        if (stoveCounter.HasChild())
        {
            var ko = stoveCounter.GetChild();
            ko.DestroySelf();
        }

        stoveCounter.InvokeStateChange(StoveModel.StoveState.Idle);
        stoveCounter.Model.FryingProgress = 0f;
        stoveCounter.Model.BurningProgress = 0f;
    }

    public override void OnExit() { }

    public override InteractionResult TryInteract(PlayerController context)
    {
        var ko = context.TryGetKitchenObject();
        var fr = KitchenSODatabase.Instance.GetFryingRecipeWithInput(ko);
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
    CountUpTimer fryingTimer;
    public FryingState(StoveController stoveCounter, ref CountUpTimer cookingTimer) : base(stoveCounter)
    {
        this.fryingTimer = cookingTimer;
    }
    public override void OnEnter()
    {
        fryingTimer.Start();
        stoveCounter.InvokeStateChange(StoveModel.StoveState.Frying);
    }

    public override void Update()
    {
        stoveCounter.Model.FryingProgress = fryingTimer.Progress;
    }

    public override void OnExit()
    {
        fryingTimer.Stop();
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
    public FriedState(StoveController stoveCounter, ref CountUpTimer burnTimer) : base(stoveCounter)
    {
        this.burnTimer = burnTimer;
    }
    public override void OnEnter()
    {
        var ownKo = stoveCounter.GetChild();
        ownKo.DestroySelf();
        KitchenObjectController.SpawnKitchenObject(stoveCounter.Model.CurrentRecipe.To, stoveCounter);
        burnTimer.Start();
        stoveCounter.InvokeStateChange(StoveModel.StoveState.Fried);
    }

    public override void Update()
    {
        stoveCounter.Model.BurningProgress = burnTimer.Progress;
    }

    public override void OnExit()
    {
        burnTimer.Stop();
    }
    public override InteractionResult TryInteract(PlayerController context)
    {
        var ownKo = stoveCounter.GetChild();
        var playerKo = context.TryGetKitchenObject();
        var fr = KitchenSODatabase.Instance.GetFryingRecipeWithInput(playerKo);

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

        if (playerKo is PlateObjectController po)
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
    public BurnedState(StoveController stoveCounter) : base(stoveCounter) { }
    public override void OnEnter()
    {
        var ownKo = stoveCounter.GetChild();
        ownKo.DestroySelf();
        KitchenObjectController.SpawnKitchenObject(stoveCounter.Model.CurrentRecipe.Burnt, stoveCounter);
        stoveCounter.InvokeStateChange(StoveModel.StoveState.Burnt);
        stoveCounter.Model.BurningProgress = 1f;
    }
    public override void OnExit() { }
    public override InteractionResult TryInteract(PlayerController context)
    {
        var ownKo = stoveCounter.GetChild();
        var playerKo = context.TryGetKitchenObject();
        var fr = KitchenSODatabase.Instance.GetFryingRecipeWithInput(playerKo);

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