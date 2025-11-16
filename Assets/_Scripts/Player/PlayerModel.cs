using NUnit.Framework;
using System;
using UnityEngine;

// Under construction
[Serializable]
public class PlayerModel
{
    public PlayerInventoryModel Inventory { get; private set; }
    public float Balance { get; set; }
    public IHoldableItem HeldItem { get; private set; }

    public PlayerModel(PlayerModel savedModel)
    {
        Inventory = savedModel.Inventory;
        Balance = savedModel.Balance;
    }

    public PlayerModel()
    {
        Inventory = new PlayerInventoryModel();
        Balance = 0;
    }

    public void Pickup(IHoldableItem item)
    {
        HeldItem = item;
    }
}

