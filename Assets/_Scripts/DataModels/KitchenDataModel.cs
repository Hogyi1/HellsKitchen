using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.UIElements;

public class KitchenDataModel : INotifyBindablePropertyChanged
{
    /// <summary>
    /// Event triggered when a bindable property on the cutting board changes, used for UI binding.
    /// </summary>
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged = delegate { };

    private int _seconds;
    private int _ordersCompleted;
    private int _ordersFailed;
    private float _totalEarnings;
    
    public KitchenDataModel(int Seconds)
    {
        this.Seconds = Seconds;
        OrdersCompleted = 0;
        OrdersFailed = 0;
        TotalEarnings = 0f;
    }

    [CreateProperty]
    public int Seconds
    {
        get => _seconds;
        set
        {
            if (_seconds == value)
                return;
            _seconds = value;

            Notify();
        }
    }

    [CreateProperty]
    public int OrdersCompleted
    {
        get => _ordersCompleted;
        set
        {
            if (_ordersCompleted == value)
                return;
            _ordersCompleted = value;
            Notify();
        }
    }

    [CreateProperty]
    public int OrdersFailed
    {
        get => _ordersFailed;
        set
        {
            if (_ordersFailed == value)
                return;
            _ordersFailed = value;
            Notify();
        }
    }

    [CreateProperty]
    public float TotalEarnings
    {
        get => _totalEarnings;
        set
        {
            if (_totalEarnings == value)
                return;
            _totalEarnings = value;
            Notify();
        }
    }

    public string GetFormattedTime()
    {
        int minutes = Seconds / 60;
        int seconds = Seconds % 60;
        return $"{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// Notifies listeners that a specific property has changed.
    /// </summary>
    /// <param name="property">The name of the property that changed.</param>
    void Notify([CallerMemberName] string property = null)
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }

    public void AddEarnings(float score) => TotalEarnings += score;
}