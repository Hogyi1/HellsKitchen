using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    //[SerializeField] private MinigameConfig config;
    public MinigameOpener[] openers;
    public PatternMinigame[] Minigames;
    int completed = 0;
    private void Start()
    {
        for(int i = 0; i < openers.Length; i++)
        {
            int num = i;
            Minigames[i].MinigameCompleted += () => OnMinigameCompleted(num);
        }
    }

    private void OnMinigameCompleted(int i)
    {
        completed++;
        openers[i].HasCompleted();
    }

    
    public float CalculateMoney()
    {
        return 50 * (float)completed/ openers.Length;
    }
}
