using System.Collections.Generic;
using System.Linq;

public class KitchenSODatabase : Singleton<KitchenSODatabase>
{
    public List<CuttingRecipeSO> CuttingRecipes = new();
    public List<FryingRecipeSO> FryingRecipes = new();
    public List<KitchenObjectSO> KitchenObjects = new();

    /// <summary>
    /// Gets the cutting recipe for a given kitchen object.
    /// </summary>
    /// <param name="kitchenObject">The kitchen object to check.</param>
    /// <returns>The cutting recipe, or null if one doesn't exist.</returns>
    public CuttingRecipeSO GetCuttingRecipeWithInput(KitchenObject kitchenObject)
    {
        if (kitchenObject == null) return null;
        return CuttingRecipes.FirstOrDefault(t => t.Input == kitchenObject.GetKitchenObjectSO());
    }

    /// <summary>
    /// Finds a frying recipe that corresponds to the given kitchen object.
    /// </summary>
    /// <param name="kitchenObject">The kitchen object to check.</param>
    /// <returns>The corresponding FryingRecipeSO, or null if no recipe is found.</returns>
    public FryingRecipeSO GetFryingRecipeWithInput(KitchenObject kitchenObject)
    {
        if (kitchenObject == null) return null;
        return FryingRecipes.FirstOrDefault(t => t.from == kitchenObject.GetKitchenObjectSO());
    }
}