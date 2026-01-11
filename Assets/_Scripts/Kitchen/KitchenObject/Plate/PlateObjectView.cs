using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(PlateObjectController))]
public class PlateObjectView : KitchenObjectView
{
    [SerializeField] private Transform plateCenter;
    private readonly List<GameObject> spawnedVisuals = new();

    public PlateObjectModel PlateModel => model as PlateObjectModel;

    public override void Initialize()
    {
        PlateModel.OnIngredientAdded += _ => RebuildStack();
        PlateModel.OnIngredientRemoved += _ => RebuildStack();
        RebuildStack();
    }

    private void OnDestroy()
    {
        currentTween?.Kill();
        PlateModel.OnIngredientAdded -= _ => RebuildStack();
        PlateModel.OnIngredientRemoved -= _ => RebuildStack();
    }

    private void RebuildStack()
    {
        foreach (var obj in spawnedVisuals)
            Destroy(obj);

        spawnedVisuals.Clear();

        float currentHeight = 0f;
        var includedIngredients = PlateModel.GetIngredientList();
        var splittable = includedIngredients?.FirstOrDefault(t => t.Splittable);

        if (splittable != null)
        {
            var bottomVisual = Instantiate(splittable.BottomPrefab, plateCenter);
            bottomVisual.transform.localPosition = new Vector3(0, currentHeight, 0);
            spawnedVisuals.Add(bottomVisual);
            currentHeight += splittable.SplitVisualOffset;
        }

        foreach (var ingredient in PlateModel.GetIngredientList())
        {
            if (ingredient == splittable)
                continue;

            var visual = Instantiate(ingredient.Prefab, plateCenter);
            visual.transform.localPosition = new Vector3(0, currentHeight, 0);

            spawnedVisuals.Add(visual);

            currentHeight += ingredient.VisualOffset;
        }

        if (splittable != null)
        {
            var topVisual = Instantiate(splittable.TopPrefab, plateCenter);
            topVisual.transform.localPosition = new Vector3(0, currentHeight, 0);
            spawnedVisuals.Add(topVisual);
            currentHeight += splittable.TopVisualOffset;
        }
    }
}
