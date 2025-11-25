using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RecipeGenerator))]
public class OrderManager : Singleton<OrderManager>
{
    public List<Recipe> debugRecipe;
    Queue<Recipe> orderQueue;
    Recipe activeRecipe;

    [SerializeField] RecipeName activeRecipeTemplate;
    [SerializeField] float extraOrderTime = 15f;
    RecipeGenerator recipeGenerator;

    public event Action<Recipe> OnNewOrder = delegate { };
    public event Action<Recipe> OnActiveOrder = delegate { };


    //Debug
    LoopTimer debugTimer;

    protected override void Awake()
    {
        base.Awake();

        orderQueue = new Queue<Recipe>();

        recipeGenerator = GetComponent<RecipeGenerator>();
        recipeGenerator.SetRecipeTemplate(activeRecipeTemplate);

        debugTimer = new(20f, -1);
        debugTimer.OnLoop += (loop) => GenerateOrder();
    }

    private void Start()
    {
        GenerateOrder();
        activeRecipe = orderQueue.Peek();
    }

    public void GenerateOrder()
    {
        Recipe newRecipe = recipeGenerator.GenerateRecipe();
        newRecipe.AddTime(orderQueue.Count * extraOrderTime);
        orderQueue.Enqueue(newRecipe);
        debugRecipe.Add(newRecipe);
        OnNewOrder?.Invoke(newRecipe);
    }

    private float ScoreOrder(Dictionary<KitchenObjectSO, int> incoming) =>
        activeRecipe.CalculateMatch(incoming);

    public void CompleteOrder(Dictionary<KitchenObjectSO, int> incoming)
    {
        float score = ScoreOrder(incoming);
        Debug.Log($"Order completed with score: {score}");

        debugRecipe.Remove(activeRecipe);

        if (orderQueue.Count > 0)
            orderQueue.Dequeue();

        if (orderQueue.Count > 0)
        {
            activeRecipe = orderQueue.Peek();
            OnActiveOrder?.Invoke(activeRecipe);
        }
        else
        {
            GenerateOrder();
            activeRecipe = orderQueue.Peek();
        }

        Debug.Log("Dish has been scored: " + score);
        // IDK money vagy something
    }
}
