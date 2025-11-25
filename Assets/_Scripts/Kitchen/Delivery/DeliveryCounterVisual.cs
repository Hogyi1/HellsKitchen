using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeliveryCounter))]
public class DeliveryCounterVisual : MonoBehaviour
{
    [SerializeField] DeliveryCounter deliveryCounter;
    [SerializeField] Transform counterEnd;
    [SerializeField] float slideDuration = 2f;

    private Dictionary<PlateObject, Tween> activeTweens = new Dictionary<PlateObject, Tween>();

    private void Awake()
    {
        counterEnd = counterEnd != null ? counterEnd : transform;

        deliveryCounter = deliveryCounter != null ? deliveryCounter : GetComponent<DeliveryCounter>();
        deliveryCounter.OnPlateDelivered += DeliveryCounter_OnPlateDelivered;
        deliveryCounter.OnPlateReleased += DeliveryCounter_OnPlateReleased;
    }

    private void DeliveryCounter_OnPlateDelivered(PlateObject po)
    {
        if (activeTweens.ContainsKey(po))
        {
            activeTweens[po].Kill();
        }

        Tween deliveryTween = DOTween.To(() => po.transform.position,
                                     x => po.transform.position = x,
                                     counterEnd.position,
                                     slideDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                activeTweens.Remove(po);
                Destroy(po.gameObject);
            });

        activeTweens[po] = deliveryTween;
    }

    private void DeliveryCounter_OnPlateReleased(PlateObject po)
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