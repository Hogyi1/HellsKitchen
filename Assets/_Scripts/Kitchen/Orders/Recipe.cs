using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Recipe
{
    public List<RecipeIngredient> ingredientList;
    public float expirationTime;

    public Recipe(float expirationTime)
    {
        ingredientList = new List<RecipeIngredient>();
        this.expirationTime = expirationTime;
    }

    public void AddTime(float extraTime)
    {
        expirationTime += extraTime;
    }

    public void AddIngredient(KitchenObjectSO ingredient, int quantity)
    {
        ingredientList.Add(new RecipeIngredient { KitchenObjectSO = ingredient, Quantity = quantity });
    }

    public void AddIngredient(RecipeIngredient recipeIngredient)
    {
        ingredientList.Add(recipeIngredient);
    }

    public float CalculateMatch(List<KitchenObjectSO> providedIngredients)
    {
        Dictionary<KitchenObjectSO, int> providedCounts = new Dictionary<KitchenObjectSO, int>();
        foreach (var pi in providedIngredients)
        {
            if (providedCounts.ContainsKey(pi))
            {
                providedCounts[pi]++;
            }
            else
            {
                providedCounts[pi] = 1;
            }
        }

        return CalculateMatch(providedCounts);
    }

    public float CalculateMatch(Dictionary<KitchenObjectSO, int> providedIngredients)
    {
        float matchScore = 0f;

        Dictionary<KitchenObjectSO, int> requiredCounts = new Dictionary<KitchenObjectSO, int>();
        int totalRequiredQuantity = 0;
        foreach (var ri in ingredientList)
        {
            requiredCounts[ri.KitchenObjectSO] = ri.Quantity;
            totalRequiredQuantity += ri.Quantity;
        }

        int totalMatchPoints = 0;
        int totalPenaltyPoints = 0;

        // --- Step 1: Evaluate each required ingredient ---
        foreach (var requiredEntry in requiredCounts)
        {
            KitchenObjectSO requiredType = requiredEntry.Key;
            int requiredQuantity = requiredEntry.Value;

            // Get the quantity of this ingredient actually provided
            providedIngredients.TryGetValue(requiredType, out int actualProvidedQuantity);

            if (actualProvidedQuantity == requiredQuantity)
            {
                // Perfect match for this ingredient type's quantity
                totalMatchPoints += requiredQuantity;
            }
            else if (actualProvidedQuantity < requiredQuantity)
            {
                // Missing ingredients: Award points for what was provided, penalize for missing.
                totalMatchPoints += actualProvidedQuantity;
                totalPenaltyPoints += (requiredQuantity - actualProvidedQuantity); // Penalize each missing item
            }
            else // actualProvidedQuantity > requiredQuantity
            {
                // Too many of this ingredient: Award points for the required amount, penalize for the excess.
                totalMatchPoints += requiredQuantity;
                totalPenaltyPoints += (actualProvidedQuantity - requiredQuantity); // Penalize each excess item
            }
        }

        // --- Step 2: Penalize for completely wrong/unneeded ingredients ---
        foreach (var providedEntry in providedIngredients)
        {
            // If this provided ingredient type was not required at all by the recipe
            if (!requiredCounts.ContainsKey(providedEntry.Key))
            {
                totalPenaltyPoints += providedEntry.Value; // Penalize every instance of this wrong ingredient
            }
        }

        // --- Step 3: Calculate final normalized score ---
        // If there are no required ingredients (empty recipe), return 0.
        if (totalRequiredQuantity == 0) return 0f;

        // The score is calculated as (points for correct items - points deducted for penalties)
        // divided by the maximum possible points (total required quantity).
        matchScore = (float)(totalMatchPoints - totalPenaltyPoints) / totalRequiredQuantity;

        // Ensure the final score is between 0 and 1.
        return Mathf.Clamp01(matchScore);
    }
}