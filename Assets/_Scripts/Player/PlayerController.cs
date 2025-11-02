using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    #region Variables
    [SerializeField] InputHandler input;
    [SerializeField] PlayerMover mover;
    [SerializeField] Transform cameraTransform;

    StateMachine stateMachine;
    CountDownTimer coyoteTimer;

    [Header("Movement settings")]
    public float runningSpeed = 8.5f;
    public float walkingSpeed = 5.5f;
    public float crouchSpeed = 3.5f;
    public float timeToCrouch = 0.25f;

    public float moveAcceleration = 50f;
    public float moveDeceleration = 70f;

    [Header("Gravity settings")]
    public float gravity = 50f;
    public float gravityMultiplier = 1.5f;
    public float _antiBump = -0.5f;

    // Local
    Vector3 velocity, savedVelocity;
    Vector2 movement, lookInput, cameraRotation;
    bool IsJumpPressed, IsJumping, IsCrouching;
    float smoothVelocity, movementSpeed;

    public float verticalVelocity; // Cannot handle reference

    [Header("Jump settings")]
    public float initialJumpVelocity;
    public float maxJumpHeight = 1f;
    public float maxJumpTime = 0.5f;

    // Testing
    [Header("TESTING")]
    [SerializeField] CinemachineCamera firstPerson;
    [SerializeField] CinemachineCamera thirdPerson;
    float smoothing = 15f;
    Vignette vignette;
    public bool isFirstPerson = true;
    public float runningFov = 80f;
    public float normalFov = 60f;
    public float crouchingVignette = 0.25f;

    [Header("Camera settings")]
    public float rotationSmoothTime = 0.05f;
    [Range(0.1f, 10f)] public float lookSensitivity = 0.1f;
    [Range(1f, 90f)] public float upperCameraLimit = 60f;
    [Range(1f, 90f)] public float lowerCameraLimit = 60f;
    #endregion

    #region Unity methods
    private void Awake()
    {
        mover = mover != null ? mover : GetComponentInChildren<PlayerMover>();
        var volumeSettings = firstPerson.GetComponentInChildren<CinemachineVolumeSettings>();
        if (volumeSettings != null)
        {
            volumeSettings.Profile.TryGet(out vignette);
        }

        SetupJumpVariables();

        stateMachine = new StateMachine();
        SetupStateMachine();

        input.Jump += (bool jumping) => { IsJumpPressed = jumping; }; // Change it to method so i can unsubscribe from it
    }

    private void OnValidate() => SetupJumpVariables();

    private void Update()
    {
        movement = input.Movement;
        lookInput = input.LookDirection;

        stateMachine.Update();

        HandleCameraRotation();
        HandleLateralMovement();
        HandleGravity();
        HandleJump();

        if (IsDebugModeOn)
            UpdateDebugBools();
    }

    private void LateUpdate()
    {
        HandleRotation();

        bool crouchPressed = input.IsCrouchPressed;
        bool sprintPressed = input.IsSprintPressed;
        bool ceilingBlocked = mover.CeilingDetected();

        if (crouchPressed && !IsCrouching)
            EnterCrouch();
        else if (!crouchPressed && !ceilingBlocked && IsCrouching)
            TryExitCrouch();


        movementSpeed = (sprintPressed && movement.y > 0.1f && !IsCrouching) ? runningSpeed : IsCrouching ? crouchSpeed : walkingSpeed;
        float targetFov = (sprintPressed && movement.y > 0.1f && !IsCrouching) ? runningFov : normalFov;
        float targetVignette = IsCrouching ? crouchingVignette : 0f; // Move this to cameracontroller

        if (Mathf.Abs(firstPerson.Lens.FieldOfView - targetFov) > 0.01f)
            firstPerson.Lens.FieldOfView = Mathf.Lerp(firstPerson.Lens.FieldOfView, targetFov, Time.deltaTime * smoothing);

        if (Mathf.Abs(vignette.intensity.value - targetVignette) > 0.01f)
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVignette, Time.deltaTime * smoothing);

        if (Input.GetKeyDown(KeyCode.T))
            ToggleCameraMode();
    }

    // Extracted to methods
    private void EnterCrouch()
    {
        IsCrouching = true;

        mover.Crouch(true, timeToCrouch);
    }

    private void TryExitCrouch()
    {
        if (mover.CeilingDetected())
            return;

        IsCrouching = false;

        mover.Crouch(false, timeToCrouch);
    }

#warning TESTING 
    private void ToggleCameraMode() // TESTING 
    {
        CameraController.Instance.RequestFocus(thirdPerson);

        isFirstPerson = !isFirstPerson;

        if (isFirstPerson)
        {
            input.SwitchToFirstPerson();
            CameraController.Instance.RequestFocus(firstPerson);
        }
        else
            input.SwitchToThirdPerson();
    }
    #endregion

    #region Setup and helper methods
    /// <summary>
    /// Sets up the statemachine with the transitions, instantiates the states
    /// </summary>
    void SetupStateMachine()
    {
        stateMachine = new StateMachine();

        var grounded = new GroundedState(this);
        var falling = new FallingState(this, ref coyoteTimer);
        var sliding = new SlidingState(this);
        var rising = new RisingState(this);
        var jumping = new JumpingState(this);

        At(grounded, rising, () => IsRising());
        At(grounded, sliding, () => IsGrounded() && IsGroundTooSteep());
        At(grounded, falling, () => !IsGrounded());
        At(grounded, jumping, () => IsJumpPressed && !IsJumping);

        At(falling, rising, () => IsRising());
        At(falling, grounded, () => IsGrounded() && !IsGroundTooSteep());
        At(falling, sliding, () => IsGrounded() && IsGroundTooSteep());

        At(sliding, rising, () => IsRising());
        At(sliding, falling, () => !IsGrounded());
        At(sliding, grounded, () => IsGrounded() && !IsGroundTooSteep());

        At(rising, grounded, () => IsGrounded() && !IsGroundTooSteep());
        At(rising, sliding, () => IsGrounded() && IsGroundTooSteep());
        At(rising, falling, () => IsFalling());
        At(rising, falling, () => mover.CeilingDetected());

        At(jumping, rising, () => !IsGrounded());
        At(jumping, falling, () => mover.CeilingDetected());

        stateMachine.SetState(falling);
    }

    /// <summary>
    /// Shorter version for the add transition of the statemachine
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="condition"></param>
    void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

    /// <summary>
    /// Shorter version for the main transitions of the statemachine
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="to"></param>
    /// <param name="condition"></param>
    void Any<T>(IState to, Func<bool> condition) => stateMachine.AddMainTransition(to, condition);

    /// <summary>
    /// Look at https://www.youtube.com/watch?v=hG9SzQxaCm8 for further info
    /// </summary>
    void SetupJumpVariables()
    {
        float timeToApex = maxJumpTime / 2;
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
        gravity = (-2 * maxJumpHeight) / (timeToApex * timeToApex);

        coyoteTimer ??= new CountDownTimer(0.2f);
    }
    #endregion

    #region Movement calculations
    /// <summary>
    /// Calculates the maximum velocity that can be reached.
    /// </summary>
    /// <returns>The targeted velocity of player</returns>
    Vector3 CalculateTargetVelocity() => CalculateMovementDirection() * movementSpeed;

    /// <summary>
    /// Calculate the direction based on the input and camera/the player position
    /// </summary>
    /// <returns>The calculated movement direction</returns>
    Vector3 CalculateMovementDirection()
    {
        var direction = !isFirstPerson
            ? new Vector3(movement.x, 0f, movement.y)
            : Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up) * movement.y +
            Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up) * movement.x;

        return direction = direction.magnitude > 1f ? direction.normalized : direction;
    }

    /// <summary>
    /// Handles camera rotation. Rotates camera on the up-down axis only
    /// </summary>
    void HandleCameraRotation()
    {
        cameraRotation.x += lookInput.x * lookSensitivity;
        cameraRotation.y += -lookInput.y * lookSensitivity;

        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -upperCameraLimit, lowerCameraLimit);
        cameraTransform.localRotation = Quaternion.Euler(cameraRotation.y, 0f, 0f);
    }

    /// <summary>
    /// Handle the rotation of the character. Smoothes out to prevent camera jitter
    /// </summary>
    void HandleRotation()
    {
        float target;

        if (isFirstPerson)
        {
            target = cameraRotation.x;
            float smoothed = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                target,
                ref smoothVelocity,
                rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0f, smoothed, 0f);
        }
        else
        {
            Vector3 moveDir = CalculateMovementDirection();

            if (moveDir.sqrMagnitude > 0.01f)
            {
                Vector3 movedir = new Vector3(movement.x, 0f, movement.y).normalized;
                transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * smoothing);
            }
            else
                target = transform.eulerAngles.y;
        }
    }

    /// <summary>
    /// Handles lateral movement as the name suggests, calculates the targeted velocity and 
    /// uses acceleration/deceleration accordingly
    /// </summary>
    void HandleLateralMovement()
    {
        Vector3 targetVelocity = CalculateTargetVelocity();

        float acceleration = targetVelocity.sqrMagnitude > 0.01f ? moveAcceleration : moveDeceleration;

        Vector3 lateralVelocity = velocity - transform.up * Vector3.Dot(velocity, transform.up);
        Vector3 newVelocity = Vector3.MoveTowards(lateralVelocity, targetVelocity, acceleration * Time.deltaTime);

        newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0f, newVelocity.z), movementSpeed);
        newVelocity.y += verticalVelocity;

        velocity = newVelocity;
        mover.Move(velocity * Time.deltaTime);
        savedVelocity = velocity;
    }

    /// <summary>
    /// Applies constant downward force to the player at all time.
    /// </summary>
    void HandleGravity()
    {
        // If we are not in the air apply constant force for the charactercontroller to not bug out
        if (IsGrounded() && verticalVelocity <= 0f)
        {
            verticalVelocity = _antiBump;
            return;
        }

        // More downward force if we are falling
        float force = IsFalling() || !IsJumpPressed
            ? gravity * gravityMultiplier
            : gravity;

        verticalVelocity += force * Time.deltaTime;
    }

    /// <summary>
    /// Handles jump presses nothing extra
    /// </summary>
    void HandleJump()
    {
        if ((coyoteTimer.IsRunning || IsGrounded()) && IsJumpPressed && !IsJumping)
        {
            IsJumping = true;
            verticalVelocity = initialJumpVelocity;
            coyoteTimer.Stop();
        }
        else if (IsGrounded() && !IsJumpPressed && IsJumping)
            IsJumping = false;
    }
    #endregion

    #region Debug
    [Header("Debug States")]
    public bool IsDebugModeOn;
    [Space]
    public String currentState;
    public bool isRising;
    public bool isFalling;
    public bool isReallyGrounded;
    public bool isSliding;
    public bool isJumpingDebug;
    public bool isCoyoteActive;
    public bool isCoyoteFinished;
    public bool ceilingHit;

    void UpdateDebugBools()
    {
        currentState = stateMachine.CurrentState.ToString();
        isRising = IsRising();
        isFalling = IsFalling();
        isReallyGrounded = IsGrounded();
        isSliding = IsGroundTooSteep();
        isJumpingDebug = IsJumping;
        isCoyoteActive = coyoteTimer.IsRunning;
        isCoyoteFinished = coyoteTimer.IsFinished;
        ceilingHit = mover.CeilingDetected();
    }
    #endregion

    bool IsGrounded() => mover.IsGrounded();
    bool IsRising() => verticalVelocity > 0f;
    bool IsFalling() => (verticalVelocity <= 0f && !IsGrounded()) || stateMachine.CurrentState is FallingState;
    bool IsGroundTooSteep() => mover.IsGroundTooSteep();

    /// <summary>
    /// Physics based acceleration calculation - deprecated
    /// </summary>
    void HandleLateralMovement_Physicsbased()
    {
        Vector3 inputDir = CalculateMovementDirection();

        if (inputDir.sqrMagnitude > 0.01f)
            velocity += inputDir * moveAcceleration * Time.deltaTime;
        else velocity -= velocity.normalized * moveDeceleration * Time.deltaTime;

        Vector3 lateralVelocity = velocity - transform.up * Vector3.Dot(velocity, transform.up);

        velocity.y += verticalVelocity;
        velocity = Vector3.ClampMagnitude(new Vector3(velocity.x, 0f, velocity.z), movementSpeed);

        mover.Move(velocity * Time.deltaTime);
        savedVelocity = velocity;
    }
}
