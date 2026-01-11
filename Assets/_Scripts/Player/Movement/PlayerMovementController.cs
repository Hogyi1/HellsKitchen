using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[DefaultExecutionOrder(-1)]
[RequireComponent(typeof(PlayerCameraController), typeof(PlayerMover))]
public class PlayerMovementController : MonoBehaviour
{
    #region Variables
    [SerializeField] InputHandler input;
    [SerializeField] PlayerMover mover;
    [SerializeField] PlayerCameraController cameraController;
    [SerializeField] Transform cameraTransform;
    [SerializeField] PlayerMovementSettings settings;

    StateMachine stateMachine;
    CountDownTimer coyoteTimer;

    // Local
    Vector3 velocity, savedVelocity;
    Vector2 movement;
    bool isJumpPressed, isJumping, isCrouching, isSprintPressed, isSprinting;
    float smoothVelocity, movementSpeed;
    public float smoothing = 15f;

    public float verticalVelocity; // Cannot handle reference
    #endregion

    #region Unity methods
    private void Awake()
    {
        stateMachine = new StateMachine();
        coyoteTimer = new CountDownTimer(settings.coyoteTime);
        mover = mover != null ? mover : GetComponentInChildren<PlayerMover>();
        cameraController = cameraController != null ? cameraController : GetComponentInChildren<PlayerCameraController>();
        settings = settings != null ? settings : ScriptableObject.CreateInstance<PlayerMovementSettings>();

        SetupJumpVariables();
        SetupStateMachine();
    }

    private void OnEnable()
    {
        input.Sprint += SprintPressed;
        input.Jump += JumpPressed;
    }

    private void OnDisable()
    {
        input.Sprint -= SprintPressed;
        input.Jump -= JumpPressed;
    }

    private void Update()
    {
        movement = input.Movement;
        movementSpeed = CalculateMovementSpeed();

        stateMachine.Update();

        HandleLateralMovement();
        HandleGravity();
        HandleJump();

        if (IsDebugModeOn)
            UpdateDebugBools();
    }

    private void LateUpdate()
    {
        HandleRotation();

        if (input.IsCrouchPressed && !isCrouching)
            EnterCrouch();
        else if (!input.IsCrouchPressed && !mover.CeilingDetected() && isCrouching)
            TryExitCrouch();

        bool currentlySprinting = IsSprinting();
        if (currentlySprinting != isSprinting)
        {
            cameraController.ChangeFOV(currentlySprinting);
            isSprinting = currentlySprinting;
        }
    }
    #endregion

    #region Setup and helper methods
    /// <summary>
    /// Sets up the statemachine with the transitions, instantiates the states
    /// </summary>
    void SetupStateMachine()
    {
        var grounded = new GroundedState(this);
        var falling = new FallingState(this, ref coyoteTimer);
        var sliding = new SlidingState(this);
        var rising = new RisingState(this);
        var jumping = new JumpingState(this);

        At(grounded, rising, () => IsRising());
        At(grounded, sliding, () => IsGrounded() && IsGroundTooSteep());
        At(grounded, falling, () => !IsGrounded());
        At(grounded, jumping, () => isJumpPressed && !isJumping);

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
    /// <param name="to"></param>
    /// <param name="condition"></param>
    void Any(IState to, Func<bool> condition) => stateMachine.AddMainTransition(to, condition);

    /// <summary>
    /// Look at https://www.youtube.com/watch?v=hG9SzQxaCm8 for further info
    /// </summary>
    void SetupJumpVariables()
    {
        float timeToApex = settings.maxJumpTime / 2;
        settings.initialJumpVelocity = (2 * settings.maxJumpHeight) / timeToApex;
        settings.gravity = (-2 * settings.maxJumpHeight) / (timeToApex * timeToApex);

        coyoteTimer ??= new CountDownTimer(settings.coyoteTime);
    }

    private void EnterCrouch()
    {
        isCrouching = true;

        mover.Crouch(isCrouching, settings.timeToCrouch);
        cameraController.ChangeVignette(isCrouching, settings.timeToCrouch);
    }

    private void TryExitCrouch()
    {
        if (mover.CeilingDetected())
            return;

        isCrouching = false;

        mover.Crouch(isCrouching, settings.timeToCrouch);
        cameraController.ChangeVignette(isCrouching, settings.timeToCrouch);
    }
    #endregion

    #region Movement calculations

    /// <summary>
    /// Calculates the movement speed based on the current input and character state.
    /// </summary>
    /// <remarks>The movement speed is determined by whether the sprint input is pressed, the movement
    /// direction, and whether the character is crouching. Sprinting is only applied when moving forward.</remarks>
    /// <returns>The movement speed as a floating-point value. Returns the running speed if sprinting, the crouch speed if
    /// crouching, or the walking speed otherwise.</returns>
    float CalculateMovementSpeed() => (isSprintPressed && movement.y > 0.1f && !isCrouching) ? settings.runningSpeed : isCrouching ? settings.crouchSpeed : settings.walkingSpeed;

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
        var direction = // !isFirstPerson
                        // ? new Vector3(movement.x, 0f, movement.y) :
            Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up) * movement.y +
            Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up) * movement.x;

        return direction = direction.magnitude > 1f ? direction.normalized : direction;
    }

    /// <summary>
    /// Handle the rotation of the character. Smoothes out to prevent camera jitter
    /// </summary>
    void HandleRotation()
    {
        float target;

        target = cameraController.cameraRotation.x;
        float smoothed = Mathf.SmoothDampAngle(
            transform.eulerAngles.y,
            target,
            ref smoothVelocity,
            settings.rotationSmoothTime);

        transform.rotation = Quaternion.Euler(0f, smoothed, 0f);


        //Vector3 moveDir = CalculateMovementDirection();

        //if (moveDir.sqrMagnitude > 0.01f)
        //{
        //    Vector3 movedir = new Vector3(movement.x, 0f, movement.y).normalized;
        //    transform.forward = Vector3.Lerp(transform.forward, moveDir, Time.deltaTime * smoothing);
        //}
        //else
        //    target = transform.eulerAngles.y;
    }

    /// <summary>
    /// Handles lateral movement as the name suggests, calculates the targeted velocity and 
    /// uses acceleration/deceleration accordingly
    /// </summary>
    void HandleLateralMovement()
    {
        Vector3 targetVelocity = CalculateTargetVelocity();

        float acceleration = targetVelocity.sqrMagnitude > 0.01f ? settings.moveAcceleration : settings.moveDeceleration;

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
            verticalVelocity = settings._antiBump;
            return;
        }

        // More downward force if we are falling
        float force = IsFalling() || !isJumpPressed
            ? settings.gravity * settings.gravityMultiplier
            : settings.gravity;

        verticalVelocity += force * Time.deltaTime;
    }

    /// <summary>
    /// Handles jump presses nothing extra
    /// </summary>
    void HandleJump()
    {
        if ((coyoteTimer.IsRunning || IsGrounded()) && isJumpPressed && !isJumping)
        {
            isJumping = true;
            verticalVelocity = settings.initialJumpVelocity;
            coyoteTimer.Stop();
        }
        else if (IsGrounded() && !isJumpPressed && isJumping)
            isJumping = false;
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
        isJumpingDebug = isJumping;
        isCoyoteActive = coyoteTimer.IsRunning;
        isCoyoteFinished = coyoteTimer.IsFinished;
        ceilingHit = mover.CeilingDetected();
    }
    #endregion

    #region Getters and Setters
    bool IsGrounded() => mover.IsGrounded();
    bool IsRising() => verticalVelocity > 0f;
    bool IsFalling() => (verticalVelocity <= 0f && !IsGrounded()) || stateMachine.CurrentState is FallingState;
    bool IsGroundTooSteep() => mover.IsGroundTooSteep();
    bool IsSprinting() => isSprintPressed && movement.y > 0.1f && !isCrouching;
    void JumpPressed(bool pressed) => isJumpPressed = pressed;
    void SprintPressed(bool pressed) => isSprintPressed = pressed;
    #endregion

    /// <summary>
    /// Physics based acceleration calculation - deprecated
    /// </summary>
    void HandleLateralMovement_Physicsbased()
    {
        Vector3 inputDir = CalculateMovementDirection();

        if (inputDir.sqrMagnitude > 0.01f)
            velocity += inputDir * settings.moveAcceleration * Time.deltaTime;
        else velocity -= velocity.normalized * settings.moveDeceleration * Time.deltaTime;

        Vector3 lateralVelocity = velocity - transform.up * Vector3.Dot(velocity, transform.up);

        velocity.y += verticalVelocity;
        velocity = Vector3.ClampMagnitude(new Vector3(velocity.x, 0f, velocity.z), movementSpeed);

        mover.Move(velocity * Time.deltaTime);
        savedVelocity = velocity;
    }
}