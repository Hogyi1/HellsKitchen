using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Abstract base class for MonoBehaviour-based counter controllers.
/// This class orchestrates interactions using a strategy pattern, manages predicates for interaction validity,
/// and binds a generic model (derived from BaseCounter) to its associated view.
/// </summary>
/// <typeparam name="T">The specific type of BaseCounter that this controller manages as its model.</typeparam>
[DefaultExecutionOrder(45)]
public abstract class CounterController : MonoBehaviour, IInteractable, IObjectParent<KitchenObjectController>
{
    /// <summary>
    /// The instance of the specific BaseCounter (acting as the model) managed by this controller.
    /// </summary>
    protected CounterModel model;

    /// <summary>
    /// The visual component associated with this controller, responsible for displaying the counter's state
    /// and bound to the managed model.
    /// </summary>
    [SerializeField] protected CounterView view;

    /// <summary>
    /// A list of predicates (conditions) that must all evaluate to true for an interaction to be considered valid.
    /// </summary>
    protected List<IPredicate> predicateList = new();

    /// <summary>
    /// A list of interaction strategies. When an interaction occurs, the controller iterates through these
    /// to find the first strategy that can execute the current interaction, then executes it.
    /// </summary>
    [SerializeField] protected List<BaseCounterStrategy> interactionStrategies = new();

    /// <summary>
    /// Initializes the controller by setting up the view reference.
    /// </summary>
    private void Awake()
    {
        view = view != null ? view : GetComponentInChildren<CounterView>();
        if (view.IsUnityNull())
            Debug.LogError($"CounterView not found in {gameObject.name} or its children.");

        SetupComponents();
    }

    /// <summary>
    /// Initializes the controller by setting up the view reference, calling the abstract Initialize method,
    /// and binding the managed model to the view.
    /// </summary>
    private void Start()
    {
        Initialize();
        view.BindModel(model);
    }

    /// <summary>
    /// Attempts to perform an interaction with this counter.
    /// It first checks if the interaction is valid using predicates, then delegates the execution
    /// to the first suitable interaction strategy found.
    /// </summary>
    /// <param name="context">The PlayerController initiating the interaction.</param>
    /// <returns>An InteractionResult indicating success or failure, along with a message.</returns>
    public virtual InteractionResult TryInteract(PlayerController context)
    {
        bool canInteract = CanInteract(context);
        if (!canInteract)
            return InteractionResult.Fail("Cannot interact right now");

        foreach (var strategy in interactionStrategies)
        {
            if (strategy.CanExecute(context, this))
            {
                return strategy.Execute(context, this);
            }
        }

        return InteractionResult.Fail("No valid strategy");
    }

    /// <summary>
    /// Determines if an interaction with this counter is currently possible based on its registered predicates.
    /// </summary>
    /// <param name="context">The PlayerController initiating the interaction, used for contextual predicates.</param>
    /// <returns>True if all predicates evaluate to true, otherwise false.</returns>
    public virtual bool CanInteract(PlayerController context)
    {
        foreach (var predicate in predicateList)
        {
            if (predicate is ContextualPredicate<PlayerController> contPred)
                contPred.SetContext(context);

            if (!predicate.Evaluate())
                return false;
        }
        return true;
    }

    /// <summary>
    /// Provides access to the specific BaseCounter instance (model) managed by this controller.
    /// </summary>
    /// <returns>The managed BaseCounter instance.</returns>
    public CounterModel GetModel() => model;

    /// <summary>
    /// Abstract method that must be implemented by derived classes to perform specific
    /// initialization logic for their respective models. Initi is called inside Start method.
    /// </summary>
    protected abstract void Initialize();

    /// <summary>
    /// Retrieves the underlying model as the specified type.
    /// </summary>
    /// <typeparam name="T">The type to which the model is cast. Must inherit from CounterModel.</typeparam>
    /// <returns>The model cast to type T, or null if the model is not of type T.</returns>
    public T GetModel<T>() where T : CounterModel => model as T;

    /// <summary>
    /// Called inside Awake to set up necessary components.
    /// </summary>
    protected virtual void SetupComponents() { }

    /// <summary>
    /// Gets the current transform representing the position and orientation of the view.
    /// </summary>
    /// <returns>A <see cref="Transform"/> object that describes the view's current position and orientation.</returns>
    public Transform GetTransform() => view.GetTransformPosition();

    /// <summary>
    /// Assigns the specified <see cref="KitchenObjectController"/> as the child of the managed model.
    /// This delegates to <see cref="CounterModel.SetChild(KitchenObjectController)"/>.
    /// </summary>
    /// <param name="child">The <see cref="KitchenObjectController"/> to set as the child.</param>
    public void SetChild(KitchenObjectController child) => model.SetChild(child);

    /// <summary>
    /// Clears any child currently assigned to the managed model.
    /// This delegates to <see cref="CounterModel.ClearChild"/>.
    /// </summary>
    public void ClearChild() => model.ClearChild();

    /// <summary>
    /// Gets the current <see cref="KitchenObjectController"/> child assigned to the managed model.
    /// </summary>
    /// <returns>The current <see cref="KitchenObjectController"/> child, or <c>null</c> if none is assigned.</returns>
    public KitchenObjectController GetChild() => model.GetChild();

    /// <summary>
    /// Determines whether the managed model currently has a child assigned.
    /// </summary>
    /// <returns><c>true</c> if a child is assigned; otherwise, <c>false</c>.</returns>
    public bool HasChild() => model.HasChild();

    /// <summary>
    /// Assigns the specified <see cref="IObjectChild"/> as the child of the managed model.
    /// This overload allows assigning children via the interface and delegates to the model.
    /// </summary>
    /// <param name="child">The <see cref="IObjectChild"/> to set as the child.</param>
    public void SetChild(IObjectChild child) => SetChild(child as KitchenObjectController);

    /// <summary>
    /// Explicit interface implementation that returns the current child as an <see cref="IObjectChild"/>.
    /// Delegates to <see cref="GetChild"/> and allows consumers using the non-generic
    /// <c>IObjectParent</c> interface to obtain the child reference.
    /// </summary>
    /// <returns>The current child as an <see cref="IObjectChild"/>, or <c>null</c> if none is assigned.</returns>
    IObjectChild IObjectParent.GetChild() => GetChild();
}