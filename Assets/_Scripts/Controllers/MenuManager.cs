using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument), typeof(SettingsManager))]
public class MenuManager : MonoBehaviour
{
    [Header("Menu Assets")]
    [Tooltip("The UXML file for the main menu.")]
    [SerializeField] private VisualTreeAsset mainMenuAsset;
    [Tooltip("The UXML file for the settings menu.")]
    [SerializeField] private VisualTreeAsset settingsMenuAsset;

    [Header("Audio Events")]
    [Tooltip("Sound to play on button hover.")]
    [SerializeField] private AudioSO hoverSound;
    [Tooltip("Sound to play on button click.")]
    [SerializeField] private AudioSO selectSound;
    [Tooltip("Sound to play on button click.")]
    [SerializeField] private AudioSO backgroundMusic;

    [Header("Managers")]
    [Tooltip("The manager responsible for handling the settings logic.")]
    [SerializeField] private SettingsManager settingsManager;

    [Header("Configuration")]
    [Tooltip("Duration of the fade transition between menus in seconds.")]
    [SerializeField] private float fadeDuration = 0.3f;

    private VisualElement root;
    private VisualElement currentMenu;
    private UIDocument currentDocument;

    // Menu element queries
    private Button newGameButton;
    private Button settingsButton;
    private Button quitGameButton;
    private Button backButton;
    private Button saveButton;

    void Awake()
    {
        settingsManager = settingsManager != null ? settingsManager : GetComponent<SettingsManager>();
        currentDocument = GetComponent<UIDocument>();
        root = currentDocument.rootVisualElement.Q<VisualElement>("Container");
    }

    void Start()
    {
        currentMenu = mainMenuAsset.CloneTree();
        currentMenu.AddToClassList("menu-screen");
        root.Add(currentMenu);

        RegisterMainMenuCallbacks();
        AudioManager.Instance.PlayMusic(backgroundMusic);
    }

    #region Menu Switching

    private void ShowMainMenu()
    {
        SwitchMenu(mainMenuAsset, RegisterMainMenuCallbacks);
    }

    private void ShowSettingsMenu()
    {
        SwitchMenu(settingsMenuAsset, RegisterSettingsMenuCallbacks);
    }

    private void SwitchMenu(VisualTreeAsset newMenuAsset, System.Action onMenuSwitchedCallback)
    {
        StartCoroutine(SwitchMenuCoroutine(newMenuAsset, onMenuSwitchedCallback));
    }

    private IEnumerator SwitchMenuCoroutine(VisualTreeAsset newMenuAsset, System.Action onMenuSwitchedCallback)
    {
        currentMenu.AddToClassList("menu-screen--hidden");
        yield return new WaitForSeconds(fadeDuration);

        root.Remove(currentMenu);

        currentMenu = newMenuAsset.CloneTree();
        currentMenu.AddToClassList("menu-screen");
        currentMenu.AddToClassList("menu-screen--hidden");
        root.Add(currentMenu);

        onMenuSwitchedCallback?.Invoke();

        yield return null;
        currentMenu.RemoveFromClassList("menu-screen--hidden");
    }

    #endregion

    #region Callback Registration

    private void RegisterMainMenuCallbacks()
    {
        newGameButton = currentMenu.Q<Button>("new-game-button");
        settingsButton = currentMenu.Q<Button>("settings-button");
        quitGameButton = currentMenu.Q<Button>("quit-game-button");

        newGameButton?.RegisterCallback<ClickEvent>(evt => StartNewGame());
        settingsButton?.RegisterCallback<ClickEvent>(evt => ShowSettingsMenu());
        quitGameButton?.RegisterCallback<ClickEvent>(evt => QuitGame());

        // Register audio events for all buttons
        RegisterAudioEvents(newGameButton);
        RegisterAudioEvents(settingsButton);
        RegisterAudioEvents(quitGameButton);
    }

    private void RegisterSettingsMenuCallbacks()
    {
        // Initialize the SettingsManager with the new menu's root
        if (settingsManager != null)
        {
            settingsManager.Initialize(currentMenu, GameManager.PlayerSettings);
        }

        backButton = currentMenu.Q<Button>("back-button");
        saveButton = currentMenu.Q<Button>("save-button");
        backButton?.RegisterCallback<ClickEvent>(evt => { ShowMainMenu(); settingsManager.RevertChanges(); });
        saveButton?.RegisterCallback<ClickEvent>(evt => SaveSettings());

        RegisterAudioEvents(backButton);
        RegisterAudioEvents(saveButton);
    }

    #endregion

    private void SaveSettings()
    {
        if (settingsManager != null)
        {
            settingsManager.SaveChanges();
        }

        AudioManager.Instance.PlaySFXUI(selectSound);
        Debug.Log("Settings saved.");
    }

    #region Button Actions

    private void StartNewGame()
    {
        AudioManager.Instance.PlaySFXUI(selectSound);
        GameManager.Instance.StartNewGame();
        Debug.Log("Starting New Game...");
    }

    private void QuitGame()
    {
        AudioManager.Instance.PlaySFXUI(selectSound);
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    #endregion

    #region Audio

    private void RegisterAudioEvents(Button button)
    {
        button?.RegisterCallback<PointerEnterEvent>(evt => AudioManager.Instance.PlaySFXUI(hoverSound));
        button?.RegisterCallback<ClickEvent>(evt => AudioManager.Instance.PlaySFXUI(selectSound));
    }
    #endregion
}
