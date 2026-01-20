using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public List<Recipe> debugRecipe;
    [SerializeField] RecipeName activeRecipeTemplate;
    [SerializeField] int maximumOrders = 5;
    [SerializeField] float startingTime = 10f;
    [SerializeField] private AudioSO moneyAudio;
    [SerializeField] private AudioSO newOrder;

    Recipe activeRecipe;
    RecipeSO activeRecipeSO;
    public int RecipesGenerated = 0; // Pure stats

    private RecipeGenerator _recipeGenerator;
    private Queue<Recipe> _orderQueue;
    private LoopTimer _orderTimer;
    private float extraOrderTime;

    public event Action<Recipe> OnNewOrder = delegate { };
    public event Action<Recipe> OnActiveOrder = delegate { };
    public event Action<Recipe> OnOrderRemoved = delegate { };
    public event Action<Recipe> OnOrderFailed = delegate { };
    public event Action<Recipe, float> OnOrderCompleted = delegate { };

    private void Awake()
    {
        _orderQueue = new(maximumOrders);
        var so = SetRecipeSO(activeRecipeTemplate);
        _recipeGenerator = new(so);
        extraOrderTime = so.AveragePrepareTime * 0.5f;
        _orderTimer = new(so.AveragePrepareTime * 0.5f, -1);
        _orderTimer.OnLoop += GenerateOrder;
    }

    private void OnDisable()
    {
        _orderTimer.Stop();
        _orderTimer.OnLoop -= GenerateOrder;
    }

    public void StartOrders()
    {
        var timer = new CountDownTimer(startingTime);
        timer.OnTimerStop += () => SelectActiveRecipe();
        timer.Start();
        _orderTimer.Start();
    }

    public void GenerateOrder(int round)
    {
        if (_orderQueue.Count >= maximumOrders) return;

        Recipe newRecipe = _recipeGenerator.GenerateRecipe();
        newRecipe.AddTime(_orderQueue.Count * extraOrderTime);
        _orderQueue.Enqueue(newRecipe);
        debugRecipe.Add(newRecipe);
        OnNewOrder?.Invoke(newRecipe);
        RecipesGenerated++;
        AudioManager.Instance.PlaySFXUI(newOrder);
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
        OnOrderFailed?.Invoke(expiredRecipe);
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
        OnOrderCompleted?.Invoke(recipe, score * activeRecipeSO.Price);
        OnOrderRemoved?.Invoke(recipe);

        AudioManager.Instance.PlaySFXUI(moneyAudio);
        SelectActiveRecipe();

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
            GenerateOrder(RecipesGenerated);
            activeRecipe = _orderQueue.Peek();
        }

        activeRecipe.IsActive = true;
    }

    public void EndOrders()
    {
        _orderTimer.Stop();

        foreach (var recipe in _orderQueue)
        {
            OnOrderRemoved?.Invoke(recipe);
            recipe.IsActive = false;
        }

        _orderQueue.Clear();
        debugRecipe.Clear();
    }
}
