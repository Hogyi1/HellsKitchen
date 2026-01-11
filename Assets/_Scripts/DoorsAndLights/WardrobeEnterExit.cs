using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Wardrobe that smoothly moves the player in/out instead of teleporting,
/// exposes UnityEvents for enter/exit/found, and provides public methods
/// an AI can call to open the door and "catch" the player.
/// Drop this on your wardrobe root (make sure "door" pivot is at hinge).
/// </summary>
public class Wardrobe_Movable_Catchable : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform door; // pivot at hinge
    [SerializeField] private Transform enterPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform playerRoot; // player (root transform)
    [Tooltip("If you have a movement script you want disabled while the wardrobe moves the player, drag it here.")]
    [SerializeField] private MonoBehaviour playerMovementScript;

    [Header("Door Rotation")]
    [SerializeField] private Vector3 closedEuler = new Vector3(0f, 0f, 0f);
    [SerializeField] private Vector3 openEuler = new Vector3(0f, 110f, 0f);
    [SerializeField] private float doorAnimTime = 0.35f;

    [Header("Movement")]
    [SerializeField] private float playerMoveTime = 0.6f; // how long to move player in/out
    [SerializeField] private float interactDistance = 4f;
    [SerializeField] private LayerMask interactMask = ~0;

    [Header("Events")]
    public UnityEvent OnEnter;       // invoked when the player finishes entering
    public UnityEvent OnExit;        // invoked when the player finishes exiting
    public UnityEvent OnPlayerFound; // invoked when an enemy opens the wardrobe and finds the player

    // state
    private bool isOpen;
    private bool isInside;
    private bool busy;

    private Camera cam;

    // cached components on playerRoot (optional)
    private CharacterController cachedController;
    private Rigidbody cachedRigidbody;

    private void Awake()
    {
        cam = Camera.main;
        if (door != null)
            door.localRotation = Quaternion.Euler(closedEuler);

        if (playerRoot != null)
        {
            cachedController = playerRoot.GetComponent<CharacterController>();
            cachedRigidbody = playerRoot.GetComponent<Rigidbody>();
        }
    }

    private void Update()
    {
        if (busy) return;

        // single key (E) for both enter and exit
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteractWithE();
        }
    }

    private void TryInteractWithE()
    {
        if (cam == null) return;

        // Simple check: player must be near enough and looking at wardrobe center
        // Raycast from center of screen to see if wardrobe is under the crosshair
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (!Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask)) return;
        if (hit.transform != transform && !hit.transform.IsChildOf(transform)) return;

        if (!isInside)
        {
            // if closed, open then move player in; if already open just move in
            StartCoroutine(EnterSequence());
        }
        else
        {
            StartCoroutine(ExitSequence());
        }
    }

    private IEnumerator EnterSequence()
    {
        // open if closed
        if (!isOpen)
            yield return AnimateDoor(true);

        yield return MovePlayerTo(enterPoint);
        isInside = true;
        OnEnter?.Invoke();
    }

    private IEnumerator ExitSequence()
    {
        yield return MovePlayerTo(exitPoint);
        isInside = false;
        OnExit?.Invoke();

        // close door after player exits
        yield return AnimateDoor(false);
    }

    /// <summary>
    /// Smoothly moves/rotates the playerRoot to the target transform over playerMoveTime.
    /// Properly disables/enables CharacterController or Rigidbody if present, and disables the
    /// provided movement script while moving.
    /// </summary>
    private IEnumerator MovePlayerTo(Transform target)
    {
        if (target == null || playerRoot == null)
            yield break;

        busy = true;

        // disable movement script
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // handle physics components
        bool hadController = false;
        bool hadRigidbody = false;
        if (cachedController != null)
        {
            hadController = true;
            cachedController.enabled = false;
        }
        if (cachedRigidbody != null)
        {
            hadRigidbody = true;
            cachedRigidbody.isKinematic = true;
        }

        Vector3 startPos = playerRoot.position;
        Quaternion startRot = playerRoot.rotation;
        Vector3 targetPos = target.position;
        Quaternion targetRot = target.rotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, playerMoveTime);
            playerRoot.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, t));
            playerRoot.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        // ensure final
        playerRoot.position = targetPos;
        playerRoot.rotation = targetRot;

        // restore physics/movement
        if (hadController)
            cachedController.enabled = true;
        if (hadRigidbody)
            cachedRigidbody.isKinematic = false;
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;

        busy = false;
    }

    /// <summary>
    /// Door animation coroutine (Slerp between rotations). Public so AI can call it.
    /// </summary>
    public IEnumerator AnimateDoor(bool open)
    {
        if (door == null)
            yield break;

        busy = true; // prevent concurrent interactions

        Quaternion from = door.localRotation;
        Quaternion to = Quaternion.Euler(open ? openEuler : closedEuler);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.0001f, doorAnimTime);
            door.localRotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }
        door.localRotation = to;
        isOpen = open;

        busy = false;
    }

    // ------------------------
    // Methods an enemy/AI can call
    // ------------------------

    /// <summary>
    /// The enemy calls this to open the wardrobe (and attempt to catch the player).
    /// If the player is inside when the door fully opens, the OnPlayerFound event is invoked.
    /// </summary>
    public void EnemyTryOpenAndCatch()
    {
        // If already busy, skip or force-stop depending on desired behavior
        StartCoroutine(EnemyOpenRoutine());
    }

    private IEnumerator EnemyOpenRoutine()
    {
        // Open door
        yield return AnimateDoor(true);

        // If player is inside when door finished opening, we've been found
        if (isInside)
        {
            OnPlayerFound?.Invoke();
            // Optional: you might want to automatically force the player out or trigger other consequences
            // Example: StartCoroutine(ExitSequence()); // uncomment to auto-evict the player
        }
    }

    // Optional helpers
    public bool IsPlayerInside => isInside;
    public bool IsDoorOpen => isOpen;
}
