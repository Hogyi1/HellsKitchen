/// <summary>
/// Defines an object that can be selected and deselected, typically for visual feedback.
/// </summary>
public interface ISelectable : IVisible
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
