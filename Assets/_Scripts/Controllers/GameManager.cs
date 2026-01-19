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

    private DayPhaseManager dayPhaseManager;

    public static PlayerSettings PlayerSettings => Instance.playerSettings;
    public static InputHandler Input => Instance.input;

    void Start()
    {
        ApplySettings();
        input.SwitchToUI();
    }

    public void StartNewGame()
    {
        sceneHandler.LoadDayScene();
        input.SwitchToDay();
    }

    public void ApplySettings()
    {
        AudioManager.Instance.ApplyMixerVolumes();
        QualitySettings.SetQualityLevel(playerSettings.graphicsSettings.qualityLevelIndex, true);
        Resolution selectedRes = Screen.resolutions[playerSettings.graphicsSettings.resolutionIndex];
        Screen.SetResolution(selectedRes.width, selectedRes.height, playerSettings.graphicsSettings.isFullscreen);
    }

    public void RegisterDayPhaseManager(DayPhaseManager manager)
    {
        dayPhaseManager = manager;
        dayPhaseManager.OnDayEnd += HandleDayEnd;
    }

    private void HandleDayEnd(KitchenDataModel dayData)
    {

    }
}

public class DayPhaseManager : MonoBehaviour
{
    public event Action OnDayStart = delegate { };
    public event Action<KitchenDataModel> OnDayEnd = delegate { };

    public AudioSO DayTimeMusic;

    private KitchenDataModel GameData;

    private void Start()
    {
        GameManager.Instance.RegisterDayPhaseManager(this);
        GameData = new KitchenDataModel();
    }

    public void StartDay()
    {
        OnDayStart?.Invoke();
    }
    public void EndDay()
    {
        OnDayEnd?.Invoke(GameData);
    }
}
