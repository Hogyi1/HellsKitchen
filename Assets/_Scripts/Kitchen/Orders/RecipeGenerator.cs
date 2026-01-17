using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeGenerator
{
    private RecipeSO activeRecipe;
    private float minTime;
    private float maxTime;

    public RecipeGenerator(RecipeSO activeRecipe)
    {
        this.activeRecipe = activeRecipe;

        CalculateRandomTime();
    }

    public Recipe GenerateRecipe()
    {
        if (activeRecipe == null)
            return null;

        int ingredientCount = 0;

        float expirationTime = activeRecipe.AveragePrepareTime + UnityEngine.Random.Range(-minTime, maxTime);
        Recipe newRecipe = new Recipe(expirationTime);

        Dictionary<KitchenObjectSO, int> ingredientCounts = new Dictionary<KitchenObjectSO, int>();
        foreach (var ingredient in activeRecipe.IngredientsToChooseFrom)
            ingredientCounts[ingredient.KitchenObjectSO] = ingredient.Quantity;

        newRecipe.AddIngredient(activeRecipe.MainIngredient);

        if (ingredientCounts.ContainsKey(activeRecipe.MainIngredient.KitchenObjectSO))
            ingredientCounts.Remove(activeRecipe.MainIngredient.KitchenObjectSO);

        foreach (var ingredients in ingredientCounts)
        {
            float chance = Random.Range(0f, 1f);
            if (chance <= 0.65f)
            {
                int count = Random.Range(1, ingredients.Value + 1);
                newRecipe.AddIngredient(ingredients.Key, count);
                ingredientCount += count;
            }
        }

        newRecipe.AddTime(ingredientCount * 2f);
        return newRecipe;
    }

    private void CalculateRandomTime()
    {
        minTime = activeRecipe.AveragePrepareTime * 0.15f;
        maxTime = activeRecipe.AveragePrepareTime * 0.25f;
    }
}