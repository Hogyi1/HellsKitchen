using DG.Tweening;
using System;
using System.Collections.Generic;
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
}
