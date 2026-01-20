using System;
using UnityEngine;

[RequireComponent(typeof(KitchenUIHandler))]
public class DayPhaseController : MonoBehaviour
{
    public event Action OnDayStart = delegate { };
    public event Action<KitchenDataModel> OnDayEnd = delegate { };

    public DayData dayData;
    [SerializeField] private KitchenUIHandler uiManager;
    [SerializeField] private OrderManager orderManager;

    public KitchenDataModel GameData { get; private set; }

    private LoopTimer dayTimer;

    private void Start()
    {
        GameManager.Instance.RegisterDayPhaseManager(this);
        GameData = new KitchenDataModel(dayData.GetDayDurationInSeconds());
        dayTimer = new(1f, dayData.GetDayDurationInSeconds());

        orderManager = orderManager != null ? orderManager : FindAnyObjectByType<OrderManager>();
        uiManager = uiManager != null ? uiManager : GetComponent<KitchenUIHandler>();

        uiManager.BindData(GameData);
        StartDay();
    }

    public void StartDay()
    {
        dayTimer.OnTimerStop += EndDay;
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

        OnDayStart?.Invoke();
    }

    public void EndDay()
    {
        dayTimer.OnLoop -= (loopCount) => GameData.Seconds--;

        orderManager.OnOrderCompleted -= (order, score) =>
        {
            GameData.OrdersCompleted++;
            GameData.AddEarnings(score);
        };
        orderManager.OnOrderRemoved -= (order) => GameData.OrdersFailed++;

        OnDayEnd?.Invoke(GameData);
    }
}
