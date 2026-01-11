using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class DispenserModel : CounterModel
{
    public event Action<PlateObjectController> OnPlateAdded = delegate { };
    public int MaxPlateCount { get; private set; }
    public int PlateCount
    {
        get => _plates.Count;
        set => _ = value;
    }

    private Queue<PlateObjectController> _plates;

    public DispenserModel(int maxPlateCount)
    {
        MaxPlateCount = maxPlateCount;
        _plates = new(maxPlateCount);
    }

    public List<PlateObjectController> GetPlates()
    {
        return new List<PlateObjectController>(_plates);
    }

    public void AddPlate(PlateObjectController plate)
    {
        if (PlateCount < MaxPlateCount)
        {
            //_plates.Push(plate);
            _plates.Enqueue(plate);
            if (!HasChild())
                SetChild(plate);
            OnPlateAdded?.Invoke(plate);
        }
    }

    public PlateObjectController TakePlate()
    {
        if (PlateCount == 0)
            return null;

        //var plate = _plates.Pop();
        var plate = _plates.Dequeue();
        if (PlateCount > 0)
            SetChild(_plates.Peek());
        else
            ClearChild();
        return plate;
    }
}