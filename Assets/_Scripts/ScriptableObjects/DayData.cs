using UnityEngine;

[CreateAssetMenu(fileName = "NewDayData", menuName = "Game/DayData")]
public class DayData : ScriptableObject
{
    public int TimeInMinutes;
    public AudioSO Music;

    public int GetDayDurationInSeconds() => TimeInMinutes * 60;
}