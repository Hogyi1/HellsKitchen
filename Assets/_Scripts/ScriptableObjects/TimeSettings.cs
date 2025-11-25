using UnityEngine;

[CreateAssetMenu(fileName = "NewTimeSettings", menuName = "Game/Time/Settings/TimeSettings")]
public class TimeSettings : ScriptableObject
{
    public float secondsPerGameHour = 60f;
    public float dayDurationInGameHours = 3.5f;
    public float nightDurationInGameHours = 6f;

    [Header("Random event")]
    public float randomEventInterval = 30f;
}