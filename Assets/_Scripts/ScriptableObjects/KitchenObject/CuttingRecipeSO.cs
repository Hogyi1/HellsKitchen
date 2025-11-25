using UnityEngine;

[CreateAssetMenu(fileName = "NewCuttingRecipe", menuName = "Kitchen/CuttingRecipeSO")]
public class CuttingRecipeSO : ScriptableObject
{
    [Range(1, 20)] public int cuttingTimes = 5;
    public KitchenObjectSO from;
    public KitchenObjectSO to;
}
