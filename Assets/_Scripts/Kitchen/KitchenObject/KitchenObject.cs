using System.Collections.Generic;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// I aint doing another mvc for this shit
// CodeMonkey typa fel�p�t�s mine better xd
public abstract class KitchenObject : MonoBehaviour, IObjectChild, IHoldableItem
{
    [SerializeField] KitchenObjectSO kitchenObjectSo;
    [SerializeField] public List<IKitchenObjectAction> Interactions = new();

    protected IObjectParent currentParent = null;

    public IObjectParent GetParent() => currentParent;

    public KitchenObjectSO GetKitchenObjectSO() => kitchenObjectSo; // Nem bizti hogy fog kelleni

    /// <summary>
    /// Clears the current parent and then sets the new
    /// </summary>
    /// <param name="parent"></param>
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

    /// <summary>
    /// Clears parent
    /// </summary>
    public void ClearParent()
    {
        currentParent?.SetChild(null);
        currentParent = null;
    }

    void SetTransform(Transform tr)
    {
        transform.position = tr.position;
        transform.SetParent(tr, true);
        transform.localRotation = Quaternion.identity;
    }

    public void Hold() { }

    /// <summary>
    /// Destroys the visual form of this gameObject.
    /// </summary>
    public void DestroySelf()
    {
        Destroy(gameObject);
        SetParent(null);
    }

    /// <summary>
    /// Swaps the parent with the parameter child, only use this on one child at a time
    /// </summary>
    /// <param name="swap"></param>
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
        var ko = instance.GetComponentInChildren<KitchenObject>();
        ko.SetParent(parent);
        return ko;
    }
}
