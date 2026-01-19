using UnityEngine;

[CreateAssetMenu(fileName = "NewGraphicsSettings", menuName = "Game/Player/Settings/GraphicsSettings")]
public class PlayerGraphicsSettings : ScriptableObject
{
    public int qualityLevelIndex;
    public int resolutionIndex;
    public bool isFullscreen;
}

