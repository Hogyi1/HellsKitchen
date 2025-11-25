using UnityEngine;
using UnityEngine.Events;

public class PlateDispenser : BaseCounter, ISpawner<KitchenObject>
{
    public KitchenObjectSO plateObject;
    public event UnityAction<KitchenObject> OnObjectSpawned = delegate { };
    public event UnityAction OnRefill = delegate { };

    int plateCount;
    LoopTimer refillTimer;

    public float RefillTime = 4f;
    public int MaxPlateCount = 5;

    private void Awake()
    {
        counterTop = counterTop != null ? counterTop : transform.Find("CounterTop");
        refillTimer = new LoopTimer(RefillTime, -1);

        var hasPlatePredicate = new FunctionPredicate(() => plateCount >= 1);
        predicateList.Add(hasPlatePredicate);
    }

    private void OnEnable()
    {
        refillTimer.OnLoop += Refill;
        refillTimer.Start();
    }

    private void OnDisable() => refillTimer.OnLoop -= Refill;

    public KitchenObject SpawnObject(IObjectParent context)
    {
        var ko = KitchenObject.SpawnVisual(plateObject, context) as PlateObject;
        OnObjectSpawned.Invoke(ko);
        return ko;
    }

    void Refill(int round)
    {
        if (plateCount < MaxPlateCount)
        {
            plateCount++;
            OnRefill.Invoke();
        }
    }
}
