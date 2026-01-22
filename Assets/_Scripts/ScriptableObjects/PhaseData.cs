using UnityEngine;

[CreateAssetMenu(fileName = "NewDayData", menuName = "Game/DayData")]
public class PhaseData : ScriptableObject
{
    [Range(0.01f, 10f)] public float TimeInMinutes;
    public AudioSO Music;

    public int GetPhaseDurationInSeconds() => Mathf.RoundToInt(TimeInMinutes * 60);
}