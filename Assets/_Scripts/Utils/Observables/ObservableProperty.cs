using UnityEngine.Events;

public class ObservableProperty<T> : IObservable<T>
{
    private T _value;

    public event UnityAction<T> OnValueChanged = delegate { };
    public T Value
    {
        get { return _value; }
        set
        {
            _value = value;
            OnValueChanged?.Invoke(_value);
        }
    }

    public T Get() => _value;
    public void Set(T newValue)
    {
        _value = newValue;
        OnValueChanged?.Invoke(_value);
    }
}