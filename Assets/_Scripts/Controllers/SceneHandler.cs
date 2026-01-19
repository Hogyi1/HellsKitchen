using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles loading scenes, including async operations and tracking scene history.
/// This is no longer a Singleton. It's intended to be a component on a persistent GameManager.
/// </summary>
public class SceneHandler : MonoBehaviour
{
    public SceneName ActiveScene { get; private set; }
    public SceneName LastScene { get; private set; }
    public List<SceneTransitionData> SceneHistory { get; private set; } = new List<SceneTransitionData>();

    public string Default = "Default";
    public string MainMenu = "MainMenu";
    public string KitchenScene = "KitchenScene";
    public string NightScene = "NightScene";

    public enum SceneName
    {
        Default,
        MainMenu,
        KitchenScene,
        NightScene,
    }

    /// <summary>
    /// Holds data about a single scene transition.
    /// </summary>
    public struct SceneTransitionData
    {
        public SceneName FromScene;
        public SceneName ToScene;
        public DateTime Timestamp;
        public float TimeSpentInFromScene; // in seconds
    }

    private Dictionary<string, SceneName> sceneNameMap = new();
    private float timeSinceSceneLoad = 0f;
    private bool isLoading = false;


    private void Awake()
    {
        // Initialize the dictionary that maps string names to enum values for type safety.
        sceneNameMap = new Dictionary<string, SceneName>
        {
            { Default, SceneName.Default },
            { MainMenu, SceneName.MainMenu },
            { KitchenScene, SceneName.KitchenScene },
            { NightScene, SceneName.NightScene }
        };

        var currentSceneName = SceneManager.GetActiveScene().name;
        if (sceneNameMap.TryGetValue(currentSceneName, out SceneName initialScene))
            ActiveScene = initialScene;
        else
            ActiveScene = SceneName.Default;

    }

    private void Update() => timeSinceSceneLoad += Time.deltaTime;

    public void LoadMainMenu() => LoadScene(MainMenu);
    public void LoadNightScene() => LoadScene(NightScene);
    public void LoadDayScene() => LoadScene(KitchenScene);


    /// <summary>
    /// Public entry point to start loading a scene asynchronously.
    /// </summary>
    /// <param name="sceneName">The string name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        if (isLoading || !sceneNameMap.ContainsKey(sceneName)) return;

        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    /// <summary>
    /// Handles the actual asynchronous loading process, including stats and UI.
    /// </summary>
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        isLoading = true;

        var targetSceneEnum = sceneNameMap[sceneName];
        var transitionData = new SceneTransitionData
        {
            FromScene = ActiveScene,
            ToScene = targetSceneEnum,
            Timestamp = DateTime.UtcNow,
            TimeSpentInFromScene = timeSinceSceneLoad
        };

        SceneHistory.Add(transitionData);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        LastScene = ActiveScene;
        ActiveScene = targetSceneEnum;
        timeSinceSceneLoad = 0f;

        isLoading = false;
    }
}
