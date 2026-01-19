using System;
using Unity.Cinemachine;
using UnityEngine;

public class PatternMinigame : MonoBehaviour, IMinigame
{
    public event Action MinigameCompleted = delegate { };
    public Canvas parentCanvas;
    public AudioSO victory;
    public AudioSource source;
    public virtual void EndGame()
    {
        //Nothing
    }

    public virtual CinemachineCamera GetCamera()
    {
        return null;
    }

    public virtual void StartGame()
    {
        //nothing
    }

    public void GameCompleted()
    {
        MinigameCompleted?.Invoke();
        AudioManager.Instance.PlaySFX(victory, source);
    }
}
