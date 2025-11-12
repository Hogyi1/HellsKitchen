using UnityEngine;

public interface IInteractable
{
    void OnInteract(PlayerModel context);

    InteractionResult CanInteract(PlayerModel context);

}
public readonly struct InteractionResult
{
    public readonly bool success;
    public readonly string message;

    public InteractionResult(bool success, string message) => (this.success, this.message) = (success, message);
}

public interface ISelectable
{
    void OnSelect();
    void OnDeselect();
}