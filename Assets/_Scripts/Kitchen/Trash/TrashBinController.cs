using UnityEngine;

/// <summary>
/// Manages the logic for a trash bin, which destroys any KitchenObject placed on it.
/// It follows the Counter pattern for interaction but with custom logic to delete items.
/// </summary>
[RequireComponent(typeof(TrashBinView))] // Requires a basic view for the interaction point.
public class TrashBinController : CounterController, IDisposer
{
    /// <summary>
    /// Initializes the counter's model and sets up interaction predicates.
    /// </summary>
    protected override void Initialize()
    {
        model = new TrashBinModel();

        var emptyAndEmpty = new ContextualPredicate<PlayerController>((context) =>
        {
            return !(context.TryGetKitchenObject() == null && !model.HasChild());
        });
        predicateList.Add(emptyAndEmpty);
    }
    public void OnDispose(IDisposable ko)
    {
        var bin = model as TrashBinModel;
        bin.SetChild(ko as KitchenObjectController);
        ko.Dispose();
    }
}
