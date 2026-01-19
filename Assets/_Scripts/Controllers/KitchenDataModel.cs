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

    private int _hours;
    private int _minutes;
    private int _ordersCompleted;
    private int _ordersFailed;
    private int _totalEarnings;

    [CreateProperty]
    public int Hours
    {
        get => _hours;
        set
        {
            if (_hours == value)
                return;
            _hours = value;
            Notify();
        }
    }

    [CreateProperty]
    public int Minutes
    {
        get => _minutes;
        set
        {
            if (_minutes == value)
                return;
            _minutes = value;
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
    public int TotalEarnings
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

    /// <summary>
    /// Notifies listeners that a specific property has changed.
    /// </summary>
    /// <param name="property">The name of the property that changed.</param>
    void Notify([CallerMemberName] string property = null)
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }
}