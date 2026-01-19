using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "Game/Player/Settings/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public PlayerGraphicsSettings graphicsSettings;
    public PlayerCameraSettings cameraSettings;
    public PlayerMovementSettings movementSettings;
    public PlayerAudioSettings playerAudioSettings;
}

