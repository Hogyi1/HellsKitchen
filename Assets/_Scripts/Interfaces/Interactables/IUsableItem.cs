public interface IUsableItem : IHoldableItem
{
    UseResult OnUse(PlayerController context);
}

/// <summary>
/// Represents the outcome of an interaction attempt, indicating success or failure and an optional message.
/// </summary>
public readonly struct UseResult
{
    public readonly bool success;
    public readonly string message;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionResult"/> struct.
    /// </summary>
    /// <param name="success">True if the interaction was successful, false otherwise.</param>
    /// <param name="message">An optional message providing details about the interaction outcome.</param>
    public UseResult(bool success, string message) => (this.success, this.message) = (success, message);

    /// <summary>
    /// Creates a successful <see cref="UseResult"/> with a given message.
    /// </summary>
    /// <param name="message">The message describing the successful outcome.</param>
    /// <returns>A successful InteractionResult.</returns>
    public static UseResult Ok(string message) => new UseResult(true, message);

    /// <summary>
    /// Creates a failed <see cref="InteractionResult"/> with a given message.
    /// </summary>
    /// <param name="message">The message describing the failed outcome.</param>
    /// <returns>A failed InteractionResult.</returns>
    public static UseResult Fail(string message) => new UseResult(false, message);
}