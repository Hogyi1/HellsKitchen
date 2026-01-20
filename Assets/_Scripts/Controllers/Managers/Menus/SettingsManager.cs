using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

public class SettingsManager : MonoBehaviour
{
    // Struct to hold a snapshot of the settings state
    private struct SettingsState
    {
        public float masterVolume;
        public float musicVolume;
        public float sfxVolume;
        public float lookSensitivity;
        public int qualityLevelIndex;
        public int resolutionIndex;
        public bool isFullscreen;
    }

    private PlayerSettings playerSettings;

    // State management
    private SettingsState originalState;
    private SettingsState pendingState;

    // UI Controls
    private VisualElement root;
    private Slider masterVolumeSlider;
    private Slider musicVolumeSlider;
    private Slider sfxVolumeSlider;
    private Slider sensitivitySlider;
    private DropdownField qualityDropdown;
    private DropdownField resolutionDropdown;
    private Toggle fullscreenToggle;
    private Label masterVolumeValue;
    private Label musicVolumeValue;
    private Label sfxVolumeValue;
    private Label sensitivityValue;

    private Resolution[] availableResolutions;

    public void Initialize(VisualElement menuRoot, PlayerSettings settings)
    {
        root = menuRoot;
        playerSettings = settings;

        // Create the original and pending states from the definitive SO
        originalState = CreateStateFromPlayerSettings(playerSettings);
        pendingState = originalState;

        QueryUIElements();
        PopulateUI();
        RegisterValueChangeCallbacks();
    }

    /// <summary>
    /// Called when the user presses "Save". Commits pending changes.
    /// </summary>
    public void SaveChanges()
    {
        // Copy pending state over to the definitive PlayerSettings SO
        playerSettings.audio.masterVolume = pendingState.masterVolume;
        playerSettings.audio.musicVolume = pendingState.musicVolume;
        playerSettings.audio.sfxVolume = pendingState.sfxVolume;
        playerSettings.controls.mouseSensitivity = pendingState.lookSensitivity;
        playerSettings.graphics.qualityLevelIndex = pendingState.qualityLevelIndex;
        playerSettings.graphics.resolutionIndex = pendingState.resolutionIndex;
        playerSettings.graphics.isFullscreen = pendingState.isFullscreen;

        originalState = pendingState;
        playerSettings.Save();
    }

    /// <summary>
    /// Called when the user presses "Back". Reverts all pending changes.
    /// </summary>
    public void RevertChanges()
    {
        // Apply the original settings, discarding any pending changes
        ApplySettingsState(originalState);

        // Also update the UI to reflect the reverted state
        PopulateUI();
    }

    private void QueryUIElements()
    {
        masterVolumeSlider = root.Q<Slider>("master-volume-slider");
        musicVolumeSlider = root.Q<Slider>("music-volume-slider");
        sfxVolumeSlider = root.Q<Slider>("sfx-volume-slider");
        sensitivitySlider = root.Q<Slider>("sensitivity-slider");
        qualityDropdown = root.Q<DropdownField>("quality-dropdown");
        resolutionDropdown = root.Q<DropdownField>("resolution-dropdown");
        fullscreenToggle = root.Q<Toggle>("fullscreen-toggle");
        masterVolumeValue = root.Q<Label>("master-volume-value");
        musicVolumeValue = root.Q<Label>("music-volume-value");
        sfxVolumeValue = root.Q<Label>("sfx-volume-value");
        sensitivityValue = root.Q<Label>("sensitivity-value");
    }

    private void PopulateUI()
    {
        // Set UI values from the original, definitive state
        masterVolumeSlider.value = originalState.masterVolume * 100;
        musicVolumeSlider.value = originalState.musicVolume * 100;
        sfxVolumeSlider.value = originalState.sfxVolume * 100;
        sensitivitySlider.value = originalState.lookSensitivity;
        fullscreenToggle.value = originalState.isFullscreen;

        // Set labels
        UpdateSliderLabel(masterVolumeValue, masterVolumeSlider.value, true);
        UpdateSliderLabel(musicVolumeValue, musicVolumeSlider.value, true);
        UpdateSliderLabel(sfxVolumeValue, sfxVolumeSlider.value, true);
        UpdateSliderLabel(sensitivityValue, sensitivitySlider.value, false);

        // Populate dropdowns
        qualityDropdown.choices = QualitySettings.names.ToList();
        qualityDropdown.index = originalState.qualityLevelIndex;

        availableResolutions = Screen.resolutions;
        resolutionDropdown.choices = availableResolutions.Select(res => $"{res.width} x {res.height} @{res.refreshRateRatio.value.ToString("F0")}Hz").ToList();
        resolutionDropdown.index = originalState.resolutionIndex;
    }

    private void RegisterValueChangeCallbacks()
    {
        masterVolumeSlider.RegisterValueChangedCallback(evt => OnSliderChanged(masterVolumeValue, evt.newValue, v => pendingState.masterVolume = v / 100f, true));
        musicVolumeSlider.RegisterValueChangedCallback(evt => OnSliderChanged(musicVolumeValue, evt.newValue, v => pendingState.musicVolume = v / 100f, true));
        sfxVolumeSlider.RegisterValueChangedCallback(evt => OnSliderChanged(sfxVolumeValue, evt.newValue, v => pendingState.sfxVolume = v / 100f, true));
        sensitivitySlider.RegisterValueChangedCallback(evt => OnSliderChanged(sensitivityValue, evt.newValue, v => pendingState.lookSensitivity = v, false));

        qualityDropdown.RegisterValueChangedCallback(evt =>
        {
            pendingState.qualityLevelIndex = qualityDropdown.index;
            ApplySettingsState(pendingState);
        });
        resolutionDropdown.RegisterValueChangedCallback(evt =>
        {
            pendingState.resolutionIndex = resolutionDropdown.index;
            ApplySettingsState(pendingState);
        });
        fullscreenToggle.RegisterValueChangedCallback(evt =>
        {
            pendingState.isFullscreen = evt.newValue;
            ApplySettingsState(pendingState);
        });
    }

    private void OnSliderChanged(Label label, float newValue, System.Action<float> updateAction, bool round)
    {
        UpdateSliderLabel(label, newValue, round);
        updateAction.Invoke(newValue);
        ApplySettingsState(pendingState);
    }

    private void UpdateSliderLabel(Label label, float value, bool round)
    {
        if (label == null) return;
        label.text = round ? Mathf.RoundToInt(value).ToString() : value.ToString("F2");
    }

    private SettingsState CreateStateFromPlayerSettings(PlayerSettings settings)
    {
        return new SettingsState
        {
            masterVolume = settings.audio.masterVolume,
            musicVolume = settings.audio.musicVolume,
            sfxVolume = settings.audio.sfxVolume,
            lookSensitivity = settings.controls.mouseSensitivity,
            qualityLevelIndex = settings.graphics.qualityLevelIndex,
            resolutionIndex = settings.graphics.resolutionIndex,
            isFullscreen = settings.graphics.isFullscreen
        };
    }

    /// <summary>
    /// Applies a given settings state to the game in real-time.
    /// </summary>
    private void ApplySettingsState(SettingsState state)
    {
        QualitySettings.SetQualityLevel(state.qualityLevelIndex, false);
        Resolution res = availableResolutions[state.resolutionIndex];
        Screen.SetResolution(res.width, res.height, state.isFullscreen);

        AudioManager.Instance.SetMixerVolumes(state.masterVolume, state.musicVolume, state.sfxVolume);
    }
}