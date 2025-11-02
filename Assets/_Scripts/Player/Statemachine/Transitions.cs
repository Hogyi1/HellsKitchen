using Unity.VisualScripting;
using UnityEngine;
using System;


// based on git-amend videos
public interface ITransition
{
    public IState State();
    public bool Evaluate();
}

public class Transition<T> : ITransition
{
    public IState ToState { get; private set; }
    private readonly T predicate;

    public Transition(IState to, T predicate)
    {
        ToState = to;
        this.predicate = predicate;
    }

    public bool Evaluate()
    {
        switch (predicate)
        {
            case Func<bool> function:
                return function();
            case IPredicate pred:
                return pred.Evaluate();
            default:
                return false;
        }
    }

    public IState State() => ToState;
}


