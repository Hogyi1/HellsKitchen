using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

/// <summary>
/// Manages a world-space popup UI, driven by a WSUISettings ScriptableObject.
/// Handles orientation, scaling, animations, and interaction based on the provided settings.
/// </summary>
public class WorldSpaceUI_New : MonoBehaviour, IUI
{
    [SerializeField] private WSUISettings settings;
    [SerializeField] private Transform cam;

    private const string layerAOT = "AlwaysOnTop";
    UIDocument document;
    VisualElement root;
    VisualTreeAsset visual;

    private Vector3 baseScale = Vector3.one;
    public bool CanActivate = false;

    /// <summary>
    /// Initializes the component, gets references to UIDocument and VisualElements,
    /// sets up the camera, and applies "Always On Top" setting if enabled.
    /// </summary>
    private void Awake()
    {
        document = GetComponent<UIDocument>();
        root = document.rootVisualElement;
        visual = document.visualTreeAsset;
        bool flowControl = SetupCamera();
        if (!flowControl)
            Debug.LogError("No valid camera");

        baseScale = transform.localScale;
        Deactivate();
    }

    /// <summary>
    /// Handles per-frame logic for UI orientation and scaling.
    /// If LookAtPlayer is enabled, it smoothly rotates the UI to face the camera.
    /// If ScaleOnDistance is enabled, it adjusts the UI's scale based on its distance from the camera.
    /// If HideOnDistance is enabled, it activates or deactivates the UI based on distance thresholds.
    /// </summary>
    void LateUpdate()
    {
        if (cam == null || settings == null) return;

        if (settings.LookAtPlayer)
        {
            Quaternion lookAt = Quaternion.LookRotation(cam.forward);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, lookAt, Time.unscaledDeltaTime * settings.Smoothing);
        }

        float distance = Vector3.Distance(transform.position, cam.transform.position);

        if (settings.ScaleOnDistance)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                baseScale * Mathf.Clamp(distance / settings.ViewingDistance, settings.MinimumScaleFactor, settings.MaximumScaleFactor),
                Time.unscaledDeltaTime * settings.Smoothing
            );
        }

        if (CheckDistance(transform.position) && settings.HideOnDistance)
            Deactivate();
        else
            Activate();

    }

    /// <summary>
    /// Activates the UI, making it visible by setting its opacity to 1.
    /// Logs an error if the settings are not assigned.
    /// </summary>
    public void Activate()
    {
        if (settings == null)
        {
            Debug.LogError("WorldSpaceUI: Settings are not assigned.", this);
            return;
        }

        if (CanActivate)
            root.style.opacity = 1;
        // TODO Add class to handle custom De/Activation 
    }

    /// <summary>
    /// Deactivates the UI, making it invisible by setting its opacity to 0.
    /// </summary>
    public void Deactivate()
    {
        if (root.style.opacity == 0) return;

        root.style.opacity = 0;
    }

    /// <summary>
    /// Called in the Unity editor when the script is loaded or a value is changed in the Inspector.
    /// Re-initializes the camera and "Always On Top" settings.
    /// </summary>
    private void Start()
    {
        SetupCamera();
        if (settings != null)
            SetupAlwaysOnTop();
    }

    /// <summary>
    /// Checks if the UI is outside the specified minimum and maximum distance from the camera.
    /// </summary>
    /// <param name="goPos">The position of the UI GameObject.</param>
    /// <returns>True if the UI should be hidden, false otherwise.</returns>
    private bool CheckDistance(Vector3 goPos)
    {
        if (settings == null || cam == null) return false;

        float distance = Vector3.Distance(goPos, cam.transform.position);
        return (distance >= settings.MaxDistance || distance <= settings.MinDistance);
    }

    /// <summary>
    /// Sets the GameObject's layer to the "AlwaysOnTop" layer.
    /// </summary>
    private void SetupAlwaysOnTop()
    {
        int AlwaysOnTop = LayerMask.NameToLayer(layerAOT);
        gameObject.layer = AlwaysOnTop;
    }

    /// <summary>
    /// Sets up the camera reference. If the camera is not assigned, it tries to find the main camera.
    /// If no camera is found, it logs an error and disables the component.
    /// </summary>
    /// <returns>True if a camera is successfully set up, false otherwise.</returns>
    private bool SetupCamera()
    {
        if (cam == null)
        {
            if (Camera.main != null)
            {
                cam = Camera.main.transform;
            }
            else
            {
                Debug.LogError("WorldSpaceUI: No camera assigned and Camera.main is not available.", this);
                enabled = false;
                return false;
            }
        }

        return true;
    }
}