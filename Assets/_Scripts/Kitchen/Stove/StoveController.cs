using System;
using UnityEngine;
using UnityEngine.Events;
using static StoveModel;

/// <summary>
/// Manages the state and interactions of a stove counter, which can fry and burn items.
/// </summary>
public class StoveController : CounterController
{
    #region Variables
    private StateMachine stateMachine;
    private CountUpTimer _fryingTimer;
    private CountUpTimer _burnTimer;

    public event UnityAction<StoveState> OnStateChanged = delegate { };
    public StoveModel Model => GetModel<StoveModel>();

    #endregion

    #region Unity methods
    /// <summary>
    /// Initializes the state machine, timers, and sets up predicates on Awake.
    /// </summary>
    protected override void Initialize()
    {
        model = new StoveModel(StoveState.Idle);
        stateMachine = new StateMachine();

        _fryingTimer = new(1f);
        _burnTimer = new(1f);

        predicateList.Add(new EmptyAndEmptyPredicate(this));

        SetupStateMachine();
    }

    /// <summary>
    /// Updates the state machine every frame.
    /// </summary>
    private void Update() => stateMachine.Update();
    #endregion

    #region Public Methods
    /// <summary>
    /// Handles the player's interaction with the stove, delegating the logic to the current state.
    /// </summary>
    /// <param name="context">The player interacting with the counter.</param>
    /// <returns>The result of the interaction.</returns>
    public override InteractionResult TryInteract(PlayerController context)
    {
        bool canInteract = CanInteract(context);
        if (!canInteract)
            return InteractionResult.Fail("Cannot interact right now.");

        if (stateMachine.CurrentState is StoveBaseState state)
        {
            return state.TryInteract(context);
        }
        return InteractionResult.Fail("Stove is in an invalid state.");
    }

    /// <summary>
    /// Initializes the frying process with a new recipe.
    /// </summary>
    /// <param name="fryingRecipe">The recipe to start frying.</param>
    public void InitFrying(FryingRecipeSO fryingRecipe)
    {
        Model.CurrentRecipe = fryingRecipe;
        if (Model.CurrentRecipe != null)
        {
            _fryingTimer.Reset(Model.CurrentRecipe.fryingTime);
            _burnTimer.Reset(Model.CurrentRecipe.burningTime);
        }
        else
            ResetStove();
    }

    /// <summary>
    /// Resets the stove to its initial state, clearing the current recipe and stopping timers.
    /// </summary>
    public void ResetStove()
    {
        Model.CurrentRecipe = null;
        _fryingTimer.Stop();
        _burnTimer.Stop();
    }

    public void InvokeStateChange(StoveState state)
    {
        Model.CurrentState = state;
        OnStateChanged?.Invoke(Model.CurrentState);
    }

    #endregion

    #region State Machine
    /// <summary>
    /// Sets up the state machine with all possible states and transitions.
    /// </summary>
    private void SetupStateMachine()
    {
        var idleState = new IdleState(this);
        var fryingState = new FryingState(this, ref _fryingTimer);
        var friedState = new FriedState(this, ref _burnTimer);
        var burnedState = new BurnedState(this);

        At(idleState, fryingState, () => Model.CurrentRecipe != null);
        At(fryingState, friedState, () => Model.CurrentRecipe != null && _fryingTimer.IsFinished);
        At(friedState, burnedState, () => Model.CurrentRecipe != null && _burnTimer.IsFinished);
        At(friedState, fryingState, () => Model.CurrentRecipe != null && !_fryingTimer.IsFinished);
        At(burnedState, fryingState, () => Model.CurrentRecipe != null && !_fryingTimer.IsFinished);
        Any(idleState, () => Model.CurrentRecipe == null);

        stateMachine.SetState(idleState);
    }

    /// <summary>
    /// A shorthand method for adding a transition to the state machine.
    /// </summary>
    private void At(IState from, IState to, Func<bool> condition) => stateMachine.AddTransition(from, to, condition);

    /// <summary>
    /// A shorthand method for adding a high-priority transition that can occur from any state.
    /// </summary>
    private void Any(IState to, Func<bool> condition) => stateMachine.AddMainTransition(to, condition);
    #endregion
}