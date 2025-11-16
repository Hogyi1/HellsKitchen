using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// idgaf, szét nem szedem viewra
public abstract class BaseCounter : MonoBehaviour, IInteractable, IObjectParent<KitchenObject>
{
    [SerializeField] protected Transform counterTop;

    [SerializeField] protected List<IPredicate> predicateList = new();
    [SerializeField] protected List<KitchenObjectInteractionTags> tags;
    [SerializeField] protected KitchenObject child;
    public event UnityAction<KitchenObject> OnItemChanged = delegate { };
    public void SetChild(KitchenObject child)
    {
        this.child = child;
        OnItemChanged.Invoke(child);
    }

    public Transform GetParentPosition() => counterTop;

    public abstract InteractionResult TryInteract(PlayerController context);

    public virtual bool CanInteract(PlayerController context)
    {
        foreach (var predicate in predicateList)
        {
            if (predicate is ContextualPredicate<PlayerController> contPred)
                contPred.SetContext(context);

            if (!predicate.Evaluate())
                return false;
        }
        return true;
    }

    public bool HasChild() => GetChild() != null;

    public KitchenObject GetChild() => child;

    public void SetChild(IObjectChild child) => SetChild((KitchenObject)child);

    IObjectChild IObjectParent.GetChild() => GetChild();

    private void OnValidate()
    {
        GetChild()?.SetParent(this);
    }
}
