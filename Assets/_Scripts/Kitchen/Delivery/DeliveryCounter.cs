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

    public event UnityAction<PlateObjectController> OnPlateDelivered = delegate { };
    public event UnityAction<PlateObjectController> OnPlateReleased = delegate { };

    private PlateObjectController _plateBeingDelivered;

    public DeliveryCounter(float deliveryDelay)
    {
        _deliveryDelay = deliveryDelay;
    }

    public void ReleaseChild()
    {
        if (HasChild())
            GetChild().SetParent(null);

        OnPlateReleased.Invoke(_plateBeingDelivered);
        ResetDelivery();
    }

    public void StartDelivery(PlateObjectController plate)
    {
        if (_plateBeingDelivered == null)
            return;

        _plateBeingDelivered = plate;
    }

    public void FinishedDelivery()
    {
        OnPlateDelivered.Invoke(_plateBeingDelivered);
    }

    public void ResetDelivery()
    {
        _plateBeingDelivered = null;
    }
}