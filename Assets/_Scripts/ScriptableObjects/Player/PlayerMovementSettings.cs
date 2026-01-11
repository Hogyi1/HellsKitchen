using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerMovement", menuName = "Game/Player/Settings/MovementSettings")]
public class PlayerMovementSettings : ScriptableObject
{
    [Header("Movement settings")]
    [Range(0f, 15f)] public float runningSpeed = 8.5f;
    [Range(0f, 15f)] public float walkingSpeed = 5.5f;
    [Range(0f, 15f)] public float crouchSpeed = 3.5f;
    [Range(0.01f, 2f)] public float timeToCrouch = 0.25f;

    [Range(20f, 150f)] public float moveAcceleration = 50f;
    [Range(20f, 150f)] public float moveDeceleration = 70f;

    [Header("Gravity settings")]
    [HideInInspector] public float gravity = 50f;
    [Range(1f, 10f)] public float gravityMultiplier = 1.5f;
    [Range(-1.5f, 1.5f)] public float _antiBump = -0.5f;

    [Header("Jump settings")]
    [Range(0.01f, 1.5f)] public float coyoteTime = 0.2f;
    [HideInInspector] public float initialJumpVelocity;
    [Range(0.01f, 5.5f)] public float maxJumpHeight = 1f;
    [Range(0.01f, 5.5f)] public float maxJumpTime = 0.5f;

    [Header("Look settings")]
    [Range(0.01f, 1f)] public float rotationSmoothTime = 0.05f;
}