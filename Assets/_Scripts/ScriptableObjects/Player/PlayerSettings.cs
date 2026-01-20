using System;
using UnityEngine;

[Serializable]
public class GraphicsSettings
{
    public int qualityLevelIndex;
    public int resolutionIndex;
    public bool isFullscreen;
}

[Serializable]
public class AudioSettings
{
    [Range(0.0001f, 1f)] public float masterVolume = 1f;
    [Range(0.0001f, 1f)] public float musicVolume = 1f;
    [Range(0.0001f, 1f)] public float sfxVolume = 1f;
}

[Serializable]
public class ControlsSettings
{
    [Range(0.1f, 2f)] public float mouseSensitivity = 1f;
}

[CreateAssetMenu(fileName = "NewPlayerSettings", menuName = "Game/Player/Settings/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public GraphicsSettings graphics = new();
    public AudioSettings audio = new();
    public ControlsSettings controls = new();

    private const string PREFS_KEY = "PlayerSettings";

    /// <summary>
    /// Saves the current state of the settings to PlayerPrefs as a JSON string.
    /// </summary>
    public void Save()
    {
        string json = JsonUtility.ToJson(this);
        PlayerPrefs.SetString(PREFS_KEY, json);
        PlayerPrefs.Save();
        Debug.Log("PlayerSettings saved to PlayerPrefs.");
    }

    /// <summary>
    /// Loads the settings from PlayerPrefs and overwrites the current values.
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey(PREFS_KEY))
        {
            string json = PlayerPrefs.GetString(PREFS_KEY);
            JsonUtility.FromJsonOverwrite(json, this);
            Debug.Log("PlayerSettings loaded from PlayerPrefs.");
        }
    }
}