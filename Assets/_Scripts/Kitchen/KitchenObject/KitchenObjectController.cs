using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(40)]
public abstract class KitchenObjectController : MonoBehaviour, IObjectChild, IHoldableItem
{
    protected KitchenObjectModel model;
    [SerializeField] protected KitchenObjectView view;
    [SerializeField] protected KitchenObjectSO so;
    public List<IKitchenObjectAction> Interactions { get; } = new();

    private void Awake()
    {
        view = view != null ? view : GetComponentInChildren<KitchenObjectView>();
        Initialize();
        view.BindModel(model);
    }

    public IObjectParent GetParent() => model.GetParent();
    public KitchenObjectSO GetKitchenObjectSO() => model.KitchenObjectSo;

    public void SetParent(IObjectParent parent)
    {
        model.SetParent(parent);

        if (parent != null)
        {
            parent.SetChild(this);
            view.SetTransform(parent.GetTransform());
        }
    }

    public void ClearParent() => model.ClearParent();

    public abstract void Hold();

    public void SwapParent(IObjectChild swap) => model.SwapParent(swap, this);

    public void DestroySelf()
    {
        model.GetParent()?.SetChild(null);
        Destroy(gameObject);
    }

    public static KitchenObjectController SpawnKitchenObject(KitchenObjectSO so, IObjectParent<KitchenObjectController> parent)
    {
        GameObject instance = Instantiate(so.Prefab, parent.GetTransform().position, Quaternion.identity, parent.GetTransform());
        var ko = instance.GetComponentInChildren<KitchenObjectController>();
        ko.SetParent(parent);
        return ko;
    }

    public static KitchenObjectController SpawnKitchenObject(KitchenObjectSO so, IObjectParent<KitchenObjectController> parent, Transform transform)
    {
        GameObject instance = Instantiate(so.Prefab, transform.position, Quaternion.identity, transform);
        var ko = instance.GetComponentInChildren<KitchenObjectController>();
        ko.SetParent(parent);
        return ko;
    }

    public static KitchenObjectController SpawnKitchenObject(KitchenObjectSO so, IObjectParent parent, Transform transform)
    {
        GameObject instance = Instantiate(so.Prefab, transform.position, Quaternion.identity, transform);
        var ko = instance.GetComponentInChildren<KitchenObjectController>();
        ko.SetParent(parent);
        return ko;
    }

    public KitchenObjectModel GetModel() => model;

    /// <summary>
    /// In start 
    /// </summary>
    public abstract void Initialize();
}
