using System;
using UnityEngine;
using UnityEngine.Events;

public class KitchenCrate : BaseCounter
{
    [SerializeField] KitchenObjectSO crateObject;
    public event UnityAction<KitchenObject> OnObjectSpawned = delegate { };

    private void Awake()
    {
        counterTop = counterTop != null ? counterTop : transform.Find("CounterTop");

        var hasNoItemPredicate = new ContextualPredicate<PlayerController>((context) => context.TryGetKitchenObject() == null);
        predicateList.Add(hasNoItemPredicate);
    }
    public override InteractionResult TryInteract(PlayerController context)
    {
        bool canInteract = CanInteract(context);
        if (!canInteract)
            return InteractionResult.Fail("Your hands are full");

        var ko = KitchenObject.SpawnVisual(crateObject, context);
        OnObjectSpawned.Invoke(ko);
        return InteractionResult.Ok("Object successfully spawned");
    }
}