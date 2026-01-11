using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(DispenserController))]
public class DispenserView : CounterView
{
    [SerializeField] Animator anim;

    public float plateHeight = 0.1f; // Adjusted for a more realistic stacking
    public float time = 0.2f;
    private DispenserModel _myModel => GetModel<DispenserModel>();

    protected override void SetupComponents()
    {
        anim = anim != null ? anim : gameObject.AddComponent<Animator>();
    }

    public void AdjustPlateheight()
    {
        List<PlateObjectController> plates = _myModel.GetPlates();
        plates.Reverse();
        for (int i = 0; i < plates.Count; i++)
        {
            var targetPosition = new Vector3(plates[i].transform.localPosition.x,
                                             plateHeight * i,
                                             plates[i].transform.localPosition.z);
            plates[i].transform.DOLocalMove(targetPosition, time).SetEase(Ease.OutQuad);
        }
    }
}
