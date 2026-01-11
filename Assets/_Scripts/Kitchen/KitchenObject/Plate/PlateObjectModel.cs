using System;
using System.Collections.Generic;

public class PlateObjectModel : KitchenObjectModel
{
    public Dictionary<KitchenObjectSO, int> AcceptedKitchenObjectSO = new();
    public Dictionary<KitchenObjectSO, int> IngredientDict = new();
    public List<KitchenObjectSO> IngredientList = new();

    public RecipeSO RecipeSO;

    public event Action<KitchenObjectSO> OnIngredientAdded = delegate { };
    public event Action<KitchenObjectSO> OnIngredientRemoved = delegate { };


    public PlateObjectModel(KitchenObjectSO kitchenObjectSo, RecipeSO recipeSO) : base(kitchenObjectSo)
    {
        SetupAcceptedIngredients(recipeSO);
        this.RecipeSO = recipeSO;
    }

    private void SetupAcceptedIngredients(RecipeSO recipeSO)
    {
        AcceptedKitchenObjectSO.Clear();

        for (int i = 0; i < recipeSO.IngredientsToChooseFrom.Length; ++i)
        {
            var ingredients = recipeSO.IngredientsToChooseFrom[i];
            var kitchenObjectSO = ingredients.KitchenObjectSO;
            int quantity = ingredients.Quantity;
            if (AcceptedKitchenObjectSO.ContainsKey(kitchenObjectSO))
                AcceptedKitchenObjectSO[kitchenObjectSO] += quantity;
            else
                AcceptedKitchenObjectSO.Add(kitchenObjectSO, quantity);
        }
    }

    public List<KitchenObjectSO> GetIngredientList() => IngredientList;
    public bool IsEmpty() => IngredientList.Count == 0;
    public bool HasIngredients() => IngredientDict.Count > 0;
    public bool CanAddIngredient(KitchenObjectSO so)
    {
        if (AcceptedKitchenObjectSO.ContainsKey(so))
        {
            if (IngredientDict.ContainsKey(so))
                return IngredientDict[so] < AcceptedKitchenObjectSO[so];
            else
                return true;
        }

        return false;
    }

    public bool AddIngredient(KitchenObjectSO kso)
    {
        if (CanAddIngredient(kso))
        {
            if (IngredientDict.ContainsKey(kso))
                IngredientDict[kso]++;
            else
                IngredientDict.Add(kso, 1);

            IngredientList.Add(kso);
            OnIngredientAdded?.Invoke(kso);
            return true;
        }
        return false;
    }

    public void RemoveIngredient(KitchenObjectSO kitchenObject, int count)
    {
        if (IngredientDict.ContainsKey(kitchenObject))
        {
            if (IngredientDict[kitchenObject] - count > 0)
                IngredientDict[kitchenObject] -= count;
            else
                IngredientDict.Remove(kitchenObject);

            for (int i = 0; i < count; i++)
                IngredientList.Remove(kitchenObject);
            OnIngredientRemoved?.Invoke(kitchenObject);
        }
    }
}