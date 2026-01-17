using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(OrderManager))]
public class OrdersUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset recipeTemplate;
    [SerializeField] private OrderManager _orderManager;

    [SerializeField] Color finish = new Color(115, 175, 111);
    [SerializeField] Color start = new Color(220, 0, 0);

    private VisualElement _ordersInstance;
    private Dictionary<Recipe, VisualElement> _activeOrders = new Dictionary<Recipe, VisualElement>();

    private void OnEnable()
    {
        uiDocument = uiDocument != null ? uiDocument : GetComponent<UIDocument>();
        _orderManager = _orderManager != null ? _orderManager : GetComponent<OrderManager>();

        _ordersInstance = uiDocument.rootVisualElement.Q<VisualElement>("OrderContainer");
        if (_ordersInstance == null)
        {
            Debug.LogError("OrdersContainer not found in UIDocument root. Check UXML element name=\"OrdersContainer\".");
            return;
        }

        _orderManager.OnNewOrder += HandleNewOrder;
        _orderManager.OnOrderRemoved += HandleOrderRemoved;
    }

    private void OnDisable()
    {
        if (_orderManager != null)
        {
            _orderManager.OnNewOrder -= HandleNewOrder;
            _orderManager.OnOrderRemoved -= HandleOrderRemoved;
        }
    }

    private void HandleNewOrder(Recipe recipe)
    {
        if (recipeTemplate == null) return;

        TemplateContainer instance = recipeTemplate.CloneTree();
        VisualElement entry = instance.Q("RecipeEntry");

        VisualElement ingredientContainer = entry.Q<VisualElement>("IngredientContainer");
        if (ingredientContainer != null)
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                for (int i = 0; i < ingredient.Quantity; i++)
                {
                    Image icon = new Image();
                    icon.sprite = ingredient.KitchenObjectSO.Sprite;
                    icon.style.width = 30;
                    icon.style.height = 30;
                    icon.style.marginBottom = 2;
                    icon.style.marginRight = 2;
                    ingredientContainer.Add(icon);
                }
            }
        }

        var _barMask = instance.Q<VisualElement>("BarFill");

        _barMask.dataSource = recipe;
        DataBinding widthBinding = new DataBinding
        {
            dataSource = recipe,
            dataSourcePath = new PropertyPath(nameof(recipe.ExpirationProgress)),
            bindingMode = BindingMode.ToTarget
        };
        widthBinding.sourceToUiConverters.AddConverter((ref float value) => new StyleLength(new Length(value * 100, LengthUnit.Percent)));

        DataBinding colorBinding = new DataBinding
        {
            dataSource = recipe,
            dataSourcePath = new PropertyPath(nameof(recipe.ExpirationProgress)),
            bindingMode = BindingMode.ToTarget
        };
        colorBinding.sourceToUiConverters.AddConverter((ref float value) => { return new StyleColor(Color.Lerp(start, finish, value)); });

        _barMask.SetBinding("style.width", widthBinding);
        _barMask.SetBinding("style.backgroundColor", colorBinding);

        _ordersInstance.Add(instance);
        _activeOrders.Add(recipe, instance);
    }

    private void HandleOrderRemoved(Recipe recipe)
    {
        if (_activeOrders.TryGetValue(recipe, out VisualElement element))
        {
            _ordersInstance.Remove(element);
            _activeOrders.Remove(recipe);
        }
    }
}
