using UnityEngine;

[CreateAssetMenu(fileName = "NewFryingRecipe", menuName = "Kitchen/FryingRecipeSO")]
public class FryingRecipeSO : ScriptableObject
{
    public KitchenObjectSO from;
    public KitchenObjectSO to;
    public KitchenObjectSO burnt;

    public float fryingTime = 5f;
    public float burningTime = 3f;
}