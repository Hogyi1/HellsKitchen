using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class DeliveryCounter : CounterModel
{

    private float _deliveryDelay;

    public float DeliveryDelay
    {
        get => _deliveryDelay;
        set => _deliveryDelay = value;
    }

    public PlateObjectController Plate
    {
        get => _plateBeingDelivered;
        set => _plateBeingDelivered = value;
    }

    private PlateObjectController _lastPlate;

    public PlateObjectController LastPlate
    {
        get => _lastPlate;
        set => _lastPlate = value;
    }

    public event UnityAction<PlateObjectController> OnPlateDelivered = delegate { };
    public event UnityAction<PlateObjectController> OnPlateReleased = delegate { };

    private PlateObjectController _plateBeingDelivered;

    public DeliveryCounter(float deliveryDelay)
    {
        _deliveryDelay = deliveryDelay;
    }

    public void ReleaseChild()
    {
        GetChild().SetParent(null);
    }

    public void StartDelivery(PlateObjectController plate)
    {
        if (_plateBeingDelivered != null)
            return;

        _plateBeingDelivered = plate;
    }

    public void FinishedDelivery()
    {
        OnPlateDelivered.Invoke(_plateBeingDelivered);
        _lastPlate = _plateBeingDelivered;
        _plateBeingDelivered = null;
    }

    public void ResetDelivery()
    {
        OnPlateReleased.Invoke(_plateBeingDelivered);
        _lastPlate = null;
        _plateBeingDelivered = null;
    }
}