using UnityEngine;

/// <summary>
/// Manages the logic for a clear counter, which can hold or release a single KitchenObject.
/// It acts as a basic surface for placing and picking up items.
/// </summary>
[RequireComponent(typeof(ClearCounterView))]
public class ClearCounterController : CounterController, IObjectHolder<KitchenObjectController>
{
    /// <summary>
    /// Initializes the counter's model and sets up interaction predicates.
    /// </summary>
    protected override void Initialize()
    {
        model = new BaseCounter();

        var emptyAndEmpty = new ContextualPredicate<PlayerController>((context) =>
        {
            return !(context.TryGetKitchenObject() == null && !model.HasChild());
        });
        predicateList.Add(emptyAndEmpty);
    }

    /// <summary>
    /// Checks if a KitchenObject can be placed on this counter.
    /// </summary>
    /// <param name="other">The KitchenObject to be placed.</param>
    /// <returns>True if the object can be placed, false otherwise.</returns>
    public bool CanPlace(KitchenObjectController other) => (other != model.GetChild());

    /// <summary>
    /// Checks if a KitchenObject can be released (picked up) from this counter.
    /// </summary>
    /// <returns>True if the counter has a child object, false otherwise.</returns>
    public bool CanRelease() => model.HasChild();

    /// <summary>
    /// Places a KitchenObject on the counter by setting its parent.
    /// </summary>
    /// <param name="other">The KitchenObject to place.</param>
    public void OnPlace(KitchenObjectController other) => other.SetParent(this);

    /// <summary>
    /// Releases the KitchenObject from the counter.
    /// </summary>
    public void OnRelease() => model.ClearChild();
}