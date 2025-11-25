using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static TimeManager;

public class RandomEventSpawner : MonoBehaviour
{
    [SerializeField] private WeightedEvent[] randomEvents;

    private float totalWeight = 0f;
    private LoopTimer randomEventTimer;

    public float RandomEventInterval { get; set; }
    public Dictionary<RandomEvent, int> playedEvents { get; private set; }
    public event UnityAction<RandomEvent> OnEventTriggered = delegate { };

    private void Start()
    {
        playedEvents = new();

        foreach (var weightedEvent in randomEvents)
        {
            totalWeight += weightedEvent.Weight;
        }

        randomEventTimer = new LoopTimer(RandomEventInterval, -1);
        randomEventTimer.OnLoop += (loopCount) => HandleRandomEvents();
    }

    private void OnEnable() => randomEventTimer?.Start();
    private void OnDisable() => randomEventTimer?.Stop();

    private void HandleRandomEvents()
    {
        RandomEvent randomEvent = GetRandomEvent();
        if (randomEvent == RandomEvent.None)
            return;

        // Keep track of events if we want to limit frequency or for analytics
        if (playedEvents.ContainsKey(randomEvent))
            playedEvents[randomEvent]++;
        else
            playedEvents.Add(randomEvent, 1);

        OnEventTriggered.Invoke(randomEvent);
    }

    private RandomEvent GetRandomEvent()
    {
        if (randomEvents == null || randomEvents.Length == 0)
            return RandomEvent.None;


        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        foreach (var weightedEvent in randomEvents)
        {
            cumulativeWeight += weightedEvent.Weight;
            if (roll < cumulativeWeight)
            {
                return weightedEvent.Event;
            }
        }

        return RandomEvent.None;
    }

}