using System;
using UnityEngine;

public class DeliveryController : CounterController, IObjectHolder<KitchenObject>
{
    [SerializeField] float _deliveryTime = 4f;

    public DeliveryCounter Model => GetModel<DeliveryCounter>();

    protected override void Initialize()
    {
        model = new DeliveryCounter(_deliveryTime);

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
    }

    public bool CanRelease() => !Model.IsReady();

    public bool CanPlace(KitchenObject other) => Model.IsReady() && other is PlateObject;

    public void OnPlace(KitchenObject other)
    {
        Model.StartDelivery(other as PlateObject);
        Model.OnPlateDelivered += ScoreDelivery;
    }

    private void ScoreDelivery(PlateObject plate)
    {
        OrderManager.Instance.CompleteOrder(plate.GetIngredientDictionary());
        Model.ReleaseChild();
    }

    public void OnRelease()
    {
        Model.ResetDelivery();
    }

}