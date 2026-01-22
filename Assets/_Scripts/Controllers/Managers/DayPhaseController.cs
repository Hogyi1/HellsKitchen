using System;
using UnityEngine;

[RequireComponent(typeof(KitchenUIHandler))]
public class DayPhaseController : MonoBehaviour, IDayPhaseManager
{
    public event Action OnPhaseStart = delegate { };
    public event Action<KitchenDataModel> OnPhaseEnd = delegate { };

    public PhaseData dayData;
    public float CooldownTime = 10f;

    [SerializeField] private AudioSO alarmSound;
    [SerializeField] private KitchenUIHandler uiManager;
    [SerializeField] private OrderManager orderManager;

    public KitchenDataModel GameData { get; private set; }

    private LoopTimer dayTimer;

    private void Start()
    {
        GameManager.Instance.RegisterDayPhaseManager(this);
        GameData = new KitchenDataModel(dayData.GetPhaseDurationInSeconds());
        dayTimer = new(1f, dayData.GetPhaseDurationInSeconds());

        orderManager = orderManager != null ? orderManager : FindAnyObjectByType<OrderManager>();
        uiManager = uiManager != null ? uiManager : GetComponent<KitchenUIHandler>();

        uiManager.BindData(GameData);
        StartPhase();
    }

    public void StartPhase()
    {
        dayTimer.OnTimerStop += EndPhase;
        dayTimer.OnLoop += (loopCount) => GameData.Seconds--;

        orderManager.OnOrderCompleted += (order, score) =>
        {
            GameData.OrdersCompleted++;
            GameData.TotalEarnings = score;
        };
        orderManager.OnOrderRemoved += (order) => GameData.OrdersFailed++;

        AudioManager.Instance.PlayMusic(dayData.Music);
        orderManager.StartOrders();
        dayTimer.Start();

        OnPhaseStart?.Invoke();
    }

    public void EndPhase()
    {
        dayTimer.OnLoop -= (loopCount) => GameData.Seconds--;

        orderManager.OnOrderCompleted -= (order, score) =>
        {
            GameData.OrdersCompleted++;
            GameData.AddEarnings(score);
        };
        orderManager.OnOrderRemoved -= (order) => GameData.OrdersFailed++;

        orderManager.EndOrders();
        var timer = new CountDownTimer(CooldownTime);
        timer.OnTimerStop += () => OnPhaseEnd?.Invoke(GameData);

        AudioManager.Instance.StopMusic(5f);
        AudioManager.Instance.PlaySFXUI(alarmSound);
        timer.Start();
    }
}

public interface IPhaseManager
{
    public event Action OnPhaseStart;
    public void StartPhase();
    public void EndPhase();
}
public interface IDayPhaseManager : IPhaseManager
{
    public event Action<KitchenDataModel> OnPhaseEnd;
}
public interface INightPhaseManager : IPhaseManager
{
    public event Action<NightDataModel> OnPhaseEnd;
}