using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Interactor))]
public class PlayerController : MonoBehaviour, IObjectParent<IHoldableItem>
{
    [SerializeField] Interactor _interactor;
    [SerializeField] InputHandler _inputHandler;
    [SerializeField] Transform _handTransform;
    [SerializeField] PlayerMovementController _movementController;

    private PlayerModel _playerModel;
    private CountDownTimer _interactionTimer;
    private CountDownTimer _useTimer;

    public float InteractionCooldown = 0.2f;
    public float UseCooldown = 0.2f;

    private void Awake()
    {
        _interactor = _interactor != null ? _interactor : GetComponentInChildren<Interactor>();
        _handTransform = _handTransform != null ? _handTransform : transform;
        _movementController = _movementController != null ? _movementController : GetComponentInParent<PlayerMovementController>();
    }

    private void Start()
    {
        _playerModel = new PlayerModel();
        _interactionTimer = new(InteractionCooldown);
        _useTimer = new(UseCooldown);
    }

    private void OnEnable()
    {
        _inputHandler.Interact += OnInteractionPressed;
        _inputHandler.Use += OnUsePressed;
    }

    private void OnDisable()
    {
        _inputHandler.Interact -= OnInteractionPressed;
        _inputHandler.Use -= OnUsePressed;
    }

    private void OnInteractionPressed()
    {
        var interactable = _interactor.GetInteractable();
        if (interactable == null || _interactionTimer.IsRunning) return;

        _interactionTimer.Start();
        var ir = interactable.TryInteract(this);
        Debug.Log(ir.message); // Switch to UI feedback later
    }

    private void OnUsePressed()
    {
        var useable = GetChild() as IUsableItem;
        if (useable == null || _useTimer.IsRunning) return;

        _useTimer.Start();
        var ur = useable.OnUse(this);
        Debug.Log(ur.message); // Switch to UI feedback later
    }

    public void SetChild(IHoldableItem child)
    {
        AudioManager.Instance.PlaySFX(child?.GetPickUpAudio(), transform.position);

        _playerModel.Pickup(child);
    }

    public bool HasChild() => _playerModel.HeldItem != null;

    public bool CanPickUpItem(IHoldableItem item)
    {
        if (item == null)
            return false;

        var child = GetChild();
        if (item.IsTwoHanded())
            return child == null;
        else
            return child == null || !child.IsTwoHanded();
    }

    public void ClearHeldItem() => _playerModel.Pickup(null);

    public IHoldableItem GetChild() => _playerModel.HeldItem;

    public void SetChild(IObjectChild child) => SetChild((IHoldableItem)child);
    IObjectChild IObjectParent.GetChild() => GetChild();

    public void ClearChild() => SetChild(null);

    public Transform GetTransform() => _handTransform;
}
