using UnityEditor.VersionControl;
using UnityEngine;

/// <summary>
/// Defines an object that can be interacted with by a player.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Attempts to interact with the object. The outcome is returned as an InteractionResult.
    /// </summary>
    /// <param name="context">The PlayerController initiating the interaction.</param>
    /// <returns>An InteractionResult indicating success or failure and an optional message.</returns>
    InteractionResult TryInteract(PlayerController context);

    /// <summary>
    /// Checks if the object can currently be interacted with by the given player context.
    /// </summary>
    /// <param name="context">The PlayerController to check interaction capability for.</param>
    /// <returns>True if interaction is possible, false otherwise.</returns>
    bool CanInteract(PlayerController context);
}

/// <summary>
/// Represents the outcome of an interaction attempt, indicating success or failure and an optional message.
/// </summary>
public readonly struct InteractionResult
{
    public readonly bool success;
    public readonly string message;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionResult"/> struct.
    /// </summary>
    /// <param name="success">True if the interaction was successful, false otherwise.</param>
    /// <param name="message">An optional message providing details about the interaction outcome.</param>
    public InteractionResult(bool success, string message) => (this.success, this.message) = (success, message);

    /// <summary>
    /// Creates a successful <see cref="InteractionResult"/> with a given message.
    /// </summary>
    /// <param name="message">The message describing the successful outcome.</param>
    /// <returns>A successful InteractionResult.</returns>
    public static InteractionResult Ok(string message) => new InteractionResult(true, message);

    /// <summary>
    /// Creates a failed <see cref="InteractionResult"/> with a given message.
    /// </summary>
    /// <param name="message">The message describing the failed outcome.</param>
    /// <returns>A failed InteractionResult.</returns>
    public static InteractionResult Fail(string message) => new InteractionResult(false, message);
}

/// <summary>
/// Defines an object that can be selected and deselected, typically for visual feedback.
/// </summary>
public interface ISelectable
{
    /// <summary>
    /// Called when the object becomes selected.
    /// </summary>
    void OnSelect();

    /// <summary>
    /// Called when the object becomes deselected.
    /// </summary>
    void OnDeselect();
}

/// <summary>
/// Defines the contract for an interaction strategy, encapsulating a specific interaction logic.
/// </summary>
public interface IInteractionStrategy
{
    /// <summary>
    /// Executes the interaction strategy with the given player context and interactable.
    /// </summary>
    /// <param name="context">The PlayerController initiating the interaction.</param>
    /// <param name="interactable">The IInteractable object being interacted with.</param>
    /// <returns>An InteractionResult indicating the outcome of the execution.</returns>
    InteractionResult Execute(PlayerController context, IInteractable interactable);

    /// <summary>
    /// Checks if the interaction strategy can be executed with the current player context and interactable.
    /// </summary>
    /// <param name="context">The PlayerController.</param>
    /// <param name="interactable">The IInteractable object.</param>
    /// <returns>True if the strategy can be executed, false otherwise.</returns>
    bool CanExecute(PlayerController context, IInteractable interactable);
}

/// <summary>
/// Provides a base abstract class for ScriptableObject-based interaction strategies.
/// All concrete interaction strategies should inherit from this class to be configurable
/// via the Unity Editor and to share a common type for strategy lists.
/// </summary>
public abstract class InteractionStrategySO : ScriptableObject, IInteractionStrategy
{
    /// <summary>
    /// Executes the interaction strategy.
    /// </summary>
    /// <param name="context">The PlayerController initiating the interaction.</param>
    /// <param name="interactable">The IInteractable object being interacted with.</param>
    /// <returns>An InteractionResult indicating the outcome of the execution.</returns>
    public abstract InteractionResult Execute(PlayerController context, IInteractable interactable);

    /// <summary>
    /// Checks if the interaction strategy can be executed.
    /// </summary>
    /// <param name="context">The PlayerController.</param>
    /// <param name="interactable">The IInteractable object.</param>
    /// <returns>True if the strategy can be executed, false otherwise.</returns>
    public abstract bool CanExecute(PlayerController context, IInteractable interactable);
}