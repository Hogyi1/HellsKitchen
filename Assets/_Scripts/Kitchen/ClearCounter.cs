using UnityEngine;

public class ClearCounter : BaseCounter
{
    private void Awake()
    {
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
    }
}