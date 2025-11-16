using UnityEditor.VersionControl;
using UnityEngine;

public interface IInteractable
{
    InteractionResult TryInteract(PlayerController context);

    bool CanInteract(PlayerController context);
}
public readonly struct InteractionResult
{
    public readonly bool success;
    public readonly string message;

    public InteractionResult(bool success, string message) => (this.success, this.message) = (success, message);
    public static InteractionResult Ok(string message) => new InteractionResult(true, message);
    public static InteractionResult Fail(string message) => new InteractionResult(false, message);
}

public interface ISelectable
{
    void OnSelect();
    void OnDeselect();
}