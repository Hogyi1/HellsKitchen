using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base abstract class for managing the visual state (selection, visibility, opacity) of an object in Unity.
/// Inherits from MonoBehaviour and implements ISelectable, providing core functionality for visual interactions.
/// </summary>
public abstract class BaseVisual : MonoBehaviour, ISelectable
{
    /// <summary>
    /// Abstract method called when the visual representation of the object should indicate deselection.
    /// </summary>
    public abstract void OnDeselect();

    /// <summary>
    /// Abstract method called when the visual representation of the object should indicate selection.
    /// </summary>
    public abstract void OnSelect();

    /// <summary>
    /// Abstract method called to make the visual representation of the object appear.
    /// </summary>
    public abstract void Show();

    /// <summary>
    /// Abstract method called to make the visual representation of the object disappear.
    /// </summary>
    public abstract void Hide();

    /// <summary>
    /// GameObject used to display visual feedback when the object is selected.
    /// This is typically an overlay or an indicator.
    /// </summary>
    [SerializeField] protected GameObject selectedVisual;

    /// <summary>
    /// Indicates whether the object is currently selected.
    /// </summary>
    protected bool isSelected = false;

    /// <summary>
    /// Indicates whether the object is capable of being selected.
    /// </summary>
    protected bool isSelectable = false;

    /// <summary>
    /// Indicates whether the object's visual representation is currently visible.
    /// </summary>
    protected bool isVisible = false;

    /// <summary>
    /// The current alpha (opacity) value applied to all controlled materials.
    /// </summary>
    protected float currentOpacity;

    /// <summary>
    /// A list of all materials found within the 'selectedVisual' GameObject and its children.
    /// These materials' alpha values are controlled by this class.
    /// </summary>
    protected List<Material> allMaterials = new();

    /// <summary>
    /// Initializes the visual state of the object.
    /// Deactivates the selected visual, collects all materials from it, and sets their initial alpha to 0.
    /// </summary>
    private void Start()
    {
        selectedVisual.SetActive(false);

        Renderer[] renderers = selectedVisual.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            var materials = renderer.materials;
            allMaterials.AddRange(materials);
        }

        SetAllMaterialsAlpha(0f);
    }

    /// <summary>
    /// Sets the alpha (opacity) for all materials collected in 'allMaterials'.
    /// </summary>
    /// <param name="alpha">The target alpha value (0.0 to 1.0).</param>
    protected void SetAllMaterialsAlpha(float alpha)
    {
        foreach (var mat in allMaterials)
        {
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
        }
        currentOpacity = alpha;
    }
}
