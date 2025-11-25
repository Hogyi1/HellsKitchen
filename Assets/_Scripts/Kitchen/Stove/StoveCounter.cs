using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the state and interactions of a stove counter, which can fry and burn items.
/// </summary>
public class StoveCounter : BaseCounter
{
    #region Events
    /// <summary>
    /// Event triggered when the stove's state changes.
    /// </summary>
    public event UnityAction<StoveBaseState> OnStateChanged = delegate { };
    #endregion

    #region Fields
    [Header("Configuration")]
    [Tooltip("List of all frying recipes this stove can handle.")]
    [SerializeField] List<FryingRecipeSO> fryingRecipes = new();

    private StateMachine stateMachine;
    private CountUpTimer fryingTimer;
    private CountUpTimer burnTimer;
    #endregion

    #region Properties
    /// <summary>
    /// Gets the currently active frying recipe. Will be null if no item is being cooked.
    /// </summary>
    public FryingRecipeSO currentRecipe { get; private set; }
    #endregion

    #region Unity methods
    /// <summary>
    /// Initializes the state machine, timers, and sets up predicates on Awake.
    /// </summary>
    private void Awake()
    {
        stateMachine = new StateMachine();
        fryingTimer = new(1f);
        burnTimer = new(1f);
        counterTop = counterTop != null ? counterTop : transform.Find("CounterTop");

        var emptyAndEmpty = new ContextualPredicate<PlayerController>((context) =>
        {
            if (context.TryGetKitchenObject() == null && !HasChild())
            {
                return false;
            }
            return true;
        });
        predicateList.Add(emptyAndEmpty);

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
    /// Finds a frying recipe that corresponds to the given kitchen object.
    /// </summary>
    /// <param name="kitchenObject">The kitchen object to check.</param>
    /// <returns>The corresponding FryingRecipeSO, or null if no recipe is found.</returns>
    public FryingRecipeSO GetFryingRecipe(KitchenObject kitchenObject)
    {
        if (kitchenObject == null) return null;
        return fryingRecipes.FirstOrDefault(t => t.from == kitchenObject.GetKitchenObjectSO());
    }

    /// <summary>
    /// Initializes the frying process with a new recipe.
    /// </summary>
    /// <param name="fryingRecipe">The recipe to start frying.</param>
    public void InitFrying(FryingRecipeSO fryingRecipe)
    {
        currentRecipe = fryingRecipe;
        if (currentRecipe != null)
        {
            fryingTimer.Reset(currentRecipe.fryingTime);
            burnTimer.Reset(currentRecipe.burningTime);
        }
        else
        {
            ResetStove();
        }
    }

    /// <summary>
    /// Resets the stove to its initial state, clearing the current recipe and stopping timers.
    /// </summary>
    public void ResetStove()
    {
        currentRecipe = null;
        fryingTimer.Stop();
        burnTimer.Stop();
    }
    #endregion

    #region State Machine
    /// <summary>
    /// Sets up the state machine with all possible states and transitions.
    /// </summary>
    private void SetupStateMachine()
    {
        var idleState = new IdleState(this);
        var fryingState = new FryingState(this, ref fryingTimer);
        var friedState = new FriedState(this, ref burnTimer);
        var burnedState = new BurnedState(this);

        At(idleState, fryingState, () => currentRecipe != null);
        At(fryingState, friedState, () => currentRecipe != null && fryingTimer.IsFinished);
        At(friedState, burnedState, () => currentRecipe != null && burnTimer.IsFinished);
        At(friedState, fryingState, () => currentRecipe != null && !fryingTimer.IsFinished);
        At(burnedState, fryingState, () => currentRecipe != null && !fryingTimer.IsFinished);
        Any(idleState, () => currentRecipe == null);

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