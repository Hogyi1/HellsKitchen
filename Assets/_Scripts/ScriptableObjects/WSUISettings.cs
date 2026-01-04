using UnityEngine;

[CreateAssetMenu(fileName = "NewWSUISettings", menuName = "Game/UI/Settings/WorldSpace/WSUISettings")]
public class WSUISettings : ScriptableObject
{
    public bool HideOnDistance = false;
    [Range(0, 20f)] public float MinDistance = 1f;
    [Range(0, 20f)] public float MaxDistance = 1f;

    public bool LookAtPlayer = false;
    public bool AlwaysOnTop = false;

    public bool ScaleOnDistance = false;
    [Range(0, 20f)] public float ViewingDistance = 1f;
    [Range(0, 3f)] public float MaximumScaleFactor = 1f;
    [Range(0, 1f)] public float MinimumScaleFactor = 1f;

    public bool IsInteractable = false;

    [Range(5f, 50f)] public float Smoothing = 10f;
}