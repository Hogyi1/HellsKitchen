using DG.Tweening;
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
        Model.OnPlateDelivered += DeliveryCounter_OnPlateDelivered;
        Model.OnPlateReleased += DeliveryCounter_OnPlateReleased;

        _slideDuration = Model.DeliveryDelay;
    }

    protected override void SetupComponents()
    {
        counterEnd = counterEnd != null ? counterEnd : transform;
    }

    private void DeliveryCounter_OnPlateDelivered(PlateObjectController po)
    {
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
            .OnComplete(() =>
            {
                activeTweens.Remove(po);
                Destroy(po.gameObject);
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