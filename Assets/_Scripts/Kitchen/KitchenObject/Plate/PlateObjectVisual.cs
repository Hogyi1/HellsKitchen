using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlateObject))]
public class PlateObjectVisual : MonoBehaviour
{
    [SerializeField] private PlateObject plateObject;
    [SerializeField] private Transform plateCenter;
    public bool debug;
    private readonly List<GameObject> spawnedVisuals = new();

    private void Awake()
    {
        if (plateObject == null)
            plateObject = GetComponent<PlateObject>();
    }

    private void Start()
    {
        plateObject.OnIngredientAdded += _ => RebuildStack();
        plateObject.OnIngredientRemoved += _ => RebuildStack();
        RebuildStack();
    }

    private void OnDestroy()
    {
        plateObject.OnIngredientAdded -= _ => RebuildStack();
        plateObject.OnIngredientRemoved -= _ => RebuildStack();
    }

    private void RebuildStack()
    {
        foreach (var obj in spawnedVisuals)
            Destroy(obj);

        spawnedVisuals.Clear();

        float currentHeight = 0f;
        var includedIngredients = plateObject.GetIngredientList();
        var splittable = includedIngredients?.FirstOrDefault(t => t.Splittable);

        if (splittable != null)
        {
            var bottomVisual = Instantiate(splittable.BottomPrefab, plateCenter);
            bottomVisual.transform.localPosition = new Vector3(0, currentHeight, 0);
            spawnedVisuals.Add(bottomVisual);
            currentHeight += splittable.SplitVisualOffset;
        }

        foreach (var ingredient in plateObject.GetIngredientList())
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

    private void OnValidate()
    {
        if (plateObject != null)
            RebuildStack();
    }
}
