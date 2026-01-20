using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlateObject", menuName = "Kitchen/RecipeSO")]
public class RecipeSO : ScriptableObject
{
    public RecipeName RecipeName;
    public RecipeIngredient[] MainIngredient;
    public RecipeIngredient[] IngredientsToChooseFrom;
    public float AveragePrepareTime = 30f;
    public int MaxIngredientCount;
    public float Price = 30f;
}

[Serializable]
public struct RecipeIngredient
{
    public KitchenObjectSO KitchenObjectSO;
    public int Quantity;
}

public enum RecipeName
{
    Default,
    Burger,
    HotDog,
    None
}