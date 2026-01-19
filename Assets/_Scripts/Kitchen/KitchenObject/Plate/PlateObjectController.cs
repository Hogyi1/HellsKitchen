using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlateObjectView))]
public class PlateObjectController : KitchenObjectController, IDisposable
{
    private PlateObjectModel PlateModel => model as PlateObjectModel;

    public bool IsEmpty() => PlateModel.IsEmpty();

    public bool CanAddIngredient(KitchenObjectController kitchenObject)
    {
        var so = kitchenObject.GetKitchenObjectSO();
        return PlateModel.CanAddIngredient(so);
    }

    public void AddIngredient(KitchenObjectController kitchenObject)
    {
        var kso = kitchenObject.GetKitchenObjectSO();
        if (PlateModel.AddIngredient(kso)) // Model now handles the logic and returns success
        {
            kitchenObject.DestroySelf(); // Controller handles Unity-specific action
        }
    }

    public void RemoveIngredient(KitchenObjectSO kitchenObject, int count)
    {
        PlateModel.RemoveIngredient(kitchenObject, count);
    }

    public override void Dispose()
    {
        if (PlateModel.HasIngredients())
        {
            var keys = PlateModel.IngredientDict.Keys.ToList();
            foreach (var key in keys)
            {
                PlateModel.RemoveIngredient(key, PlateModel.IngredientDict[key]);
            }
        }
        else
        {
            DestroySelf();
        }
    }

    public override void Hold() { }

    public override void Initialize()
    {
        model = new PlateObjectModel(so, KitchenSODatabase.GetCurrentRecipe());
        Interactions.Add(new PlaceOntoPlateAction(this));
    }

    public Dictionary<KitchenObjectSO, int> GetIngredientDictionary() => PlateModel.IngredientDict;
}
