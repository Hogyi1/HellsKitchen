using UnityEngine;

// Under construction
public class Interactor : MonoBehaviour
{
    [SerializeField] InputHandler input;
    RaycastSensor interactionSensor;

    bool CanInteract() => true; // Player has something in hand etc

    private void Awake()
    {
        interactionSensor = GetComponent<RaycastSensor>();
        interactionSensor.SetRaycastDirection(RaycastSensor.RaycastDirection.Forward);

        // input.Interact += OnInteractPressed;
    }

    public void OnInteractPressed()
    {
        interactionSensor.Cast();
        if (interactionSensor.HasDetectedHit() && CanInteract())
        {
            IInteractable interactable;
            interactionSensor.GetTransform().TryGetComponent<IInteractable>(out interactable);
            if (interactable != null)
            {
                interactable.OnInteract();
            }
        }
    }

    private void Update()
    {
        // Set visuals for closeby or selected interactables
    }

    private void OnEnable() => input.Interact += OnInteractPressed;
    private void OnDisable() => input.Interact -= OnInteractPressed;
}
