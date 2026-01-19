using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MinigameRunner : MonoBehaviour
{
    public static MinigameRunner Instance { get; private set; } // Call this to get the instance
    [SerializeField] InputHandler input;

    IMinigame activeMinigame;
    private CinemachineCamera cam;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void StartGame(MonoBehaviour minigame) => StartCoroutine(StartMinigame(minigame));
    public void EndGame(MonoBehaviour minigame) => StartCoroutine(EndMinigame());

    /// <summary>
    /// Starts the specified minigame, transitioning the input system and camera as needed.
    /// </summary>
    
    IEnumerator StartMinigame(MonoBehaviour minigame)
    {
        if (activeMinigame != null) yield break;
        if(minigame is not PatternMinigame newMiniGame) yield break;
        activeMinigame = newMiniGame;
        var camController = CameraController.Instance;
        
        input.SwitchToMap(InputHandler.ActionMap.Minigame);
        if (activeMinigame.GetCamera() != null)
        {
            cam = activeMinigame.GetCamera();
            camController.RequestFocus(activeMinigame.GetCamera());
            yield return new WaitUntil(() => !camController.IsBlending());
        }

        activeMinigame.StartGame();
    }

    /// <summary>
    /// Ends the currently active minigame and transitions back to the first-person view.
    /// </summary>
    
    IEnumerator EndMinigame()
    {
        if (activeMinigame == null) yield break;
        activeMinigame.EndGame();
        var camController = CameraController.Instance;
        if (cam != null)
        {
            Debug.Log("asd");
           
            camController.ReleaseFocus(cam);
            yield return new WaitUntil(() => !camController.IsBlending());
        }
        input.SwitchToNight();
        activeMinigame = null;
    }
}

public interface IMinigame
{
    void StartGame();
    void EndGame();
    CinemachineCamera GetCamera();
}