using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class KitchenSODatabase
{
    public static List<CuttingRecipeSO> CuttingRecipes = new();
    public static List<FryingRecipeSO> FryingRecipes = new();
    public static List<KitchenObjectSO> KitchenObjects = new();
    public static List<RecipeSO> Recipes = new();
    public static RecipeName CurrentRecipeName = RecipeName.None;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Initialize()
    {
        LoadRecipes();
    }

    /// <summary>
    /// Gets the cutting recipe for a given kitchen object.
    /// </summary>
    /// <param name="kitchenObject">The kitchen object to check.</param>
    /// <returns>The cutting recipe, or null if one doesn't exist.</returns>
    public static CuttingRecipeSO GetCuttingRecipeWithInput(KitchenObjectController kitchenObject)
    {
        if (kitchenObject == null) return null;
        return CuttingRecipes.FirstOrDefault(t => t.Input == kitchenObject.GetKitchenObjectSO());
    }

    /// <summary>
    /// Finds a frying recipe that corresponds to the given kitchen object.
    /// </summary>
    /// <param name="kitchenObject">The kitchen object to check.</param>
    /// <returns>The corresponding FryingRecipeSO, or null if no recipe is found.</returns>
    public static FryingRecipeSO GetFryingRecipeWithInput(KitchenObjectController kitchenObject)
    {
        if (kitchenObject == null) return null;
        return FryingRecipes.FirstOrDefault(t => t.from == kitchenObject.GetKitchenObjectSO());
    }

    /// <summary>
    /// Retrieves the recipe associated with the specified recipe name.
    /// </summary>
    /// <param name="recipeName">The name of the recipe to retrieve. Specify a value other than RecipeName.None to search for a valid recipe.</param>
    /// <returns>A RecipeSO object representing the recipe with the specified name, or null if no matching recipe is found or if
    /// recipeName is RecipeName.None.</returns>
    public static RecipeSO GetRecipeByName(RecipeName recipeName)
    {
        if (recipeName == RecipeName.None) return null;
        return Recipes.FirstOrDefault(r => r.RecipeName == recipeName);
    }

    /// <summary>
    /// Sets the current recipe name to the specified value.
    /// </summary>
    /// <param name="recipeName">The recipe name to assign as the current recipe. Must be a valid <see cref="RecipeName"/> value.</param>
    public static void SetRecipeName(RecipeName recipeName)
    {
        CurrentRecipeName = recipeName;
    }

    /// <summary>
    /// Retrieves the current recipe based on the active recipe name.
    /// </summary>
    /// <returns>A <see cref="RecipeSO"/> instance representing the current recipe. Returns <see langword="null"/> if no recipe
    /// matches the current name.</returns>
    public static RecipeSO GetCurrentRecipe()
    {
        return GetRecipeByName(CurrentRecipeName);
    }

    /// <summary>
    /// Loads every recipe from the Resources folder into their respective lists.
    /// </summary>
    public static void LoadRecipes()
    {
        CuttingRecipes = Resources.LoadAll<CuttingRecipeSO>("ScriptableObjects/CuttingRecipes").ToList();
        FryingRecipes = Resources.LoadAll<FryingRecipeSO>("ScriptableObjects/FryingRecipes").ToList();
        KitchenObjects = Resources.LoadAll<KitchenObjectSO>("ScriptableObjects/KitchenObjects").ToList();
        Recipes = Resources.LoadAll<RecipeSO>("ScriptableObjects/Recipes").ToList();
    }
}