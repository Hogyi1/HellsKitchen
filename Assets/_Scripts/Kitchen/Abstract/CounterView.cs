using System;
using UnityEngine;

/// <summary>
/// Base class for all counter views, providing common functionality and structure.
/// Runs after Controller scripts to ensure proper initialization order.
/// </summary>
[DefaultExecutionOrder(50)]
public abstract class CounterView : MonoBehaviour
{
    /// <summary>
    /// Reference to the associated BaseCounter logic component.
    /// </summary>
    protected CounterModel _model;

    /// <summary>
    /// The Transform representing the top point of the counter where kitchen objects are usually placed.
    /// </summary>
    [SerializeField] protected Transform counterTop;
    /// <summary>
    /// A generic BaseVisual component used for displaying visual feedback, such as selection.
    /// </summary>
    [SerializeField] protected BaseVisual visual;

    /// <summary>
    /// Initializes references to the counterTop Transform and the BaseVisual component.
    /// If not assigned in the Inspector, it attempts to find them automatically.
    /// </summary>
    private void Awake()
    {
        counterTop = counterTop != null ? counterTop : transform.Find("CounterTop");
        visual = visual != null ? visual : GetComponentInChildren<BaseVisual>();

        SetupComponents();
    }

    /// <summary>
    /// Initializes the counter view by calling the abstract Initialize method.
    /// </summary>
    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Gets the Transform representing the position where kitchen objects should be placed on this counter.
    /// </summary>
    /// <returns>The Transform of the counter's top point.</returns>
    public Transform GetTransformPosition() => counterTop;

    /// <summary>
    /// Activates the generic visual indicator associated with this counter.
    /// </summary>
    public void TurnOnVisual() => visual.Show();
    /// <summary>
    /// Deactivates the generic visual indicator associated with this counter.
    /// </summary>
    public void TurnOffVisual() => visual.Hide();

    /// <summary>
    /// Binds the specified counter model to the current instance.
    /// </summary>
    /// <remarks>Use this method to update the internal model reference for the instance. Subsequent
    /// operations may use the bound model for their logic.</remarks>
    /// <param name="counter">The counter model to associate with this instance. The value is passed by readonly reference and cannot be null.</param>
    public void BindModel(in CounterModel counter) => this._model = counter;

    /// <summary>
    /// Called inside Start to initialize the view with necessary bindings and setup.
    /// </summary>
    protected virtual void Initialize() { }

    /// <summary>
    /// Called inside Awake to set up necessary components.
    /// </summary>
    protected virtual void SetupComponents() { }

    public T GetModel<T>() where T : CounterModel => _model as T;
}