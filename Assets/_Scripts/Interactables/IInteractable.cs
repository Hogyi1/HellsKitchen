using UnityEngine;

public interface IInteractable
{
    void OnInteract(Interactor context);

    InteractionResult CanInteract(Interactor context);
}
public readonly struct InteractionResult
{
    public readonly bool success;
    public readonly string message;

    public InteractionResult(bool success, string message) => (this.success, this.message) = (success, message);
}