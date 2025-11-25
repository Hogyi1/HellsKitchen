using System;
using UnityEngine;
public class TimeManager : Singleton<TimeManager>
{
    public bool IsPaused { get; private set; }
    [SerializeField] TimeSettings timeSettings;

    public enum RandomEvent
    {
        None,
        NewOrder,
        SpawnRobot,
        PowerOutage
    }

    [Serializable]
    public struct WeightedEvent
    {
        public RandomEvent Event;
        public float Weight;
    }

    public void SetPause(bool pause)
    {
        IsPaused = pause;
        Time.timeScale = pause ? 0f : 1f;
    }
}