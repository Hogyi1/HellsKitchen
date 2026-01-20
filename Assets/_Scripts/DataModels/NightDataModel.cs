using System;
using System.Runtime.CompilerServices;
using UnityEngine.UIElements;

public class NightDataModel : INotifyBindablePropertyChanged
{
    /// <summary>
    /// Event triggered when a bindable property on the cutting board changes, used for UI binding.
    /// </summary>
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged = delegate { };

    /// <summary>
    /// Notifies listeners that a specific property has changed.
    /// </summary>
    /// <param name="property">The name of the property that changed.</param>
    void Notify([CallerMemberName] string property = null)
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }

    private int _seconds;
    public int Seconds
    {
        get => _seconds;
        set
        {
            if (_seconds != value)
            {
                _seconds = value;
                Notify();
            }
        }
    }

    private int _powerOutages;
    public int PowerOutages
    {
        get => _powerOutages;
        set
        {
            if (_powerOutages != value)
            {
                _powerOutages = value;
                Notify();
            }
        }
    }

    private int _robotsSpawned;
    public int RobotsSpawned
    {
        get => _robotsSpawned;
        set
        {
            if (_robotsSpawned != value)
            {
                _robotsSpawned = value;
                Notify();
            }
        }
    }

    public NightDataModel(float initialNightDurationInSeconds)
    {
        Seconds = (int)initialNightDurationInSeconds;
        PowerOutages = 0;
        RobotsSpawned = 0;
    }
}
