using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlateObject : KitchenObject, IDisposable
{
    public event Action<KitchenObjectSO> OnIngredientAdded = delegate { };
    public event Action<KitchenObjectSO> OnIngredientRemoved = delegate { };

    [SerializeField] RecipeSO recipeSO;
    private Dictionary<KitchenObjectSO, int> acceptedKitchenObjectSO = new();
    private Dictionary<KitchenObjectSO, int> ingredientDict = new();
    [SerializeField] List<KitchenObjectSO> ingredientList = new(); // Keep the original order

    private void Awake()
    {
        SetupAcceptedIngredients();

        Interactions.Add(new PlaceOntoPlateAction(this));
    }
    public bool IsEmpty() => ingredientList.Count == 0;
    private void SetupAcceptedIngredients()
    {
        acceptedKitchenObjectSO.Clear();

        for (int i = 0; i < recipeSO.IngredientsToChooseFrom.Length; ++i)
        {
            var ingredients = recipeSO.IngredientsToChooseFrom[i];
            var kitchenObjectSO = ingredients.KitchenObjectSO;
            int quantity = ingredients.Quantity;
            if (acceptedKitchenObjectSO.ContainsKey(kitchenObjectSO))
                acceptedKitchenObjectSO[kitchenObjectSO] += quantity;
            else
                acceptedKitchenObjectSO.Add(kitchenObjectSO, quantity);
        }
    }

    public List<KitchenObjectSO> GetIngredientList() => ingredientList;
    public Dictionary<KitchenObjectSO, int> GetIngredientDictionary() => ingredientDict;

    public bool CanAddIngredient(KitchenObject kitchenObject)
    {
        var so = kitchenObject.GetKitchenObjectSO();
        if (acceptedKitchenObjectSO.ContainsKey(so))
        {
            if (ingredientDict.ContainsKey(so))
                return ingredientDict[so] < acceptedKitchenObjectSO[so];
            else
                return true;
        }

        return false;
    }

    public void AddIngredient(KitchenObject kitchenObject)
    {
        if (CanAddIngredient(kitchenObject))
        {
            var kso = kitchenObject.GetKitchenObjectSO();
            kitchenObject.DestroySelf();

            if (ingredientDict.ContainsKey(kso))
                ingredientDict[kso]++;
            else
                ingredientDict.Add(kso, 1);

            ingredientList.Add(kso);
            OnIngredientAdded?.Invoke(kso);
        }
    }

    public void RemoveIngredient(KitchenObjectSO kitchenObject, int count)
    {
        if (ingredientDict[kitchenObject] - count > 0)
            ingredientDict[kitchenObject] = ingredientDict[kitchenObject] - count;
        else
            ingredientDict.Remove(kitchenObject);

        for (int i = 0; i < count; i++)
            ingredientList.Remove(kitchenObject);
        OnIngredientRemoved?.Invoke(kitchenObject);
    }

    public void Dispose()
    {
        if (ingredientDict.Count > 0)
        {
            var keys = ingredientDict.Keys.ToList();
            foreach (var key in keys)
            {
                RemoveIngredient(key, ingredientDict[key]);
            }
        }
        else
        {
            DestroySelf();
        }
    }

    private void OnValidate()
    {
        SetupAcceptedIngredients();
    }
}
