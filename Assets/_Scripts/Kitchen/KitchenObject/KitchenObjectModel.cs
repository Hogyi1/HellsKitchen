using System.Collections.Generic;
using UnityEngine.Events;

public class KitchenObjectModel
{
    public KitchenObjectSO KitchenObjectSo { get; }
    private IObjectParent currentParent;

    public event UnityAction<IObjectParent> OnParentChanged = delegate { };

    public KitchenObjectModel(KitchenObjectSO kitchenObjectSo)
    {
        KitchenObjectSo = kitchenObjectSo;
    }

    public IObjectParent GetParent() => currentParent;

    public void SetParent(IObjectParent parent)
    {
        if (currentParent == parent) return;

        ClearParent();
        currentParent = parent;
        OnParentChanged.Invoke(currentParent);
    }

    public void ClearParent()
    {
        currentParent?.SetChild(null);
        currentParent = null;
        OnParentChanged.Invoke(null);
    }

    public void SwapParent(IObjectChild swap, IObjectChild self)
    {
        var parentA = currentParent;
        var parentB = swap.GetParent();

        if (parentA == parentB)
            return;

        swap.ClearParent();
        self.ClearParent();

        self.SetParent(parentB);
        swap.SetParent(parentA);
    }
}