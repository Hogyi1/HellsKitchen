using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MinigameRunner : MonoBehaviour
{
    public static MinigameRunner Instance { get; private set; } // Call this to get the instance
    [SerializeField] InputHandler input;

    IMinigame activeMinigame;

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

    public void StartGame(IMinigame minigame) => StartCoroutine(StartMinigame(minigame));
    public void EndGame(IMinigame minigame) => StartCoroutine(EndMinigame());

    /// <summary>
    /// Starts the specified minigame, transitioning the input system and camera as needed.
    /// </summary>
    /// <remarks>This method ensures that only one minigame can be active at a time. If a minigame is already
    /// active, the method exits immediately. If the specified minigame provides a custom camera, the camera system
    /// transitions to focus on it before starting the minigame. The input system is switched to the Minigame action map
    /// during the transition.</remarks>
    /// <param name="minigame">The minigame to start. Must implement the <see cref="IMinigame"/> interface.</param>
    /// <returns>An enumerator that can be used to control the coroutine execution.</returns>
    IEnumerator StartMinigame(IMinigame minigame)
    {
        if (activeMinigame != null) yield break;

        activeMinigame = minigame;
        var camController = CameraController.Instance;

        input.SwitchToMap(InputHandler.ActionMap.Minigame);
        if (activeMinigame.GetCamera() != null)
        {
            camController.RequestFocus(activeMinigame.GetCamera());
            yield return new WaitUntil(() => !camController.IsBlending());
        }

        activeMinigame.StartGame();
    }

    /// <summary>
    /// Ends the currently active minigame and transitions back to the first-person view.
    /// </summary>
    /// <remarks>This method finalizes the active minigame by invoking its <see cref="Minigame.EndGame"/>
    /// method. If the minigame has an associated camera, the focus is released, and the method waits for the camera
    /// transition to complete before switching the input mode to first-person. If no minigame is active, the method
    /// exits immediately.</remarks>
    /// <returns></returns>
    IEnumerator EndMinigame()
    {
        if (activeMinigame == null) yield break;
        activeMinigame.EndGame();
        var camController = CameraController.Instance;
        if (activeMinigame.GetCamera() != null)
        {
            camController.ReleaseFocus(activeMinigame.GetCamera());
            yield return new WaitUntil(() => !camController.IsBlending());
        }
        input.SwitchToFirstPerson();
        activeMinigame = null;
    }
}

public interface IMinigame
{
    void StartGame();
    void EndGame();
    CinemachineCamera GetCamera();
}