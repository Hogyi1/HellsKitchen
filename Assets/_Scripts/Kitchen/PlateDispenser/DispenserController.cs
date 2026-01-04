using System;
using UnityEngine;

public class DispenserController : CounterController
{
    [SerializeField] private int _maxPlateCount;
    [SerializeField] private float _refillTime;
    [SerializeField] private PlateObject _platePrefab;

    private LoopTimer _refillTimer;

    public DispenserModel Model => GetModel<DispenserModel>();

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
        Model.AddPlate(Instantiate(_platePrefab, gameObject.transform));
    }


}