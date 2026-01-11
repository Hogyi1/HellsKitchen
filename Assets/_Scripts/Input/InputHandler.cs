using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

[DefaultExecutionOrder(-3)]
[CreateAssetMenu(fileName = "NewInputHandler", menuName = "Game/Input")]
public class InputHandler : ScriptableObject, IFirstPersonActions, IThirdPersonActions, IUIActions, IMinigameActions, IInputReader
{
    #region Properties
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction<Vector2> Look = delegate { };
    public event UnityAction<bool> Jump = delegate { };
    public event UnityAction<bool> Sprint = delegate { };
    public event UnityAction<bool> Crouch = delegate { };
    public event UnityAction Interact = delegate { };

    private PlayerInputActions inputActions;

    public Vector2 Movement => inputActions.FindAction("Move").ReadValue<Vector2>();
    public Vector2 LookDirection => inputActions.FindAction("Look").ReadValue<Vector2>();
    public bool IsJumpPressed => inputActions.FindAction("Jump").IsPressed();
    public bool IsCrouchPressed => inputActions.FindAction("Crouch").IsPressed() || crouchToggled;
    public bool IsSprintPressed => inputActions.FindAction("Sprint").IsPressed();
    public bool IsInteractPressed => inputActions.FindAction("Interact").IsPressed();
    #endregion

    #region Actionmaps
    public void SwitchToUI() => SwitchToMap(ActionMap.UI);
    public void SwitchToFirstPerson() => SwitchToMap(ActionMap.FirstPerson);
    public void SwitchToThirdPerson() => SwitchToMap(ActionMap.ThirdPerson);
    public void EnableActions()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();
            inputActions.FirstPerson.SetCallbacks(this);
            inputActions.ThirdPerson.SetCallbacks(this);
            inputActions.UI.SetCallbacks(this);
            inputActions.Minigame.SetCallbacks(this); // FONTOS így fogja ez a szkript megkapni az eventeket
        }

        SwitchToFirstPerson();
    }

    private void OnEnable() => EnableActions();
    private void OnDisable() => inputActions.Disable();

    public void SwitchToMap(ActionMap map)
    {
        inputActions.Disable();
        Debug.Log("Switching to: " + map);
        switch (map)
        {
            case ActionMap.UI:
                inputActions.UI.Enable();
                Cursor.lockState = CursorLockMode.None;
                break;
            case ActionMap.FirstPerson:
                inputActions.FirstPerson.Enable();
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case ActionMap.ThirdPerson:
                inputActions.ThirdPerson.Enable();
                Cursor.lockState = CursorLockMode.Locked;
                break;
            case ActionMap.Minigame:
                inputActions.Minigame.Enable();
                break;
            default:
                inputActions.Disable();
                break;
        }
    }

    #endregion

    #region Player actions
    public void OnMove(InputAction.CallbackContext context) => Move.Invoke(context.ReadValue<Vector2>());
    public void OnLook(InputAction.CallbackContext context) => Look.Invoke(context.ReadValue<Vector2>());
    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Jump.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Jump.Invoke(false);
                break;
        }
    }

    bool crouchToggled;
    public void OnCrouch(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Crouch.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                crouchToggled = false;
                Crouch.Invoke(false);
                break;
        }
    }
    public void OnCrouchToggle(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            crouchToggled = !crouchToggled;
            Crouch.Invoke(crouchToggled);
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Interact.Invoke();
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Sprint.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                Sprint.Invoke(false);
                break;
        }
    }
    #endregion


    #region UI actions
    public void OnNavigate(InputAction.CallbackContext context) { }

    public void OnSubmit(InputAction.CallbackContext context) { }

    public void OnCancel(InputAction.CallbackContext context) { }

    public void OnPoint(InputAction.CallbackContext context) { }

    public void OnClick(InputAction.CallbackContext context) { }

    public void OnRightClick(InputAction.CallbackContext context) { }

    public void OnMiddleClick(InputAction.CallbackContext context) { }

    public void OnScrollWheel(InputAction.CallbackContext context) { }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }

    #endregion

    #region Example
    // Minigame example
    // Létrehozhatsz külön változókat metódusokat vagy akár Actionoket is, amiket másik szkript elérhet
    // a létrehozott input action interfacet meg implementálnia kell az osztálynak ahhoz, hogy fogadhassa
    // az input eventeket. Ez mindig: "I" + inputAction.name + "Actions" - nevű interfacet jelenti
    // Alább láthatsz egy példát arról, hogy hogyan tudod kezelni az egyes Actionoket
    // ha Vector2 vagy más value based értéket akarsz kiolvasni akkor neked kell kezelned, hogy a megfelelő inputot olvassad ki
    // Ezt te fogod beállítani, hogy milyen értéket adjon az action map fülben

    public event UnityAction Exit = delegate { };
    public event UnityAction<bool> MyInteract = delegate { };

    public void OnExit(InputAction.CallbackContext context)
    {
        Exit.Invoke();
        Debug.Log("Exit pressed");
    }

    public void OnMyInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact pressed");
        switch (context.phase)
        {
            case InputActionPhase.Started:
                MyInteract.Invoke(true);
                break;
            case InputActionPhase.Canceled:
                MyInteract.Invoke(false);
                break;
        }
    }

    public void OnValuebased(InputAction.CallbackContext context)
    {
        var someValue = context.ReadValue<float>();
    }
    #endregion

    public enum ActionMap
    {
        FirstPerson,
        ThirdPerson,
        UI,
        None,
        Minigame
    }
}

public interface IInputReader
{
    Vector2 Movement { get; }
    public void EnableActions();
    public void SwitchToMap(InputHandler.ActionMap map);
}


