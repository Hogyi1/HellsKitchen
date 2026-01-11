using System;

public class TrashBinModel : CounterModel
{
    public event Action<KitchenObjectController> OnItemDisposed = delegate { };

    /// <summary>
    /// Overrides the base method to destroy any KitchenObject placed on the trash bin.
    /// </summary>
    /// <param name="child">The KitchenObject to be placed.</param>
    public override void SetChild(KitchenObjectController child)
    {
        OnItemDisposed?.Invoke(child);
        return;
    }
}
