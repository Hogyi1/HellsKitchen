using UnityEngine;

// Under construction
public abstract class BaseTool : IHoldableItem, IUsableItem
{
    public void ClearParent()
    {
        throw new System.NotImplementedException();
    }

    public IObjectParent GetParent()
    {
        throw new System.NotImplementedException();
    }

    public abstract void Hold();

    public void SetParent(IObjectParent parent)
    {
        throw new System.NotImplementedException();
    }

    public void SwapParent(IObjectChild swap)
    {
        throw new System.NotImplementedException();
    }

    public abstract void Use();
}

// hmmmm idk
public class FlashLight : BaseTool, IInventoryItem
{
    private bool isOn = false;
    public override void Hold()
    {
        // Logic for holding the flashlight
        Debug.Log("Flashlight is being held.");
    }
    public override void Use()
    {
        isOn = !isOn;
        if (isOn)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }
    }
    private void TurnOn()
    {
        Debug.Log("Flashlight turned ON.");
        // Additional logic to turn on the flashlight
    }
    private void TurnOff()
    {
        Debug.Log("Flashlight turned OFF.");
        // Additional logic to turn off the flashlight
    }

    public void Equip()
    {

    }
}