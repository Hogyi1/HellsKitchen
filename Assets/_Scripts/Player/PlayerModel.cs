using NUnit.Framework;
using System;
using UnityEngine;

// Under construction
[Serializable]
public class PlayerModel
{
    public float Balance { get; set; }
    public IHoldableItem HeldItem { get; private set; }

    public PlayerModel(PlayerModel savedModel)
    {
        Balance = savedModel.Balance;
    }

    public PlayerModel()
    {
        Balance = 0;
    }

    public void Pickup(IHoldableItem item)
    {
        HeldItem = item;
    }
}

