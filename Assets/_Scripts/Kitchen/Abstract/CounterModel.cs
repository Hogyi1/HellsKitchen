using UnityEngine.Events;

/// <summary>
/// Abstract base class for counter data models. It holds the state of a counter,
/// primarily which <see cref="KitchenObjectController"/> it is currently holding.
/// </summary>
public abstract class CounterModel
{
    /// <summary>
    /// The kitchen object currently held by this counter.
    /// </summary>
    protected KitchenObjectController child;

    /// <summary>
    /// Invoked whenever the kitchen object on this counter changes (including being set to null).
    /// </summary>
    public event UnityAction<KitchenObjectController> OnItemChanged = delegate { };

    /// <summary>
    /// Sets the kitchen object on this counter and invokes the OnItemChanged event.
    /// </summary>
    /// <param name="child">The kitchen object to place on the counter.</param>
    public virtual void SetChild(KitchenObjectController child)
    {
        this.child = child;
        OnItemChanged.Invoke(child);
    }

    /// <summary>
    /// Gets the kitchen object currently on this counter.
    /// </summary>
    /// <returns>The <see cref="KitchenObjectController"/> instance, or null if empty.</returns>
    public KitchenObjectController GetChild() => child;

    /// <summary>
    /// Removes the kitchen object from this counter by setting the child to null.
    /// </summary>
    public void ClearChild() => SetChild(null);

    /// <summary>
    /// Checks if the counter is currently holding a kitchen object.
    /// </summary>
    /// <returns>True if there is a child object, false otherwise.</returns>
    public bool HasChild() => child != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="CounterModel"/> class with no child object.
    /// </summary>
    public CounterModel()
    {
        this.child = null;
    }
}
