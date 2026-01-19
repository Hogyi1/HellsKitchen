using UnityEngine;

/// <summary>
/// Manages the interaction logic for a cutting board, handling placing, releasing,
/// and performing cutting actions on KitchenObjects. It implements IObjectHolder
/// and IProgressiveInteraction interfaces.
/// </summary>
public class CuttingController : CounterController, IObjectHolder<KitchenObjectController>, IProgressiveInteraction, IHasCooldown
{
    [SerializeField] float cuttingCooldown = 2f;
    private CountDownTimer _timer;

    /// <summary>
    /// Gets the associated CuttingBoard model.
    /// </summary>
    public CuttingModel Model => GetModel<CuttingModel>();

    /// <summary>
    /// Initializes the controller by creating its CuttingBoard model
    /// and setting up initial interaction predicates.
    /// </summary>
    protected override void Initialize()
    {
        model = new CuttingModel(cuttingCooldown);
        _timer = new(cuttingCooldown);
        predicateList.Add(new EmptyAndEmptyPredicate(this));
    }

    /// <summary>
    /// Places a KitchenObject on the cutting board and initializes the cutting process.
    /// </summary>
    /// <param name="other">The KitchenObject to be placed.</param>
    public void OnPlace(KitchenObjectController other) => InitCutting(other);
    /// <summary>
    /// Checks if a KitchenObject can be released (picked up) from the cutting board.
    /// An item can be released only if the cutting process is considered "done" (output is ready).
    /// </summary>
    /// <returns>True if the item can be released, false otherwise.</returns>
    public bool CanRelease() => IsDone();
    /// <summary>
    /// Checks if a KitchenObject can be placed on the cutting board.
    /// It can be placed if there's a valid cutting recipe for it and the board is currently empty.
    /// </summary>
    /// <param name="other">The KitchenObject to potentially place.</param>
    /// <returns>True if the object can be placed, false otherwise.</returns>
    public bool CanPlace(KitchenObjectController other) => GetRecipeFor(other) != null && Model.CurrentRecipe == null;
    /// <summary>
    /// Handles the release of a KitchenObject from the board, resetting the current recipe.
    /// </summary>
    public void OnRelease() => Model.CurrentRecipe = null;

    /// <summary>
    /// Initializes the cutting process for the item on the board.
    /// Resets cutting progress and sets the current recipe based on the placed item.
    /// </summary>
    /// <param name="other">The KitchenObject to be cut.</param>
    private void InitCutting(KitchenObjectController other)
    {
        Model.ResetCutting();
        _timer.Reset();
        Model.CurrentRecipe = KitchenSODatabase.GetCuttingRecipeWithInput(other);
    }

    /// <summary>
    /// Retrieves the cutting recipe for a given KitchenObject.
    /// </summary>
    /// <param name="other">The KitchenObject to find a recipe for.</param>
    /// <returns>The CuttingRecipeSO if found, otherwise null.</returns>
    private CuttingRecipeSO GetRecipeFor(KitchenObjectController other) => KitchenSODatabase.GetCuttingRecipeWithInput(other);

    /// <summary>
    /// Determines whether the timer is ready for use.
    /// </summary>
    /// <returns>true if the timer is not currently running; otherwise, false.</returns>
    public bool IsReady() => !_timer.IsRunning;

    /// <summary>
    /// Performs a single cutting action on the item currently on the board.
    /// Handles progress, output spawning, and event notification.
    /// </summary>
    public void Cut()
    {
        if (!IsReady()) // Check if the cutting timer is not running
            return;

        Model.OnCut(); // Perform the cut action on the model
        _timer.Start(); // Start the cutting cooldown timer

        // Check if cutting is complete based on the recipe
        if (Model.CuttingTimes >= Model.CurrentRecipe.cuttingTimes)
        {
            (view as CuttingView).OnCuttingAnimationPlayed += SpawnCuttedInstance;
        }
    }

    private void SpawnCuttedInstance()
    {
        Model.GetChild().DestroySelf(); // Destroy the input item

        KitchenObjectController.SpawnKitchenObject(Model.CurrentRecipe.Output, this, view.GetTransformPosition());
        Model.OnFinishedCutting(); // Notify that cutting is finished
        (view as CuttingView).OnCuttingAnimationPlayed -= SpawnCuttedInstance;
    }

    /// <summary>
    /// Determines if the cutting process is effectively "done" for the purpose of releasing the item.
    /// This means the recipe is null (output is already produced) and there's still a child (the output object).
    /// </summary>
    /// <returns>True if the output object is ready for pickup, false otherwise.</returns>
    public bool IsDone() => Model.CurrentRecipe == null && Model.HasChild();

    /// <summary>
    /// Gets the current cutting progress from the model.
    /// </summary>
    /// <returns>The cutting progress as a float (0.0 to 1.0).</returns>
    public float Progress() => Model.CuttingProgress;

    /// <summary>
    /// Triggers a cutting action. Implements IProgressiveInteraction.OnAction.
    /// </summary>
    public void OnAction() => Cut();

    public bool CanAct(IObjectChild child) => Model.CurrentRecipe != null && child == null && IsReady();

}
