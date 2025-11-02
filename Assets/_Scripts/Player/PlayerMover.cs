using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMover : MonoBehaviour
{
    #region Variables
    [SerializeField] private SpherecastSensor ceilingDetector;
    [SerializeField] private SpherecastSensor groundSensor;
    [SerializeField] CharacterController characterController;

    public float slopeLimit; // Ezt lehet itt fogom megvalósítani

    public bool useExtendedRange = false;
    public float baseRange = 0.5f;
    public float extendedRange = 1.5f;

    public float crouchingHeight = 0.5f;
    Vector3 crouchingCenter = new Vector3(0, 0.25f, 0);

    public float standingHeight = 2f;
    Vector3 standingCenter;

    float radiusOffset = 0.05f;
    int currentLayer; // Hit layer
    Coroutine currentRoutine;

    bool isGrounded;
    bool ceilingHit;
    Vector3 groundNormal;
    #endregion

    #region Unity methods
    private void Awake()
    {
        characterController = characterController != null ? characterController : GetComponentInChildren<CharacterController>();

        SetupVariables();
        CalculateSensorLayerMask();
        SetupSensors(false);
    }

    private void Update()
    {
        CheckForGround();
        CheckForCeiling();

        groundSensor.DrawDebug();
        // ceilingDetector.DrawDebug();
    }
    #endregion

    #region Getters
    public bool IsGrounded() => isGrounded;
    public bool CeilingDetected() => ceilingHit;
    public Vector3 GetGroundNormal() => groundNormal;
    public bool IsGroundTooSteep() => Vector3.Angle(GetGroundNormal(), transform.up) > slopeLimit;
    #endregion

    #region Setups/Setters
    /// <summary>
    /// "Creates" a layermask for everything except 'Ignore Layermask' and myself
    /// </summary>
    void CalculateSensorLayerMask()
    {
        int objectLayer = gameObject.layer;
        int layerMask = Physics.AllLayers;

        for (int i = 0; i < 32; i++)
        {
            if (Physics.GetIgnoreLayerCollision(objectLayer, i))
            {
                layerMask &= ~(1 << i);
            }
        }

        int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        layerMask &= ~(1 << ignoreRaycastLayer);

        ceilingDetector.SetLayerMask(layerMask);
        groundSensor.SetLayerMask(layerMask);
        currentLayer = objectLayer;
    }

    /// <summary>
    /// Initializes the <see cref="groundSensor"/> and <see cref="ceilingDetector"/> direction, and cast variables.
    /// </summary>
    void SetupSensors(bool isCrouching)
    {
        groundSensor.SetRaycastDirection(Sensor.RaycastDirection.Down);
        ceilingDetector.SetRaycastDirection(Sensor.RaycastDirection.Up);

        float radius = characterController.radius - radiusOffset;

        groundSensor.SetSphereRadius(radius);
        ceilingDetector.SetSphereRadius(radius);

        Vector3 center = isCrouching ? crouchingCenter : standingCenter;
        float height = isCrouching ? crouchingHeight : standingHeight;
        float range = useExtendedRange ? extendedRange : baseRange;

        Vector3 origin = characterController.transform.position + center;
        float distance = height * 0.5f + range;

        groundSensor.SetCastOrigin(origin);
        ceilingDetector.SetCastOrigin(origin);
        ceilingDetector.SetRaycastLength(distance + (standingHeight - crouchingHeight));
        groundSensor.SetRaycastLength(distance);
    }

    /// <summary>
    /// Sets up variables whoaw
    /// </summary>
    void SetupVariables()
    {
        standingHeight = characterController.height;
        slopeLimit = characterController.slopeLimit;
        currentRoutine = null;

        crouchingCenter.y = CalculateCenter(crouchingHeight, characterController.radius);
        standingCenter.y = CalculateCenter(standingHeight, characterController.radius);
    }

    float CalculateCenter(float height, float radius) => 1f - (Mathf.Max(radius * 2, height) * 0.5f);
    #endregion

    #region Collision checking
    /// <summary>
    /// Checks for ground with the ground sensor
    /// </summary>
    public void CheckForGround()
    {
        if (gameObject.layer != currentLayer)
        {
            CalculateSensorLayerMask();
        }

        groundSensor.Cast();

        bool grounded = false; // Must be a better way to calculate it but im too lazy
        if (groundSensor.HasDetectedHit())
        {
            float distance = groundSensor.GetDistance();
            float halfHeight = characterController.height * 0.5f;

            float sphereR = characterController.radius - radiusOffset;

            float threshold = halfHeight - sphereR;
            grounded = distance <= (threshold + 0.02f); // Margin of error
        }

        isGrounded = grounded;
        groundNormal = groundSensor.GetNormal();
    }

    /// <summary>
    /// Checks for ceilings with the ceiling detector
    /// </summary>
    public void CheckForCeiling()
    {
        if (gameObject.layer != currentLayer)
        {
            CalculateSensorLayerMask();
        }

        ceilingDetector.Cast();

        bool ceiling = false; // Must be a better way to calculate it but im too lazy
        if (ceilingDetector.HasDetectedHit())
        {
            float distance = ceilingDetector.GetDistance();
            float halfheight = characterController.height * 0.5f;
            float diff = standingHeight - characterController.height;
            float sphereR = characterController.radius - radiusOffset;

            float threshold = halfheight + diff - sphereR;
            ceiling = distance <= threshold + 0.02f; // Margin of error
            Debug.Log(distance + " " + threshold);
        }

        ceilingHit = ceiling;
    }
    #endregion

    #region Movement
    /// <summary>
    /// Moves the player using the <see cref="CharacterController.Move"/> method.
    /// The amount should already be scaled by Time.deltaTime outside this method.
    /// </summary>
    /// <param name="amount">The movement delta to apply this frame.</param>
    public void Move(Vector3 amount) => characterController.Move(amount);

    /// <summary>
    /// Smoothly transitions the player's collider height and center 
    /// between standing and crouching states over the given duration.
    /// </summary>
    /// <param name="isCrouching">Whether the player should crouch or stand.</param>
    /// <param name="timeToCrouch">The time (in seconds) over which to blend the transition.</param>
    public void Crouch(bool isCrouching, float timeToCrouch)
    {
        SetupSensors(isCrouching);

        var targetHeight = isCrouching ? crouchingHeight : standingHeight;
        var targetCenter = isCrouching ? crouchingCenter : standingCenter;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ChangeHeightRoutine(targetHeight, targetCenter, timeToCrouch));
    }

    /// <summary>
    /// Coroutine that interpolates the character controller's height and center 
    /// over time to achieve a smooth crouch or stand transition.
    /// </summary>
    /// <param name="targetHeight">Final height of the collider.</param>
    /// <param name="targetCenter">Final center of the collider.</param>
    /// <param name="duration">Transition duration in seconds.</param>
    IEnumerator ChangeHeightRoutine(float targetHeight, Vector3 targetCenter, float duration)
    {
        float time = 0;

        while (time <= duration)
        {
            time += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, time / duration);

            characterController.height = Mathf.Lerp(characterController.height, targetHeight, progress);
            characterController.center = Vector3.Lerp(characterController.center, targetCenter, progress);

            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;
        currentRoutine = null;
    }
    #endregion
}
