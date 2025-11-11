using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerMovement", menuName = "Game/Player/Settings/MovementSettings")]
public class PlayerMovementSettings : ScriptableObject
{
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

    [Header("Jump settings")]
    public float coyoteTime = 0.2f;
    public float initialJumpVelocity;
    public float maxJumpHeight = 1f;
    public float maxJumpTime = 0.5f;

    [Header("Look settings")]
    public float rotationSmoothTime = 0.05f;
}