using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CuttingBoard : BaseCounter, ICuttingTable
{
    int cuttingTimes;
    CuttingRecipeSO currentRecipe;

    public event UnityAction<float> OnCuttingAction = delegate { };
    [SerializeField] List<CuttingRecipeSO> cuttingRecipes = new();

    public float Progress() => currentRecipe != null ? (float)cuttingTimes / (float)currentRecipe.cuttingTimes : 1f;
    public bool IsDone() => currentRecipe == null && HasChild();

    public override void OnPlace(KitchenObject other) => InitCutting(GetCuttingRecipe(other));
    public override bool CanRelease() => IsDone();
    public override bool CanPlace(KitchenObject other) => GetCuttingRecipe(other) != null;

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

    /// <summary>
    /// Initializes the cutting process for the item on the board.
    /// </summary>
    private void InitCutting(CuttingRecipeSO cuttingRecipeSO)
    {
        cuttingTimes = 0;
        currentRecipe = cuttingRecipeSO;
    }

    /// <summary>
    /// Gets the cutting recipe for a given kitchen object.
    /// </summary>
    /// <param name="kitchenObject">The kitchen object to check.</param>
    /// <returns>The cutting recipe, or null if one doesn't exist.</returns>
    private CuttingRecipeSO GetCuttingRecipe(KitchenObject kitchenObject)
    {
        if (kitchenObject == null) return null;
        return cuttingRecipes.FirstOrDefault(t => t.from == kitchenObject.GetKitchenObjectSO());
    }

    /// <summary>
    /// Performs a cutting action on the item on the board.
    /// </summary>
    public void Cut()
    {
        if (currentRecipe == null)
            return;

        cuttingTimes++;
        OnCuttingAction.Invoke(Progress());

        if (cuttingTimes >= currentRecipe.cuttingTimes)
        {
            GetChild().DestroySelf();
            KitchenObject.SpawnVisual(currentRecipe.to, this);
            currentRecipe = null;
        }
    }
}

public interface IProgress
{
    bool IsDone();
    float Progress();
}

public interface ICuttingTable : IProgress
{
    void Cut();
}