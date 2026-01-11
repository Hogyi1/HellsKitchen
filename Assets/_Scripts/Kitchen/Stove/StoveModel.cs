using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine.UIElements;

[GeneratePropertyBag]
[Serializable]
public class StoveModel : CounterModel, INotifyBindablePropertyChanged
{
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged = delegate { };

    private float _fryingProgress;
    private float _burningProgress;
    private StoveState _currentState;
    private FryingRecipeSO _currentRecipe;

    public event Action<StoveState> OnStateChanged = delegate { };

    public StoveModel(StoveState state)
    {
        _currentState = state;
    }

    [CreateProperty]
    public float FryingProgress
    {
        get => _fryingProgress;
        set
        {
            if (_fryingProgress == value)
                return;

            _fryingProgress = value;
            Notify();
        }
    }

    [CreateProperty]
    public float BurningProgress
    {
        get => _burningProgress;
        set
        {
            if (_burningProgress == value)
                return;

            _burningProgress = value;
            Notify();
        }
    }

    [CreateProperty]
    public StoveState CurrentState
    {
        get => _currentState;
        set
        {
            if (_currentState == value)
                return;

            OnStateChanged.Invoke(value);
            _currentState = value;
            Notify();
        }
    }

    public FryingRecipeSO CurrentRecipe
    {
        get => _currentRecipe;
        set => _currentRecipe = value;
    }

    void Notify([CallerMemberName] string property = null)
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }

    public enum StoveState
    {
        Idle,
        Frying,
        Fried,
        Burnt
    }
}