using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Handles the UI presentation for the cutting progress bar,
/// binding to a CuttingBoard model's properties for updates.
/// </summary>
[RequireComponent(typeof(WorldSpaceUI_New))]
public class CuttingUIHandler : MonoBehaviour, IDataBinder
{
    [SerializeField] WorldSpaceUI_New ui;
    [SerializeField] UIDocument uiDocument;

    [SerializeField] Color finish = new Color(115, 175, 111);
    [SerializeField] Color start = new Color(220, 0, 0);

    VisualElement _barMask;

    private void Awake()
    {
        uiDocument = uiDocument != null ? uiDocument : GetComponent<UIDocument>();
        ui = ui != null ? ui : GetComponent<WorldSpaceUI_New>();
    }

    /// <summary>
    /// Binds UI elements to the provided data (expected to be a CuttingBoard instance).
    /// Sets up bindings for progress bar width and color based on CuttingProgress.
    /// </summary>
    /// <param name="data">The object containing the data to bind (e.g., a CuttingBoard).</param>
    public void BindData(object data)
    {
        var cuttingBoard = data as CuttingModel;

        _barMask = uiDocument.rootVisualElement.Q<VisualElement>("BarFill");
        _barMask.dataSource = cuttingBoard;
        DataBinding widthBinding = new DataBinding
        {
            dataSource = cuttingBoard,
            dataSourcePath = new PropertyPath(nameof(cuttingBoard.CuttingProgress)),
            bindingMode = BindingMode.ToTarget
        };
        widthBinding.sourceToUiConverters.AddConverter((ref float value) => new StyleLength(new Length(value * 100, LengthUnit.Percent)));

        DataBinding colorBinding = new DataBinding
        {
            dataSource = cuttingBoard,
            dataSourcePath = new PropertyPath(nameof(cuttingBoard.CuttingProgress)),
            bindingMode = BindingMode.ToTarget
        };
        colorBinding.sourceToUiConverters.AddConverter((ref float value) => { Debug.Log(value); return new StyleColor(Color.Lerp(start, finish, value)); });

        _barMask.SetBinding("style.width", widthBinding);
        _barMask.SetBinding("style.backgroundColor", colorBinding);
    }

    /// <summary>
    /// Activates and displays the cutting UI.
    /// </summary>
    public void TurnOn()
    {
        ui.CanActivate = true;
        ui.Activate();
    }

    /// <summary>
    /// Deactivates and hides the cutting UI.
    /// </summary>
    public void TurnOff()
    {
        ui.CanActivate = false;
        ui.Deactivate();
    }
}
