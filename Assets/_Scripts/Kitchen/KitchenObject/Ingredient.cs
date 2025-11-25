using UnityEngine;

public class Ingredient : KitchenObject, IDisposable
{
    public bool CanBeCut => isCuttable && !isCut;
    public bool CanBeFried => isFryable && !isFried;

    bool isCut;
    bool isFried;
    [SerializeField] bool isCuttable = false;
    [SerializeField] bool isFryable = false;

    public void Dispose() => DestroySelf();
}
