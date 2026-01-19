using UnityEngine;

public class MinigameOpener : MonoBehaviour, IInteractable
{
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] PatternMinigame minigame;
    private bool completed = false;

    public bool CanInteract(PlayerController context)
    {
        return !completed;
    }

    public InteractionResult TryInteract(PlayerController context)
    {
        if (!CanInteract(context)) return InteractionResult.Fail("failed");
        MinigameRunner.Instance.StartGame(minigame);
        inputHandler.Exit += OnExit;
        return InteractionResult.Ok("Game started");
    }

    private void OnExit()
    {
        MinigameRunner.Instance.EndGame(minigame);
        inputHandler.Exit -= OnExit;
    }

    public void HasCompleted()
    {
        Debug.Log("***COMLETED*** ");
        completed = true;
    }
}
