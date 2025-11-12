using System.Collections.Generic;

// Under construction
public class PlayerInventoryModel
{
    List<IInventoryItem> items;
}

public interface IInventoryItem
{

}

public interface IUsableItem
{
    void Use();
}

public interface IHoldableItem
{
    void Hold();
}