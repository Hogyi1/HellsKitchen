using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeGenerator : MonoBehaviour
{
    [SerializeField] List<RecipeSO> Recipes = new();
    RecipeSO activeRecipe;

    private void Awake()
    {
        if (activeRecipe == null) SetRecipeTemplate(RecipeName.Default);
    }

    public Recipe GenerateRecipe()
    {
        if (activeRecipe == null)
            return null;

        float expirationTime = activeRecipe.AveragePrepareTime + UnityEngine.Random.Range(-5, 6);
        Recipe newRecipe = new Recipe(expirationTime);

        Dictionary<KitchenObjectSO, int> ingredientCounts = new Dictionary<KitchenObjectSO, int>();
        foreach (var ingredient in activeRecipe.IngredientsToChooseFrom)
            ingredientCounts[ingredient.KitchenObjectSO] = ingredient.Quantity;

        newRecipe.AddIngredient(activeRecipe.MainIngredient);

        if (ingredientCounts.ContainsKey(activeRecipe.MainIngredient.KitchenObjectSO))
            ingredientCounts.Remove(activeRecipe.MainIngredient.KitchenObjectSO);

        foreach (var ingredients in ingredientCounts)
        {
            float chance = UnityEngine.Random.Range(0f, 1f);
            if (chance <= 0.5f)
            {
                int count = UnityEngine.Random.Range(1, ingredients.Value + 1);
                newRecipe.AddIngredient(ingredients.Key, count);
            }
        }

        return newRecipe;
    }

    public void SetRecipeTemplate(RecipeName activeRecipeTemplate)
    {
        if (activeRecipeTemplate == RecipeName.Default)
            activeRecipe = Recipes.First();
        else
            activeRecipe = Recipes.FirstOrDefault(r => r.RecipeName == activeRecipeTemplate);
    }
}