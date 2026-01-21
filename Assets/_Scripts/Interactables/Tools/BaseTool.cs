using UnityEngine;

// Under construction
public abstract class BaseTool : IUsableItem
{
    public void ClearParent()
    {
        throw new System.NotImplementedException();
    }

    public IObjectParent GetParent()
    {
        throw new System.NotImplementedException();
    }

    public AudioSO GetPickUpAudio()
    {
        throw new System.NotImplementedException();
    }

    public AudioSO GetPlaceAudio()
    {
        throw new System.NotImplementedException();
    }

    public abstract void Hold();

    public bool IsTwoHanded()
    {
        throw new System.NotImplementedException();
    }

    public UseResult OnUse(PlayerController context)
    {
        throw new System.NotImplementedException();
    }

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