using System;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class KitchenCrate : BaseCounter, ISpawner<KitchenObject>
{
    public KitchenObjectSO crateObject;
    public event UnityAction<KitchenObject> OnObjectSpawned = delegate { };

    private void Awake()
    {
        counterTop = counterTop != null ? counterTop : transform.Find("CounterTop");
    }
    public KitchenObject SpawnObject(IObjectParent context)
    {
        var ko = KitchenObject.SpawnVisual(crateObject, context);
        OnObjectSpawned.Invoke(ko);
        return ko;
    }
}
