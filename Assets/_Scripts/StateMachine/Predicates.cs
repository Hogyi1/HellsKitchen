using System;
using UnityEngine;

public interface IPredicate
{
    public bool Evaluate();
}

public class FunctionPredicate : IPredicate
{
    private readonly Func<bool> predicate;

    public FunctionPredicate(Func<bool> predicate = null)
    {
        this.predicate = predicate == null ? () => false : predicate;
    }

    public bool Evaluate() => predicate();

}

public class ContextualPredicate<T> : IPredicate
{
    private readonly T context;
    private readonly Func<T, bool> predicate;

    public ContextualPredicate(T context, Func<T, bool> predicate)
    {
        this.context = context;
        this.predicate = predicate;
    }

    public bool Evaluate() => predicate(context);
}