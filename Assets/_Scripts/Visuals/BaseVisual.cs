using System.Collections.Generic;
using UnityEngine;

public abstract class BaseVisual : MonoBehaviour, ISelectable
{
    public abstract void OnDeselect();

    public abstract void OnSelect();

    [SerializeField] protected GameObject selectedVisual;
    protected float currentOpacity;
    protected List<Material> allMaterials = new();

    private void Start()
    {
        selectedVisual.SetActive(false);

        Renderer[] renderers = selectedVisual.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            var materials = renderer.materials;
            allMaterials.AddRange(materials);
        }

        SetAllMaterialsAlpha(0f);
        currentOpacity = 0f;
    }

    protected void SetAllMaterialsAlpha(float alpha)
    {
        foreach (var mat in allMaterials)
        {
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
            currentOpacity = alpha;
        }
    }
}
