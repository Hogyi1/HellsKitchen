using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class DispenserModel : CounterModel
{
    public event Action<PlateObject> OnPlateAdded = delegate { };
    public int MaxPlateCount { get; private set; }
    public int PlateCount
    {
        get => _plates.Count;
        set => _ = value;
    }

    private Stack<PlateObject> _plates;

    public DispenserModel(int maxPlateCount)
    {
        MaxPlateCount = maxPlateCount;
        _plates = new(maxPlateCount);
    }

    public List<PlateObject> GetPlates()
    {
        return new List<PlateObject>(_plates);
    }

    public void AddPlate(PlateObject plate)
    {
        if (PlateCount < MaxPlateCount)
        {
            _plates.Push(plate);
            SetChild(plate);
            OnPlateAdded?.Invoke(plate);
        }
    }

    public PlateObject TakePlate()
    {
        if (PlateCount == 0)
            return null;

        var plate = _plates.Pop();
        if (PlateCount > 0)
            SetChild(_plates.Peek());
        else
            ClearChild();
        return plate;
    }
}