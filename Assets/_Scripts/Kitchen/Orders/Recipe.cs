using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class Recipe : INotifyBindablePropertyChanged
{
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged = delegate { };

    private List<RecipeIngredient> _ingredientList;
    private float _expirationTime;
    private float _passedTime;
    private float _expirationProgress;
    public bool IsExpired => ExpirationTime <= PassedTime;

    [CreateProperty]
    public float ExpirationProgress
    {
        get => _expirationProgress;
        set
        {
            if (_expirationProgress == value)
                return;

            _expirationProgress = value;
            Notify();
        }
    }

    public float PassedTime
    {
        get => _passedTime;
        set
        {
            if (_passedTime == value)
                return;

            _passedTime = value;
            ExpirationProgress = 1f - (_expirationTime > 0f ? _passedTime / _expirationTime : 1f);
        }
    }

    public float ExpirationTime
    {
        get => _expirationTime;
        set
        {
            if (_expirationTime == value)
                return;

            _expirationTime = value;
        }
    }

    [CreateProperty]
    public List<RecipeIngredient> Ingredients
    {
        get => _ingredientList;
        set
        {
            if (_ingredientList == value)
                return;

            _ingredientList = value;
            Notify();
        }
    }

    public Recipe(float expirationTime)
    {
        _ingredientList = new List<RecipeIngredient>();
        this.ExpirationTime = expirationTime;
    }

    public void AddTime(float extraTime) => ExpirationTime += extraTime;
    public void AddIngredient(RecipeIngredient recipeIngredient)
    {
        int existingIndex = _ingredientList.FindIndex(t => t.KitchenObjectSO == recipeIngredient.KitchenObjectSO);

        if (existingIndex != -1)
            _ingredientList[existingIndex] = new RecipeIngredient
            {
                KitchenObjectSO = recipeIngredient.KitchenObjectSO,
                Quantity = _ingredientList[existingIndex].Quantity + recipeIngredient.Quantity
            };
        else
            _ingredientList.Add(recipeIngredient);
    }

    public void AddIngredient(KitchenObjectSO ingredient, int quantity)
    {
        var ri = new RecipeIngredient { KitchenObjectSO = ingredient, Quantity = quantity };
        AddIngredient(ri);
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
        foreach (var ri in _ingredientList)
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
    void Notify([CallerMemberName] string property = null)
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }
}