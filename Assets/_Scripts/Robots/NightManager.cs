using UnityEngine;

public class NightManager : MonoBehaviour
{
    [SerializeField] private RandomEventSpawner eventSpawner;
    [SerializeField] private PowerOutageEvent outageEvent;
    [SerializeField] private PowerBoxScript breakerbox;
    [SerializeField] private EnemyManager enemyManager;

    private void Awake()
    {
        eventSpawner.RandomEventInterval = 15;
    }
    void Start()
    {
        enemyManager.IsEnabled = true;
        breakerbox.isActive = false;
        eventSpawner.OnEventTriggered += EventSpawnerOnEventTriggered;
        outageEvent.PowerOutage += OnPowerOutage;
        outageEvent.PowerBack += OnPowerBack;
        breakerbox.OnPowerRestoreEvent += OnPowerRestore; 
    }

    private void OnPowerRestore()
    {
        outageEvent.PowerComesBack();
    }
    private void OnPowerOutage()
    {
        breakerbox.isActive = true;
    }
    private void OnPowerBack()
    {
        breakerbox.isActive = false;
    }

    private void EventSpawnerOnEventTriggered(TimeManager.RandomEvent randEvent)
    {
        switch(randEvent)
        {
            case TimeManager.RandomEvent.None:
                break;
            case TimeManager.RandomEvent.PowerOutage:
                if(!breakerbox.isActive)
                {
                    Debug.Log("Nightmanager: power outage");
                    outageEvent.PowerChange();
                }
                break;
            case TimeManager.RandomEvent.SpawnRobot:
                Debug.Log("Nightmanager: robot spawned");
                enemyManager.Spawn();
                break;
        }
    }
}
