using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.UIElements;

public class NightDataModel : INotifyBindablePropertyChanged
{
    
    /// <summary>
    /// Event triggered when a bindable property on the cutting board changes, used for UI binding.
    /// </summary>
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged = delegate { };

    private int _seconds;
    private int _powerOutages;
    private int _robotsSpawned;

    public NightDataModel(float initialNightDurationInSeconds)
    {
        Seconds = (int)initialNightDurationInSeconds;
        PowerOutages = 0;
        RobotsSpawned = 0;
    }

    [CreateProperty]
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
}
