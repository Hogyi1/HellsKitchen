using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine.UIElements;

/// <summary>
/// Represents the model for a cutting board, managing the state, progress, and recipe
/// for cutting KitchenObjects. It notifies about cutting actions and completion.
/// </summary>
[GeneratePropertyBag]
public class CuttingModel : CounterModel, INotifyBindablePropertyChanged
{
    /// <summary>
    /// Event triggered when the cutting process is finished, providing the output KitchenObjectSO.
    /// </summary>
    public event Action<KitchenObjectSO> OnCuttingFinished = delegate { };
    /// <summary>
    /// Event triggered each time a cutting action is performed, providing the input KitchenObjectSO.
    /// </summary>
    public event Action<KitchenObjectSO> OnCuttingAction = delegate { };
    /// <summary>
    /// Event triggered when a bindable property on the cutting board changes, used for UI binding.
    /// </summary>
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged = delegate { };

    private CuttingRecipeSO _currentRecipe = null;

    private float _cuttingProgress = 0;
    private int _cuttingTimes = 0;
    private float _coolDown = 0f;

    /// <summary>
    /// Gets or sets the number of times the current item has been cut.
    /// Notifies property change listeners when set.
    /// </summary>
    [CreateProperty]
    public int CuttingTimes
    {
        get => _cuttingTimes;
        set
        {
            if (_cuttingTimes == value)
                return;
            _cuttingTimes = value;
            Notify();
        }
    }

    /// <summary>
    /// Gets or sets the current cutting progress (0.0 to 1.0).
    /// Notifies property change listeners when set.
    /// </summary>
    [CreateProperty]
    public float CuttingProgress
    {
        get => _cuttingProgress;
        set
        {
            if (_cuttingProgress == value)
                return;

            _cuttingProgress = value;
            Notify();
        }
    }

    /// <summary>
    /// Gets or sets the current cutting recipe being applied.
    /// </summary>
    public CuttingRecipeSO CurrentRecipe
    {
        get => _currentRecipe;
        set => _currentRecipe = value;
    }

    /// <summary>
    /// Calculates the current cutting progress based on the number of cuts and the recipe's required cuts.
    /// Returns 1f if there's no current recipe, indicating "done" or no active task.
    /// </summary>
    /// <returns>The cutting progress as a float between 0.0 and 1.0.</returns>
    public float CalculateProgress() => _currentRecipe != null ? (float)CuttingTimes / _currentRecipe.cuttingTimes : 1f;

    /// <summary>
    /// Initializes a new instance of the CuttingBoard model.
    /// </summary>
    /// <param name="cuttingTimer">The cooldown timer duration for cutting actions.</param>
    public CuttingModel(float cuttingTimer)
    {
        _coolDown = cuttingTimer;
    }

    /// <summary>
    /// Performs a single cutting action, increments cut count, starts the timer,
    /// updates progress, and invokes the OnCuttingAction event.
    /// </summary>
    public void OnCut()
    {
        CuttingTimes++;
        CuttingProgress = CalculateProgress();
        OnCuttingAction?.Invoke(GetChild().GetKitchenObjectSO());
    }

    /// <summary>
    /// Resets all cutting-related properties to their initial state, clearing progress and recipe.
    /// </summary>
    public void ResetCutting()
    {
        CuttingTimes = 0;
        CuttingProgress = 0f;
        CurrentRecipe = null;
    }

    /// <summary>
    /// Handles the completion of the cutting process, invokes the OnCuttingFinished event,
    /// and resets the current recipe.
    /// </summary>
    public void OnFinishedCutting()
    {
        OnCuttingFinished?.Invoke(_currentRecipe.Output);
        _currentRecipe = null;
        CuttingProgress = CalculateProgress(); // Update progress after finishing
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
