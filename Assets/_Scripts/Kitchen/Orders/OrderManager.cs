using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : Singleton<OrderManager>
{
    public List<Recipe> debugRecipe;
    [SerializeField] RecipeName activeRecipeTemplate;
    [SerializeField] int maximumOrders = 5;

    Recipe activeRecipe;
    RecipeSO activeRecipeSO;
    int recipesGenerated = 0; // Pure stats

    private RecipeGenerator _recipeGenerator;
    private Queue<Recipe> _orderQueue;
    private LoopTimer _orderTimer;
    private float extraOrderTime;

    public event Action<Recipe> OnNewOrder = delegate { };
    public event Action<Recipe> OnActiveOrder = delegate { };
    public event Action<Recipe> OnOrderRemoved = delegate { };
    public event Action<Recipe> OnOrderExpiration = delegate { };

    public override void BaseAwake()
    {
        _orderQueue = new(maximumOrders);
        var so = SetRecipeSO(activeRecipeTemplate);
        _recipeGenerator = new(so);
        extraOrderTime = so.AveragePrepareTime * 0.75f;
        _orderTimer = new(so.AveragePrepareTime * 0.5f, -1);
        _orderTimer.OnLoop += GenerateOrder;
        _orderTimer.Start();

        SelectActiveRecipe();
    }

    public void GenerateOrder(int round)
    {
        if (_orderQueue.Count >= maximumOrders) return;

        Recipe newRecipe = _recipeGenerator.GenerateRecipe();
        newRecipe.AddTime(_orderQueue.Count * extraOrderTime);
        _orderQueue.Enqueue(newRecipe);
        debugRecipe.Add(newRecipe);
        OnNewOrder?.Invoke(newRecipe);
        recipesGenerated++;
    }

    private void Update()
    {
        foreach (var recipe in _orderQueue)
        {
            recipe.PassedTime += Time.deltaTime;

            if (recipe.IsExpired)
            {
                OnExpiration(recipe);
                break;
            }
        }
    }

    private void OnExpiration(Recipe recipe)
    {
        _orderQueue.TryDequeue(out Recipe expiredRecipe);
        debugRecipe.Remove(expiredRecipe);
        OnOrderRemoved?.Invoke(expiredRecipe);
        SelectActiveRecipe();
        Debug.Log("Order expired!");
    }

    private float ScoreOrder(Dictionary<KitchenObjectSO, int> incoming) =>
        activeRecipe.CalculateMatch(incoming);

    public void CompleteOrder(Dictionary<KitchenObjectSO, int> incoming)
    {
        float score = ScoreOrder(incoming);
        Debug.Log($"Order completed with score: {score}");


        _orderQueue.TryDequeue(out Recipe recipe);
        OnOrderRemoved?.Invoke(recipe);

        SelectActiveRecipe();

        Debug.Log("Dish has been scored: " + score);
        // IDK money vagy something
        debugRecipe.Remove(activeRecipe);
    }

    private RecipeSO SetRecipeSO(RecipeName activeRecipeTemplate)
    {
        KitchenSODatabase.SetRecipeName(activeRecipeTemplate);
        activeRecipeSO = KitchenSODatabase.GetRecipeByName(activeRecipeTemplate);
        return activeRecipeSO;
    }

    private void SelectActiveRecipe()
    {
        if (_orderQueue.Count > 0)
        {
            activeRecipe = _orderQueue.Peek();
            OnActiveOrder?.Invoke(activeRecipe);
        }
        else
        {
            GenerateOrder(recipesGenerated);
            activeRecipe = _orderQueue.Peek();
        }
    }
}
