using UnityEngine;
using UnityEngine.UIElements;

// Under construction
public class Interactor : MonoBehaviour
{
    [SerializeField] float interactionLength = 2f;
    [SerializeField] RaycastSensor interactionSensor;
    [SerializeField] LayerMask interactionMask;

    private void Awake()
    {
        interactionSensor = interactionSensor != null ? interactionSensor : gameObject.AddComponent<RaycastSensor>();
        interactionSensor.SetRaycastLength(interactionLength);
        interactionSensor.SetLayerMask(interactionMask);
        interactionSensor.SetRaycastDirection(RaycastSensor.RaycastDirection.Forward);
    }

    public IInteractable GetInteractable() => lastInteractable;

    IInteractable lastInteractable = null;
    ISelectable lastSelectable = null;

    private void Update()
    {
        var hoveredObject = GetRaycastResult();
        HandleHoveredObject(hoveredObject);
        lastInteractable = hoveredObject != null ? hoveredObject.GetComponentInChildren<IInteractable>() : null;
        interactionSensor.DrawDebug();
    }

    /// <summary>
    /// Handles the hovered object by checking if it's selectable and updating the selection state.
    /// </summary>
    /// <param name="hoveredObject"></param>
    void HandleHoveredObject(Transform hoveredObject)
    {
        if (hoveredObject != null)
        {
            var selectable = hoveredObject.GetComponentInChildren<ISelectable>();
            if (selectable != null && selectable != lastSelectable)
            {
                lastSelectable?.OnDeselect();
                selectable.OnSelect();
                lastSelectable = selectable;
            }
            else
            {
                if (selectable == null)
                {
                    lastSelectable?.OnDeselect();
                    lastSelectable = null;
                }
            }
        }
        else
        {
            lastSelectable?.OnDeselect();
            lastSelectable = null;
        }
    }

    /// <summary>
    /// Gets the transform of the object being interacted with.
    /// </summary>
    /// <returns></returns>
    Transform GetRaycastResult()
    {
        interactionSensor.Cast();
        return interactionSensor.GetTransform();
    }

    /// <summary>
    /// Sets the interaction length for the interactor.
    /// </summary>
    /// <param name="length"></param>
    public void SetInteractionLength(float length)
    {
        interactionLength = length;
        interactionSensor.SetRaycastLength(interactionLength);
    }
}
