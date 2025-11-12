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
        interactionSensor = GetComponent<RaycastSensor>();
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
        lastInteractable = hoveredObject.GetComponentInChildren<IInteractable>();
    }

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

    Transform GetRaycastResult()
    {
        interactionSensor.Cast();
        return interactionSensor.GetTransform();
    }

    public void SetInteractionLength(float length)
    {
        interactionLength = length;
        interactionSensor.SetRaycastLength(interactionLength);
    }
}
