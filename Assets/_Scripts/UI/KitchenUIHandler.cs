using System;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

public class KitchenUIHandler : MonoBehaviour, IDataBinder
{
    [SerializeField] UIDocument uiDocument;

    Label _timeLabel;
    Label _moneyLabel;
    private void Awake()
    {
        uiDocument = uiDocument != null ? uiDocument : GetComponent<UIDocument>();

        _timeLabel = uiDocument.rootVisualElement.Q<Label>("TimeLabel");
        _moneyLabel = uiDocument.rootVisualElement.Q<Label>("MoneyLabel");
    }

    public void BindData(object data)
    {
        if (data == null)
            return;

        var root = uiDocument.rootVisualElement;
        var kitchenData = data as KitchenDataModel;

        root.dataSource = kitchenData;

        DataBinding timeBinding = new DataBinding
        {
            dataSource = kitchenData,
            dataSourcePath = new PropertyPath(nameof(kitchenData.Seconds)),
            bindingMode = BindingMode.ToTarget
        };
        timeBinding.sourceToUiConverters.AddConverter((ref int value) => kitchenData.GetFormattedTime());

        DataBinding moneyBinding = new DataBinding
        {
            dataSource = kitchenData,
            dataSourcePath = new PropertyPath(nameof(kitchenData.TotalEarnings)),
            bindingMode = BindingMode.ToTarget
        };
        moneyBinding.sourceToUiConverters.AddConverter((ref float value) => $"${value:0.00}");

        _timeLabel.SetBinding("text", timeBinding);
        _moneyLabel.SetBinding("text", moneyBinding);
    }
}