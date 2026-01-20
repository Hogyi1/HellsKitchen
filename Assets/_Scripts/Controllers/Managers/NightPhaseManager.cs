using System;
using UnityEngine;

public class NightPhaseManager : MonoBehaviour, INightPhaseManager
{
    [SerializeField] private RandomEventSpawner eventSpawner;
    [SerializeField] private PowerOutageEvent outageEvent;
    [SerializeField] private PowerBoxScript breakerbox;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private PhaseData nightData;
    [SerializeField] private AudioSO alarmSound;

    public float CooldownTime = 10f;
    public NightDataModel GameData { get; private set; }
    private LoopTimer nightTimer;

    public event Action<NightDataModel> OnPhaseEnd;
    public event Action OnPhaseStart;

    private void Awake()
    {
        if (eventSpawner != null)
        {
            eventSpawner.RandomEventInterval = 15;
        }
    }

    void Start()
    {
        GameManager.Instance.RegisterNightPhaseManager(this); // Assuming this method exists on the GameManager
        GameData = new NightDataModel(nightData.GetPhaseDurationInSeconds());
        nightTimer = new(1f, nightData.GetPhaseDurationInSeconds());

        eventSpawner = eventSpawner != null ? eventSpawner : FindAnyObjectByType<RandomEventSpawner>();
        outageEvent = outageEvent != null ? outageEvent : FindAnyObjectByType<PowerOutageEvent>();
        breakerbox = breakerbox != null ? breakerbox : FindAnyObjectByType<PowerBoxScript>();
        enemyManager = enemyManager != null ? enemyManager : FindAnyObjectByType<EnemyManager>();

        StartPhase();
    }

    public void StartPhase()
    {
        nightTimer.OnTimerStop += EndPhase;
        nightTimer.OnLoop += (loopCount) => GameData.Seconds--;
        eventSpawner.OnEventTriggered += EventSpawnerOnEventTriggered;
        outageEvent.PowerOutage += OnPowerOutage;
        outageEvent.PowerBack += OnPowerBack;
        breakerbox.OnPowerRestoreEvent += OnPowerRestore;

        breakerbox.isActive = false;
        AudioManager.Instance.PlayMusic(nightData.Music);
        eventSpawner.StartEvent();
        nightTimer.Start();

        OnPhaseStart?.Invoke();
    }

    public void EndPhase()
    {
        nightTimer.OnTimerStop -= EndPhase;
        nightTimer.OnLoop -= (loopCount) => GameData.Seconds--;
        eventSpawner.OnEventTriggered -= EventSpawnerOnEventTriggered;
        outageEvent.PowerOutage -= OnPowerOutage;
        outageEvent.PowerBack -= OnPowerBack;
        breakerbox.OnPowerRestoreEvent -= OnPowerRestore;

        eventSpawner.StopEvent(); // Assuming this method exists to counterpart StartEvent()

        var timer = new CountDownTimer(CooldownTime);
        timer.OnTimerStop += () => OnPhaseEnd?.Invoke(GameData);

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySFXUI(alarmSound);
        timer.Start();
    }

    private void OnPowerRestore()
    {
        outageEvent.PowerComesBack();
    }

    private void OnPowerOutage()
    {
        breakerbox.isActive = true;
        GameData.PowerOutages++;
    }

    private void OnPowerBack()
    {
        breakerbox.isActive = false;
    }

    private void EventSpawnerOnEventTriggered(RandomEvent randEvent)
    {
        switch (randEvent)
        {
            case RandomEvent.None:
                break;
            case RandomEvent.PowerOutage:
                if (!breakerbox.isActive)
                {
                    Debug.Log("Nightmanager: power outage");
                    outageEvent.PowerChange();
                }
                break;
            case RandomEvent.SpawnRobot:
                Debug.Log("Nightmanager: robot spawned");
                StartCoroutine(enemyManager.Spawn());
                GameData.RobotsSpawned++;
                break;
        }
    }
}