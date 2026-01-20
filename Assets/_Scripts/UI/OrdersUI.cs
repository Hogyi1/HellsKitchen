using System;
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
    [SerializeField] Color active = new Color(122, 122, 122);
    [SerializeField] Color inActive = new Color(48, 48, 48);

    private VisualElement _ordersContainer;
    private Dictionary<Recipe, VisualElement> _activeOrders = new Dictionary<Recipe, VisualElement>();

    private void OnEnable()
    {
        uiDocument = uiDocument != null ? uiDocument : GetComponent<UIDocument>();
        _orderManager = _orderManager != null ? _orderManager : GetComponent<OrderManager>();

        _ordersContainer = uiDocument.rootVisualElement.Q<VisualElement>("OrderContainer");

        _orderManager.OnNewOrder += HandleNewOrder;
        _orderManager.OnOrderRemoved += HandleOrderRemoved;
        _orderManager.OnActiveOrder += HandleActive;
    }

    private void OnDisable()
    {
        _orderManager.OnNewOrder -= HandleNewOrder;
        _orderManager.OnOrderRemoved -= HandleOrderRemoved;
        _orderManager.OnActiveOrder -= HandleActive;
    }

    private void HandleActive(Recipe recipe)
    {
        if (_activeOrders.TryGetValue(recipe, out VisualElement element))
        {
            element.Q<VisualElement>("RecipeEntry").style.backgroundColor = new StyleColor(active);
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
        var _containerBg = instance.Q<VisualElement>("RecipeEntry");

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

        _containerBg.dataSource = recipe;
        DataBinding backgroundColorBinding = new DataBinding
        {
            dataSource = recipe,
            dataSourcePath = new PropertyPath(nameof(recipe.IsActive)),
            bindingMode = BindingMode.ToTarget
        };
        backgroundColorBinding.sourceToUiConverters.AddConverter((ref bool value) => { return new StyleColor(value ? active : inActive); });

        _barMask.SetBinding("style.width", widthBinding);
        _barMask.SetBinding("style.backgroundColor", colorBinding);
        _containerBg.SetBinding("style.backgroundColor", backgroundColorBinding);

        _ordersContainer.Add(instance);
        _activeOrders.Add(recipe, instance);
    }

    private void HandleOrderRemoved(Recipe recipe)
    {
        if (_activeOrders.TryGetValue(recipe, out VisualElement element))
        {
            _ordersContainer.Remove(element);
            _activeOrders.Remove(recipe);
        }
    }
}
