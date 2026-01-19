using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class PlayerController : Singleton<PlayerController>, IObjectParent<IHoldableItem>
{
    [SerializeField] Interactor interactor;
    [SerializeField] InputHandler input;
    [SerializeField] Transform handTransform;
    [SerializeField] PlayerMovementController movementController;
    public AudioSO pickupSound;

    PlayerModel playerModel;
    public bool hasChild;
    CountDownTimer interactionTimer;

    public float interactionCooldown = 0.2f;

    public override void BaseAwake()
    {
        interactor = interactor != null ? interactor : GetComponentInChildren<Interactor>();
        playerModel = new PlayerModel();
        interactionTimer = new(interactionCooldown);
    }

    private void OnEnable() => input.Interact += OnInteractionPressed;

    private void OnDisable() => input.Interact -= OnInteractionPressed;
    
    public void DisableMovement()
    {
        input.Interact -= OnInteractionPressed;
        input.SwitchToUI();
    }

    void OnInteractionPressed()
    {
        var interactable = interactor.GetInteractable();
        if (interactable == null || interactionTimer.IsRunning) return;

        interactionTimer.Start();
        var ir = interactable.TryInteract(this);
        Debug.Log(ir.message);
    }

    public void SetChild(IHoldableItem child)
    {
        if (child != null)
            AudioManager.Instance.PlaySFX(pickupSound, transform.position);
        playerModel.Pickup(child);
    }

    public bool HasChild() => playerModel.HeldItem != null;

    public KitchenObjectController TryGetKitchenObject()
    {
        if (playerModel.HeldItem is KitchenObjectController kitchenObject)
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

    public void ClearChild()
    {
        SetChild(null);
    }

    public Transform GetTransform() => handTransform;
}
