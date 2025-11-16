using System.Collections.Generic;

// Under construction
public class PlayerInventoryModel
{
    List<IInventoryItem> items;

    public PlayerInventoryModel()
    {
        items = new List<IInventoryItem>();
    }

    public void Add(IInventoryItem item)
    {
        items.Add(item);
    }

    public void Remove(IInventoryItem item)
    {
        items.Remove(item);
    }
}

public interface IInventoryItem
{

}

public interface IUsableItem
{
    void Use();
}

public interface IHoldableItem : IObjectChild
{
    void Hold();
}
