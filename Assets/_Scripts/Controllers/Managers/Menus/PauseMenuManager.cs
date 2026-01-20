using System;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using static GameManager;

public class PauseMenuManager : MonoBehaviour
{
    public bool IsPaused { get; private set; }
    [SerializeField] UIDocument pauseMenuUI;

    [Header("Audio Events")]
    [Tooltip("Sound to play on button hover.")]
    [SerializeField] private AudioSO hoverSound;
    [Tooltip("Sound to play on button click.")]
    [SerializeField] private AudioSO selectSound;

    Button _continueButton;
    Button _backToButton;

    private void Start()
    {
        pauseMenuUI = pauseMenuUI != null ? pauseMenuUI : GetComponent<UIDocument>();
        var root = pauseMenuUI.rootVisualElement;

        if (root != null)
        {
            _continueButton = root.Q<Button>("ContinueButton");
            _backToButton = root.Q<Button>("MainMenuButton");

            RegisterAudioEvents(_continueButton);
            RegisterAudioEvents(_backToButton);

            RegisterCallbacks();
        }

        pauseMenuUI.rootVisualElement.style.display = DisplayStyle.None;
    }

    private void RegisterCallbacks()
    {
        _continueButton?.RegisterCallback<ClickEvent>(evt => ContinuePressed());
        _backToButton?.RegisterCallback<ClickEvent>(evt => MainMenuPressed());
    }

    private void ContinuePressed()
    {
        GameManager.Instance.ResumeGame();
    }

    private void MainMenuPressed()
    {
        GameManager.Instance.BackToMainMenu();
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            Time.timeScale = 0f;
            pauseMenuUI.rootVisualElement.style.display = DisplayStyle.Flex;
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenuUI.rootVisualElement.style.display = DisplayStyle.None;
        }
    }

    private void RegisterAudioEvents(Button button)
    {
        button?.RegisterCallback<PointerEnterEvent>(evt => AudioManager.Instance.PlaySFXUI(hoverSound));
        button?.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlaySFXUI(selectSound));
    }
}