using UnityEngine.Events;

public interface IObservable<T>
{
    event UnityAction<T> OnValueChanged;
}