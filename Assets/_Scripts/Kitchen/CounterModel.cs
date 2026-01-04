using UnityEngine.Events;

public abstract class CounterModel : IObjectParent<KitchenObject>
{
    protected KitchenObject child;
    public event UnityAction<KitchenObject> OnItemChanged = delegate { };

    public virtual void SetChild(KitchenObject child)
    {
        this.child = child;
        OnItemChanged.Invoke(child);
    }
    public void SetChild(IObjectChild child) => SetChild((KitchenObject)child);

    public KitchenObject GetChild() => child;
    IObjectChild IObjectParent.GetChild() => GetChild();

    public void ClearChild() => SetChild(null);

    public bool HasChild() => child != null;

    public CounterModel(KitchenObject kitchenObject)
    {
        this.child = kitchenObject;
    }

    public CounterModel()
    {
        this.child = null;
    }
}
