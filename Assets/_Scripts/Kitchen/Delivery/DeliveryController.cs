using System;
using UnityEngine;

public class DeliveryController : CounterController, IObjectHolder<KitchenObjectController>, IHasCooldown
{
    [SerializeField] float _deliveryTime = 4f;
    private float _graceTime;
    private CountDownTimer _deliveryTimer;
    private CountDownTimer _graceTimer;

    public DeliveryCounter Model => GetModel<DeliveryCounter>();

    protected override void Initialize()
    {
        model = new DeliveryCounter(_deliveryTime);
        _graceTime = _deliveryTime / 2f;

        _deliveryTimer = new CountDownTimer(_deliveryTime);
        _graceTimer = new CountDownTimer(_graceTime);

        var hasPlateAndNotEmpty = new ContextualPredicate<PlayerController>(
            (PlayerController context) =>
            {
                var heldObject = context.TryGetKitchenObject();
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
    }

    private void ScoreDelivery()
    {
        _deliveryTimer.OnTimerStop -= ScoreDelivery;
        OrderManager.Instance.CompleteOrder(Model.Plate.GetIngredientDictionary());
        Model.ReleaseChild();
    }

    public void OnRelease()
    {
        Model.ResetDelivery();
    }

    public bool IsReady() => _deliveryTimer.IsFinished;
}