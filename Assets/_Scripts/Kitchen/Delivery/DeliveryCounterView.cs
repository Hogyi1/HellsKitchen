using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeliveryController))]
public class DeliveryCounterView : CounterView
{
    [SerializeField] Transform counterEnd;
    float _slideDuration;

    public DeliveryCounter Model => GetModel<DeliveryCounter>();

    private Dictionary<PlateObjectController, Tween> activeTweens = new Dictionary<PlateObjectController, Tween>();

    protected override void Initialize()
    {
        Model.OnItemChanged += DeliveryCounter_OnPlateDelivered;
        Model.OnPlateReleased += DeliveryCounter_OnPlateReleased;

        _slideDuration = Model.DeliveryDelay * 2f;
    }

    protected override void SetupComponents()
    {
        counterEnd = counterEnd != null ? counterEnd : transform;
    }

    private void DeliveryCounter_OnPlateDelivered(KitchenObjectController ko)
    {
        var po = ko as PlateObjectController;
        if (po == null)
            return;

        if (activeTweens.ContainsKey(po))
        {
            activeTweens[po].Kill();
        }

        Tween deliveryTween =
            DOTween.To(() => po.transform.position,
                         x => po.transform.position = x,
                         counterEnd.position,
                         _slideDuration)
            .SetEase(Ease.Linear)
            .SetDelay(0.2f)
            .OnComplete(() =>
            {
                activeTweens.Remove(po);
                po.DestroySelf();
            });

        activeTweens[po] = deliveryTween;
    }

    private void DeliveryCounter_OnPlateReleased(PlateObjectController po)
    {
        if (activeTweens.TryGetValue(po, out Tween activeTween))
        {
            activeTween.Kill();
            activeTweens.Remove(po);
        }
    }

    private void OnDestroy()
    {
        foreach (var tween in activeTweens.Values)
        {
            tween.Kill();
        }
    }
}