using System;
using System.Collections;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages a world-space popup UI, driven by a WSUISettings ScriptableObject.
/// Handles orientation, scaling, animations, and interaction based on the provided settings.
/// </summary>
public class WorldSpaceUI_Canvas : MonoBehaviour, IUI
{
    [SerializeField] private WSUISettings settings;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform target;

    Canvas canvas;
    CanvasGroup canvasGroup;
    Coroutine currentRoutine;

    private Vector3 baseScale = Vector3.one;

    private const string shaderTestMode = "unity_GUIZTestMode"; //The magic property we need to set
    [SerializeField] UnityEngine.Rendering.CompareFunction desiredUIComparison = UnityEngine.Rendering.CompareFunction.Always; //If you want to try out other effects
    [SerializeField] Graphic[] uiElementsToApplyTo;
    [SerializeField] TextMeshProUGUI[] uiTextElementsToApplyTo;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvasGroup = canvas.GetComponent<CanvasGroup>();

        canvasGroup = canvasGroup != null ? canvasGroup : canvas.gameObject.AddComponent<CanvasGroup>();

        bool flowControl = SetupCamera();
        if (!flowControl)
            return;

        if (settings.AlwaysOnTop)
            SetupAlwaysOnTop();

        canvasGroup.interactable = settings.IsInteractable;

        baseScale = transform.localScale;
        Deactivate();
    }

    void LateUpdate()
    {
        if (cam == null || settings == null) return;

        // Look at camera
        if (settings.LookAtPlayer)
        {
            Quaternion lookAt = Quaternion.LookRotation(cam.forward);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, lookAt, Time.unscaledDeltaTime * settings.Smoothing);
        }

        float distance = Vector3.Distance(transform.position, cam.transform.position);

        // Scale based on distance
        if (settings.ScaleOnDistance)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                baseScale * Mathf.Clamp(distance / settings.ViewingDistance, settings.MinimumScaleFactor, settings.MaximumScaleFactor),
                Time.unscaledDeltaTime * settings.Smoothing
            );
        }

        // Hide if outside range
        if (settings.HideOnDistance)
        {
            if (CheckDistance(transform.position))
                Deactivate();
            else
                Activate();
        }

        if (target != null)
            transform.position = target.position;
    }

    public void Activate()
    {
        if (settings == null)
        {
            Debug.LogError("WorldSpaceUI: Settings are not assigned.", this);
            return;
        }

        canvas.enabled = true;
        currentRoutine = StartCoroutine(FadeScaleRoutine(true));
        LayoutRebuilder.ForceRebuildLayoutImmediate(canvas.GetComponent<RectTransform>());
    }

    public void Deactivate()
    {
        if (!canvas.enabled) return;

        canvas.enabled = false;
        currentRoutine = StartCoroutine(FadeScaleRoutine(false));
    }

    private void OnValidate()
    {
        SetupCamera();
        if (settings != null)
            SetupAlwaysOnTop();
    }

    // Write my own Animation folder for multiple animation strategies
    public IEnumerator FadeScaleRoutine(bool show)
    {
        float duration = 0.25f;
        float time = 0f;

        float startAlpha = canvasGroup.alpha;
        float endAlpha = show ? 1f : 0f;

        canvasGroup.interactable = show && settings.IsInteractable;
        canvasGroup.blocksRaycasts = show && settings.IsInteractable;

        Vector3 startScale = show ? Vector3.zero : canvas.transform.localScale;
        Vector3 endScale;

        if (show)
        {
            float distance = Vector3.Distance(transform.position, cam.transform.position);
            if (settings.ScaleOnDistance)
            {
                endScale = Vector3.one * Mathf.Max(distance / settings.ViewingDistance, settings.MinimumScaleFactor);
            }
            else
            {
                endScale = Vector3.one;
            }
        }
        else
        {
            endScale = Vector3.zero;
        }

        while (time < duration)
        {
            float t = time / duration;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            canvas.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
        canvas.transform.localScale = endScale;

        if (!show)
        {
            gameObject.SetActive(false);
            target = null;
        }

        currentRoutine = null;
    }

    private bool CheckDistance(Vector3 goPos)
    {
        if (settings == null || !settings.HideOnDistance || cam == null) return false;

        float distance = Vector3.Distance(goPos, cam.transform.position);
        return (distance >= settings.MaxDistance || distance <= settings.MinDistance);
    }

    private void SetupAlwaysOnTop()
    {
        if (uiElementsToApplyTo.Length == 0)
        {
            uiElementsToApplyTo = gameObject.GetComponentsInChildren<Graphic>();
            uiTextElementsToApplyTo = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
        }

        foreach (var graphic in uiElementsToApplyTo)
        {
            Material material = graphic.materialForRendering;
            if (material == null)
            {
                Debug.LogError($"{nameof(WorldSpaceUI_Canvas)}: skipping target without material {graphic.name}.{graphic.GetType().Name}");
                continue;
            }

            Material materialCopy = new Material(material);
            materialCopy.SetInt(shaderTestMode, (int)desiredUIComparison);
            graphic.material = materialCopy;
        }

        foreach (var text in uiTextElementsToApplyTo)
        {
            Material material = text.fontMaterial;
            if (material == null)
            {
                Debug.LogError($"{nameof(WorldSpaceUI_Canvas)}: skipping target without material {text.name}.{text.GetType().Name}");
                continue;
            }

            Material materialCopy = new Material(material);
            materialCopy.SetInt(shaderTestMode, (int)desiredUIComparison);
            text.fontMaterial = materialCopy;
        }
    }

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
