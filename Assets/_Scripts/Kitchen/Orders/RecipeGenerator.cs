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
        Recipe newRecipe = new(expirationTime);

        Dictionary<KitchenObjectSO, int> ingredientCounts = new Dictionary<KitchenObjectSO, int>();
        foreach (var ingredient in activeRecipe.IngredientsToChooseFrom)
            ingredientCounts[ingredient.KitchenObjectSO] = ingredient.Quantity;

        foreach (var ing in activeRecipe.MainIngredient)
        {
            newRecipe.AddIngredient(ing);
            if (ingredientCounts.ContainsKey(ing.KitchenObjectSO))
            {
                ingredientCounts[ing.KitchenObjectSO] -= ing.Quantity;
                if (ingredientCounts[ing.KitchenObjectSO] <= 0)
                    ingredientCounts.Remove(ing.KitchenObjectSO);
                ingredientCount += ing.Quantity;
            }
        }

        foreach (var ingredients in ingredientCounts)
        {
            int remaining = activeRecipe.MaxIngredientCount - ingredientCount;
            if (remaining <= 0) break;

            float chance = Random.Range(0f, 1f);
            if (chance <= 0.65f)
            {
                int maxAdd = Mathf.Min(ingredients.Value, remaining);
                if (maxAdd <= 0) continue;

                int count = Random.Range(1, maxAdd + 1);

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