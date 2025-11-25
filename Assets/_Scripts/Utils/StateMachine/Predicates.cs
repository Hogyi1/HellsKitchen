using System;

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
    private T context;
    private readonly Func<T, bool> predicate;

    public ContextualPredicate(Func<T, bool> predicate)
    {
        this.predicate = predicate;
    }

    public void SetContext(T context) => this.context = context;
    public bool Evaluate() => predicate(context);
}
