using NUnit.Framework;
using UnityEngine;

// Under construction
public class PlayerModel
{
    public PlayerInventoryModel Inventory { get; private set; }
    public float Balance { get; set; }
    public IInventoryItem EquippedItem { get; private set; }
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

    public void EquipItem(IInventoryItem item)
    {
        EquippedItem = item;
    }
}

