using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-5)]
public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] PlayerSettings playerSettings;
    [SerializeField] InputHandler input;
    [SerializeField] SceneHandler sceneHandler;
    public static PlayerSettings PlayerSettings => Instance.playerSettings;

    void Start()
    {
        ApplySettings();
        input.SwitchToUI();
    }

    public void StartNewGame()
    {
        sceneHandler.LoadDayScene();
        AudioManager.Instance.StopMusic(5f);
        input.SwitchToDay();
    }

    public void ApplySettings()
    {
        AudioManager.Instance.ApplyMixerVolumes();
        QualitySettings.SetQualityLevel(playerSettings.graphicsSettings.qualityLevelIndex, true);
        Resolution selectedRes = Screen.resolutions[playerSettings.graphicsSettings.resolutionIndex];
        Screen.SetResolution(selectedRes.width, selectedRes.height, playerSettings.graphicsSettings.isFullscreen);
    }
}
