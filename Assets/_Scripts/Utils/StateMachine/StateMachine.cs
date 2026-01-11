using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEngine.Events;

// based on git-amend videos
/// <summary>
/// Manages and transitions between different states in a state machine pattern.
/// </summary>
public class StateMachine
{
    private StateNode currentNode;
    /// <summary>
    /// Event triggered when the state machine transitions to a new state.
    /// </summary>
    public event UnityAction<IState, IState> StateChanged = delegate { };
    readonly Dictionary<Type, StateNode> nodes = new(); // Every single state with its transitions
    readonly HashSet<ITransition> mainTransitions = new(); // Can transition at any given time, has the most priority

    /// <summary>
    /// Gets the currently active state.
    /// </summary>
    public IState CurrentState => currentNode.State;
    /// <summary>
    /// Updates the current state and handles transitions based on predicates.
    /// </summary>
    public void Update()
    {
        var transition = GetTransition();

        if (transition != null)
            ChangeState(transition.State());

        currentNode.State?.Update();
    }

    /// <summary>
    /// Calls FixedUpdate on the current state.
    /// </summary>
    public void FixedUpdate()
    {
        currentNode.State?.FixedUpdate();
    }

    /// <summary>
    /// Retrieves an existing node for a state or creates a new one if it doesn't exist.
    /// </summary>
    /// <param name="state">The state for which to get or add a node.</param>
    /// <returns>The corresponding StateNode.</returns>
    private StateNode GetOrAddNode(IState state)
    {
        var node = nodes.GetValueOrDefault(state.GetType());
        if (node == null)
        {
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }
        return node;
    }

    /// <summary>
    /// Adds a transition from one state to another, triggered by a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the predicate.</typeparam>
    /// <param name="from">The state to transition from.</param>
    /// <param name="to">The state to transition to.</param>
    /// <param name="predicate">The condition that triggers the transition.</param>
    public void AddTransition<T>(IState from, IState to, T predicate)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, predicate);
    }

    /// <summary>
    /// Adds a main transition that can occur from any state, with high priority.
    /// </summary>
    /// <typeparam name="T">The type of the predicate.</typeparam>
    /// <param name="to">The state to transition to.</param>
    /// <param name="predicate">The condition that triggers the transition.</param>
    public void AddMainTransition<T>(IState to, T predicate)
    {
        mainTransitions.Add(new Transition<T>(GetOrAddNode(to).State, predicate));
    }

    /// <summary>
    /// Immediately sets the current state of the state machine.
    /// </summary>
    /// <param name="state">The state to set as current.</param>
    public void SetState(IState state)
    {
        var node = GetOrAddNode(state);
        node.State.OnEnter();
        currentNode = node;
    }

    /// <summary>
    /// Changes the current state to the specified state and handles OnExit and OnEnter calls.
    /// </summary>
    /// <param name="to">The state to transition to.</param>
    private void ChangeState(IState to)
    {
        if (to == currentNode?.State)
            return;

        var previousState = currentNode.State;
        var newState = GetOrAddNode(to).State;

        StateChanged?.Invoke(previousState, newState);

        previousState?.OnExit();
        newState.OnEnter();
        currentNode = GetOrAddNode(to);
    }

    /// <summary>
    /// Checks for and returns the first valid transition, prioritizing main transitions.
    /// </summary>
    /// <returns>A valid transition if one is found, otherwise null.</returns>
    private ITransition GetTransition()
    {
        foreach (var transition in mainTransitions)
        {
            if (transition.Evaluate())
                return transition;

        }

        foreach (var transition in currentNode.Transitions)
        {
            if (transition.Evaluate())
                return transition;
        }

        return null;
    }

    /// <summary>
    /// Represents a node in the state machine, containing a state and its transitions.
    /// </summary>
    private class StateNode
    {
        /// <summary>
        /// The state associated with this node.
        /// </summary>
        public IState State { get; private set; }

        /// <summary>
        /// The set of transitions from this state.
        /// </summary>
        public HashSet<ITransition> Transitions { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateNode"/> class.
        /// </summary>
        /// <param name="state">The state for this node.</param>
        public StateNode(IState state)
        {
            this.State = state;
            Transitions = new();
        }

        /// <summary>
        /// Adds a transition from this node to another state.
        /// </summary>
        /// <typeparam name="T">The type of the predicate.</typeparam>
        /// <param name="to">The state to transition to.</param>
        /// <param name="predicate">The condition that triggers the transition.</param>
        public void AddTransition<T>(IState to, T predicate)
        {
            Transitions.Add(new Transition<T>(to, predicate));
        }
    }
}
