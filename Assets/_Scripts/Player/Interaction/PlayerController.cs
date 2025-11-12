using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Interactor interactor;
    [SerializeField] InputHandler input;

    PlayerModel playerModel;

    private void Awake()
    {
        interactor = GetComponentInChildren<Interactor>();
        playerModel = new PlayerModel();
    }

    private void OnEnable() => input.Interact += OnInteractionPressed;

    private void OnDisable() => input.Interact -= OnInteractionPressed;

    void OnInteractionPressed()
    {
        var interactable = interactor.GetInteractable();
        if (interactable == null) return;

        var ir = interactable.CanInteract(playerModel);
        if (!ir.success)
        {
            Debug.Log(ir.message); // Tell the UIManager or something
            return;
        }

        interactable.OnInteract(playerModel); // Lets interact
    }
}
