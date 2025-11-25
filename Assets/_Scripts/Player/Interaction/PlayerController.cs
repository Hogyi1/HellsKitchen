using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class PlayerController : MonoBehaviour, IObjectParent<IHoldableItem>
{
    [SerializeField] Interactor interactor;
    [SerializeField] InputHandler input;
    [SerializeField] Transform handTransform;

    PlayerModel playerModel;
    public bool hasChild;
    CountDownTimer interactionTimer;

    public float interactionCooldown = 0.2f;

    private void Awake()
    {
        interactor = interactor != null ? interactor : GetComponentInChildren<Interactor>();
        playerModel = new PlayerModel();
        interactionTimer = new(interactionCooldown);
    }

    private void OnEnable() => input.Interact += OnInteractionPressed;

    private void OnDisable() => input.Interact -= OnInteractionPressed;

    void OnInteractionPressed()
    {
        var interactable = interactor.GetInteractable();
        if (interactable == null || interactionTimer.IsRunning) return;

        interactionTimer.Start();
        var ir = interactable.TryInteract(this);
        Debug.Log(ir.message);
    }

    public Transform GetParentPosition() => handTransform;

    public void SetChild(IHoldableItem child)
    {
        playerModel.Pickup(child);
    }

    public bool HasChild() => playerModel.HeldItem != null;

    public KitchenObject TryGetKitchenObject()
    {
        if (playerModel.HeldItem is KitchenObject kitchenObject)
            return kitchenObject;
        return null;
    }

    public void ClearHeldItem() => playerModel.Pickup(null);

    public IHoldableItem GetChild() => playerModel.HeldItem;

    private void Update()
    {
        hasChild = HasChild();
    }

    public void SetChild(IObjectChild child) => SetChild((IHoldableItem)child);
    IObjectChild IObjectParent.GetChild() => GetChild();

}
