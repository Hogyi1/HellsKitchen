using UnityEngine;

/// <summary>
/// Manages the logic for a trash bin, which destroys any KitchenObject placed on it.
/// It follows the Counter pattern for interaction but with custom logic to delete items.
/// </summary>
[RequireComponent(typeof(TrashBinView))] // Requires a basic view for the interaction point.
public class TrashBinController : CounterController, IDisposer<KitchenObjectController>
{

    /// <summary>
    /// Initializes the counter's model and sets up interaction predicates.
    /// </summary>
    protected override void Initialize()
    {
        model = new TrashBinModel();
        predicateList.Add(new EmptyAndEmptyPredicate(this));
    }
    public void OnDispose(KitchenObjectController ko)
    {
        var bin = GetModel<TrashBinModel>();
        bin.SetChild(ko);
        ko.Dispose();
    }
}
