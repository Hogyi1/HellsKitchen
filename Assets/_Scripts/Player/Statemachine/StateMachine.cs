using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

// based on git-amend videos
public class StateMachine
{
    private StateNode currentNode;

    readonly Dictionary<Type, StateNode> nodes = new(); // Every single state with its transitions
    readonly HashSet<ITransition> mainTransitions = new(); // Can transition at any given time, has the most priority

    public IState CurrentState => currentNode.State;
    public void Update()
    {
        var transition = GetTransition();

        if (transition != null)
            ChangeState(transition.State());

        currentNode.State?.Update();
    }

    public void FixedUpdate()
    {
        currentNode.State?.FixedUpdate();
    }

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

    public void AddTransition<T>(IState from, IState to, T predicate)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, predicate);
    }

    public void AddMainTransition<T>(IState to, T predicate)
    {
        mainTransitions.Add(new Transition<T>(GetOrAddNode(to).State, predicate));
    }



    public void SetState(IState state)
    {
        var node = GetOrAddNode(state);
        node.State.OnEnter();
        currentNode = node;
    }

    private void ChangeState(IState to)
    {
        if (to == currentNode.State)
            return;

        var previousState = currentNode.State;
        var newState = GetOrAddNode(to).State;

        previousState?.OnExit();
        newState.OnEnter();
        currentNode = GetOrAddNode(to);
    }

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

    private class StateNode
    {
        public IState State { get; private set; }
        public HashSet<ITransition> Transitions { get; private set; }
        public StateNode(IState state)
        {
            this.State = state;
            Transitions = new();
        }

        public void AddTransition<T>(IState to, T predicate)
        {
            Transitions.Add(new Transition<T>(to, predicate));
        }
    }
}
