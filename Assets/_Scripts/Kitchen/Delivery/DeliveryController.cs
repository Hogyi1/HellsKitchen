using System;
using UnityEngine;

public class DeliveryController : CounterController, IObjectHolder<KitchenObjectController>, IHasCooldown
{
    [SerializeField] float _deliveryTime = 4f;
    [SerializeField] float _coolDown = 1f;
    private CountDownTimer _deliveryTimer;
    private CountDownTimer _coolDownTimer;

    [SerializeField] private OrderManager orderManager;

    public DeliveryCounter Model => GetModel<DeliveryCounter>();

    protected override void Initialize()
    {
        model = new DeliveryCounter(_deliveryTime);
        _deliveryTimer = new CountDownTimer(_deliveryTime);
        _coolDownTimer = new CountDownTimer(_coolDown);
        orderManager = orderManager != null ? orderManager : FindAnyObjectByType<OrderManager>();

        var hasPlateAndNotEmpty = new ContextualPredicate<PlayerController>(
            (PlayerController context) =>
            {
                var heldObject = context.GetChild() as KitchenObjectController;
                if (heldObject is PlateObjectController plate)
                {
                    return !plate.IsEmpty();
                }

                return heldObject == null;
            }
        );

        predicateList.Add(hasPlateAndNotEmpty);
    }

    public bool CanRelease() => !IsReady();

    public bool CanPlace(KitchenObjectController other) => IsReady() && other is PlateObjectController;

    public void OnPlace(KitchenObjectController other)
    {
        _deliveryTimer.OnTimerStop += ScoreDelivery;
        Model.StartDelivery(other as PlateObjectController);
        _deliveryTimer.Start();
        _coolDownTimer.Start();
    }

    private void ScoreDelivery()
    {
        _deliveryTimer.OnTimerStop -= ScoreDelivery;
        orderManager.CompleteOrder(Model.Plate.GetIngredientDictionary());
        _coolDownTimer.Stop();
        Model.ReleaseChild();
    }

    public void OnRelease()
    {
        Model.ResetDelivery();
        _coolDownTimer.Stop();
        _deliveryTimer.OnTimerStop -= ScoreDelivery;
        _deliveryTimer.Stop();
    }

    public bool IsReady() => !_deliveryTimer.IsRunning;
}