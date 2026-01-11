using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerCamera", menuName = "Game/Player/Settings/CameraSettings")]
public class PlayerCameraSettings : ScriptableObject
{
    [Header("Camera settings")]
    [Range(0.1f, 10f)] public float lookSensitivity = 0.1f;
    [Range(1f, 90f)] public float upperCameraLimit = 60f;
    [Range(1f, 90f)] public float lowerCameraLimit = 60f;

    [Header("Camera look settings")]
    public float runningFov = 80f;
    public float normalFov = 60f;
    public float crouchingVignette = 0.25f;
    public float fovChangeSpeed = 0.25f;
}
