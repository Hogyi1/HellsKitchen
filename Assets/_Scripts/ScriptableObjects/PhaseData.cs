using UnityEngine;

[CreateAssetMenu(fileName = "NewDayData", menuName = "Game/DayData")]
public class PhaseData : ScriptableObject
{
    public int TimeInMinutes;
    public AudioSO Music;

    public int GetPhaseDurationInSeconds() => TimeInMinutes * 60;
}