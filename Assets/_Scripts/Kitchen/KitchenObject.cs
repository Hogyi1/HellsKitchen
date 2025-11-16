using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.UIElements;

// I aint doing another mvc for this shit
// CodeMonkey typa felépítés mine better xd
public class KitchenObject : MonoBehaviour, IObjectChild, IHoldableItem
{
    [SerializeField] KitchenObjectSO kitchenObjectSo;

    public string Name = "ASD";
    public List<KitchenObjectInteractionTags> Tags { get; set; }
    IObjectParent currentParent;

    public IObjectParent GetParent() => currentParent;

    public void SetParent(IObjectParent parent)
    {
        ClearParent();

        if (parent != null)
        {
            parent.SetChild(this);
            SetTransform(parent.GetParentPosition());
        }

        currentParent = parent;
    }

    public void ClearParent()
    {
        currentParent?.SetChild(null);
        currentParent = null;
    }

    void SetTransform(Transform tr)
    {
        transform.position = tr.position;
        transform.SetParent(tr);
    }

    public void Hold() { }

    public void DestroySelf()
    {
        Destroy(gameObject);
        SetParent(null);
    }

    /// <summary>
    /// Spawns the kitchenobject, also sets the parent
    /// </summary>
    /// <param name="so"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static KitchenObject SpawnVisual(KitchenObjectSO so, IObjectParent parent)
    {
        var parentTransform = parent.GetParentPosition();
        GameObject instance = Instantiate(so.Prefab, parentTransform.position, Quaternion.identity);
        instance.transform.SetParent(parentTransform, true);
        var ko = instance.GetComponentInChildren<KitchenObject>();
        ko.SetParent(parent);
        return ko;
    }

    public void SwapParent(IObjectChild swap)
    {
        var parentA = currentParent;
        var parentB = swap.GetParent();

        if (parentA == parentB)
            return;

        swap.ClearParent();
        ClearParent();

        SetParent(parentB);
        swap.SetParent(parentA);
    }
}

public interface IObjectParent
{
    Transform GetParentPosition();
    void SetChild(IObjectChild child);
    IObjectChild GetChild();
}

public interface IObjectChild
{
    void SetParent(IObjectParent parent);
    void ClearParent();
    void SwapParent(IObjectChild swap);
    IObjectParent GetParent();
}

public interface IObjectParent<T> : IObjectParent where T : IObjectChild
{
    void SetChild(T child);
    new T GetChild();
}

public enum KitchenObjectInteractionTags
{
    Any,
    Cut,
    Cooked
}