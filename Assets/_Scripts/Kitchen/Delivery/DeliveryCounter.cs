using System;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryCounter : BaseCounter
{

    CountDownTimer deliveryTimer;
    [SerializeField] float deliveryDelay = 0.5f;
    public event UnityAction<PlateObject> OnPlateDelivered = delegate { };
    public event UnityAction<PlateObject> OnPlateReleased = delegate { };

    private PlateObject plateBeingDelivered;

    public bool debugTimer => deliveryTimer.IsRunning;

    private void Awake()
    {
        counterTop = counterTop != null ? counterTop : transform.Find("CounterTop");

        var hasPlateAndNotEmpty = new ContextualPredicate<PlayerController>(
            (PlayerController context) =>
            {
                var heldObject = context.TryGetKitchenObject();
                if (heldObject is PlateObject plate)
                {
                    return !plate.IsEmpty();
                }

                return heldObject == null;
            }
        );

        predicateList.Add(hasPlateAndNotEmpty);

        deliveryTimer = new CountDownTimer(deliveryDelay);
        deliveryTimer.OnTimerStop += ReleaseChild;
    }

    private void ReleaseChild()
    {
        OrderManager.Instance.CompleteOrder(plateBeingDelivered.GetIngredientDictionary());

        if (HasChild())
            GetChild().SetParent(null);

        plateBeingDelivered = null;
    }

    public override bool CanRelease() => deliveryTimer.IsRunning;

    public override bool CanPlace(KitchenObject other) => !deliveryTimer.IsRunning;

    public override void OnPlace(KitchenObject other)
    {
        plateBeingDelivered = (PlateObject)other;
        deliveryTimer.Start();
        OnPlateDelivered.Invoke(plateBeingDelivered);
    }

    public override void OnRelease()
    {
        if (plateBeingDelivered != null)
        {
            OnPlateReleased.Invoke(plateBeingDelivered);
            plateBeingDelivered = null;
        }
        deliveryTimer.Stop();
    }
}