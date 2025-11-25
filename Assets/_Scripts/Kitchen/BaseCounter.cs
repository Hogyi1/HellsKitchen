using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseCounter : MonoBehaviour, IInteractable, IObjectParent<KitchenObject>, IObjectHolder<KitchenObject>
{
    [SerializeField] protected Transform counterTop;

    protected List<IPredicate> predicateList = new();
    [SerializeField] protected List<BaseCounterStrategy> interactionStrategies = new();
    [SerializeField] protected KitchenObject child;
    public event UnityAction<KitchenObject> OnItemChanged = delegate { };

    public void SetChild(KitchenObject child)
    {
        this.child = child;
        OnItemChanged.Invoke(child);
    }
    public void SetChild(IObjectChild child) => SetChild((KitchenObject)child);

    public virtual InteractionResult TryInteract(PlayerController context)
    {
        bool canInteract = CanInteract(context);
        if (!canInteract)
            return InteractionResult.Fail("Cannot interact right now");

        foreach (var strategy in interactionStrategies)
        {
            if (strategy.CanExecute(context, this))
            {
                return strategy.Execute(context, this);
            }
        }

        return InteractionResult.Fail("No valid strategy");
    }
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
    public Transform GetParentPosition() => counterTop;

    public KitchenObject GetChild() => child;
    IObjectChild IObjectParent.GetChild() => GetChild();

    public virtual bool CanRelease() => true;
    public virtual void OnRelease() { }
    public virtual bool CanPlace(KitchenObject other) => true;
    public virtual void OnPlace(KitchenObject other) { }

    private void OnValidate() => GetChild()?.SetParent(this);
}

// Use strategy for every interaction like TakeObject, PlaceObject, etc.
public abstract class BaseCounterStrategy : InteractionStrategySO
{
    public sealed override InteractionResult Execute(PlayerController context, IInteractable interactable)
    {
        if (interactable is BaseCounter counter)
            return ExecuteOnCounter(context, counter);

        return InteractionResult.Fail("Not a Counter");
    }
    public sealed override bool CanExecute(PlayerController context, IInteractable interactable)
    {
        if (interactable is BaseCounter counter)
            return CanExecuteOnCounter(context, counter);

        return false;
    }

    public abstract InteractionResult ExecuteOnCounter(PlayerController context, BaseCounter counter);
    public abstract bool CanExecuteOnCounter(PlayerController context, BaseCounter counter);
}
