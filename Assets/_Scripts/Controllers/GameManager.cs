using System;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-5)]
public class GameManager : PersistentSingleton<GameManager>
{
    public enum GamePhase
    {
        MainMenu,
        Day,
        Night,
        PauseMenu
    }

    [SerializeField] PlayerSettings playerSettings;
    [SerializeField] PlayerMovementSettings playerMovement;
    [SerializeField] PlayerCameraSettings cameraSettings;

    [SerializeField] InputHandler input;
    [SerializeField] SceneHandler sceneHandler;
    [SerializeField] PauseMenuManager pauseManager;

    private DayPhaseController dayPhaseManager;

    public static PlayerSettings PlayerSettings => Instance.playerSettings;
    public static PlayerMovementSettings MovementSettings => Instance.playerMovement;
    public static PlayerCameraSettings CameraSettings => Instance.cameraSettings;
    public static InputHandler Input => Instance.input;

    public GamePhase Phase = GamePhase.MainMenu;
    private GamePhase LastPhase = GamePhase.MainMenu;

    void Start()
    {
        playerSettings.Load();
        ApplySettings();
        Phase = GamePhase.MainMenu;
        LastPhase = GamePhase.MainMenu;
        SwitchInputMap(Phase);
        input.Pause += HandlePause;
    }

    private void HandlePause()
    {
        // 1. Do not allow pausing in the main menu.
        if (Phase == GamePhase.MainMenu) return;

        // 2. If we are NOT paused, pause the game.
        if (!pauseManager.IsPaused)
        {
            LastPhase = Phase;          // Save the state we were in
            Phase = GamePhase.PauseMenu; // Set the new state
            pauseManager.TogglePause();   // Show UI, set time to 0
            SwitchInputMap(Phase);      // Switch to UI input map
        }
        // 3. If we ARE paused, resume the game.
        else
        {
            ResumeGame();
        }
    }

    public void StartNewGame()
    {
        sceneHandler.LoadDayScene();
        Phase = GamePhase.Day;
        SwitchInputMap(Phase);
    }

    public void ApplySettings()
    {
        AudioManager.Instance.ApplyMixerVolumes(playerSettings.audio);
        QualitySettings.SetQualityLevel(playerSettings.graphics.qualityLevelIndex, true);
        Resolution selectedRes = Screen.resolutions[playerSettings.graphics.resolutionIndex];
        Screen.SetResolution(selectedRes.width, selectedRes.height, playerSettings.graphics.isFullscreen);
        cameraSettings.lookSensitivity = playerSettings.controls.mouseSensitivity;
    }

    public void RegisterDayPhaseManager(DayPhaseController manager)
    {
        dayPhaseManager = manager;
        dayPhaseManager.OnDayEnd += HandleDayEnd;
    }

    private void HandleDayEnd(KitchenDataModel dayData)
    {
        sceneHandler.LoadNightScene();
        Phase = GamePhase.Night;
        SwitchInputMap(Phase);
    }

    public void BackToMainMenu()
    {
        // Ensure the game is unpaused before changing scenes
        if (pauseManager.IsPaused)
        {
            pauseManager.TogglePause(); // This sets timescale to 1 and hides the UI
        }
        Time.timeScale = 1f; // Belt and suspenders

        sceneHandler.LoadMainMenu();
        Phase = GamePhase.MainMenu;
        LastPhase = GamePhase.MainMenu; // Reset last phase
        SwitchInputMap(Phase);
    }

    public void ResumeGame()
    {
        // Can only resume if we are actually paused
        if (!pauseManager.IsPaused) return;

        Phase = LastPhase;          // Restore the previous game phase
        pauseManager.TogglePause();   // Hide UI, set time to 1
        SwitchInputMap(Phase);      // Switch back to the appropriate input map
    }

    private void SwitchInputMap(GamePhase phase)
    {
        switch (phase)
        {
            case GamePhase.MainMenu:
            case GamePhase.PauseMenu:
                input.SwitchToUI();
                break;
            case GamePhase.Day:
                input.SwitchToDay();
                break;
            case GamePhase.Night:
                input.SwitchToNight();
                break;
            default:
                break;
        }
    }
}
