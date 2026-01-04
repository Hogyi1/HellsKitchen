using UnityEngine.Events;

public class DeliveryCounter : CounterModel, IHasCooldown
{

    private CountDownTimer _deliveryTimer;
    private float _deliveryDelay;

    public float DeliveryDelay
    {
        get => _deliveryDelay;
        set => _deliveryDelay = value;
    }

    public event UnityAction<PlateObject> OnPlateDelivered = delegate { };
    public event UnityAction<PlateObject> OnPlateReleased = delegate { };

    private PlateObject plateBeingDelivered;

    public DeliveryCounter(float deliveryDelay)
    {
        _deliveryDelay = deliveryDelay;
        _deliveryTimer = new CountDownTimer(deliveryDelay);
        _deliveryTimer.OnTimerStop += ReleaseChild;
    }

    public void ReleaseChild()
    {
        if (HasChild())
            GetChild().SetParent(null);

        OnPlateReleased.Invoke(plateBeingDelivered);
        ResetDelivery();
    }

    public void StartDelivery(PlateObject plate)
    {
        if (plateBeingDelivered == null)
            return;

        plateBeingDelivered = plate;
        _deliveryTimer.OnTimerStop += FinishedDelivery;
        _deliveryTimer.Start();
    }

    public void FinishedDelivery()
    {
        OnPlateDelivered.Invoke(plateBeingDelivered);
        _deliveryTimer.OnTimerStop -= FinishedDelivery;
    }

    public void ResetDelivery()
    {
        _deliveryTimer.Reset();
        _deliveryTimer.OnTimerStop -= FinishedDelivery;
        plateBeingDelivered = null;
    }

    /// <summary>
    /// Is ready to take another plate because the timer has already stopped
    /// </summary>
    /// <returns></returns>
    public bool IsReady() => !_deliveryTimer.IsRunning;
}