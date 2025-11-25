using System;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.Rendering.Universal;

// TODO move camera with crouching
public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] CinemachineCamera firstPersonCamera;
    [SerializeField] InputHandler input;
    [SerializeField] PlayerCameraSettings settings;

    Tween currentVignetteTween;
    Vignette vignette;
    public Vector2 cameraRotation;

    private void Awake()
    {
        firstPersonCamera = cameraTransform.GetComponentInChildren<CinemachineCamera>();
        settings = settings != null ? settings : ScriptableObject.CreateInstance<PlayerCameraSettings>();

        var volumeSettings = firstPersonCamera.GetComponentInChildren<CinemachineVolumeSettings>();
        if (volumeSettings != null) volumeSettings.Profile.TryGet(out vignette);
    }

    private void Update()
    {
        HandleCameraRotation();
    }

    /// <summary>
    /// Handles camera rotation. Rotates camera on the up-down axis only
    /// </summary>
    void HandleCameraRotation()
    {
        cameraRotation.x += input.LookDirection.x * settings.lookSensitivity;
        cameraRotation.y += -input.LookDirection.y * settings.lookSensitivity;

        cameraRotation.y = Mathf.Clamp(cameraRotation.y, -settings.upperCameraLimit, settings.lowerCameraLimit);
        cameraTransform.localRotation = Quaternion.Euler(cameraRotation.y, 0f, 0f);
    }

    /// <summary>
    /// Changes the vignette effect based on the player's crouching state.
    /// </summary>
    /// <param name="isCrouching"></param>
    /// <param name="timeToSet"></param>
    public void ChangeVignette(bool isCrouching, float timeToSet)
    {
        if (vignette == null) return;

        float targetIntensity = isCrouching ? settings.crouchingVignette : 0f;
        currentVignetteTween?.Kill();
        currentVignetteTween = DOTween.To(() => vignette.intensity.value,
                                         x => vignette.intensity.value = x,
                                         targetIntensity,
                                         Mathf.Max(0.0001f, timeToSet))
                                    .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// Changes the field of view (FOV) based on the player's sprinting state.
    /// </summary>
    /// <param name="isSprinting"></param>
    public void ChangeFOV(bool isSprinting)
    {
        if (firstPersonCamera == null) return;

        float targetFOV = isSprinting ? settings.runningFov : settings.normalFov;
        float duration = Mathf.Max(0.0001f, settings.fovChangeSpeed);
        firstPersonCamera.DOKill();
        DOTween.To(() => firstPersonCamera.Lens.FieldOfView,
                                     x => firstPersonCamera.Lens.FieldOfView = x,
                                     targetFOV,
                                     duration)
                                .SetEase(Ease.InOutSine);
    }
}
