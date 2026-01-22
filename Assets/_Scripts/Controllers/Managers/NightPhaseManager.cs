using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class NightPhaseManager : MonoBehaviour, INightPhaseManager
{
    [SerializeField] private RandomEventSpawner eventSpawner;
    [SerializeField] private PowerOutageEvent outageEvent;
    [SerializeField] private PowerBoxScript breakerbox;
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private PhaseData nightData;
    [SerializeField] private AudioSO alarmSound;
    [SerializeField] private NightUIHandler uiManager;
    [SerializeField] private UIDocument jumpScareUI;

    private VisualElement rootElement;
    private VisualElement robot1;
    private VisualElement robot2;

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

        rootElement = jumpScareUI.rootVisualElement;
        robot1 = rootElement.Q<VisualElement>("Robot1");
        robot2 = rootElement.Q<VisualElement>("Robot2");
        rootElement.style.display = DisplayStyle.None;
    }

    void Start()
    {
        GameManager.Instance.RegisterNightPhaseManager(this);
        GameData = new NightDataModel(nightData.GetPhaseDurationInSeconds());
        nightTimer = new(1f, nightData.GetPhaseDurationInSeconds());

        eventSpawner = eventSpawner != null ? eventSpawner : FindAnyObjectByType<RandomEventSpawner>();
        outageEvent = outageEvent != null ? outageEvent : FindAnyObjectByType<PowerOutageEvent>();
        breakerbox = breakerbox != null ? breakerbox : FindAnyObjectByType<PowerBoxScript>();
        enemyManager = enemyManager != null ? enemyManager : FindAnyObjectByType<EnemyManager>();

        uiManager.BindData(GameData);

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
        enemyManager.PlayerDied += OnPlayerDied;

        breakerbox.isActive = false;
        AudioManager.Instance.PlayMusic(nightData.Music);
        eventSpawner.StartEvent();
        nightTimer.Start();

        OnPhaseStart?.Invoke();
    }

    private void OnPlayerDied(EnemyType enemyType)
    {
        var timer = new CountDownTimer(3f);
        timer.OnTimerStop += () => ShowJumpScareUI(enemyType);
        timer.Start();
    }

    private void ShowJumpScareUI(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.TypeA:
                robot1.style.display = DisplayStyle.Flex;
                robot2.style.display = DisplayStyle.None;
                break;
            case EnemyType.TypeB:
                robot2.style.display = DisplayStyle.Flex;
                robot1.style.display = DisplayStyle.None;
                break;
        }

        rootElement.style.display = DisplayStyle.Flex;
        EndPhase();
    }

    public void EndPhase()
    {
        nightTimer.OnTimerStop -= EndPhase;
        nightTimer.OnLoop -= (loopCount) => GameData.Seconds--;
        eventSpawner.OnEventTriggered -= EventSpawnerOnEventTriggered;
        outageEvent.PowerOutage -= OnPowerOutage;
        outageEvent.PowerBack -= OnPowerBack;
        breakerbox.OnPowerRestoreEvent -= OnPowerRestore;

        eventSpawner.StopEvent();

        var timer = new CountDownTimer(CooldownTime);
        timer.OnTimerStop += () => OnPhaseEnd?.Invoke(GameData);
        timer.Start();

        AudioManager.Instance.StopMusic(5f);
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