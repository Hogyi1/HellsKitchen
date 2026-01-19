using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DispenserController : CounterController, ISpawner<PlateObjectController>
{
    [SerializeField] private int _maxPlateCount;
    [SerializeField] private float _refillTime;
    [SerializeField] private KitchenObjectSO _platePrefab;

    private LoopTimer _refillTimer;

    public DispenserModel Model => GetModel<DispenserModel>();

    public PlateObjectController GetSpawnerObject() => Model.TakePlate();

    public Transform GetSpawnPosition() => view.GetTransformPosition();

    public PlateObjectController SpawnObject(IObjectParent context, Transform transform)
    {
        if (Model.PlateCount <= 0)
            return null;
        var plate = Model.TakePlate();
        plate.SetParent(context);
        (view as DispenserView).AdjustPlateheight();
        return plate;
    }

    protected override void Initialize()
    {
        model = new DispenserModel(_maxPlateCount);
        _refillTimer = new LoopTimer(_refillTime, -1);

        var hasPlatePredicate = new FunctionPredicate(() => Model.PlateCount >= 1);
        predicateList.Add(hasPlatePredicate);

        _refillTimer.OnLoop += RefillPlate;
        _refillTimer.Start();
    }

    private void OnDisable()
    {
        _refillTimer.OnLoop -= RefillPlate;
        _refillTimer.Stop();
    }

    private void RefillPlate(int round)
    {
        var plate = KitchenObjectController.SpawnKitchenObject(_platePrefab, this);
        Model.AddPlate(plate as PlateObjectController);
        (view as DispenserView).AdjustPlateheight();
    }
}