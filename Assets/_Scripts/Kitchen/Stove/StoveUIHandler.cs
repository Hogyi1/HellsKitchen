using DG.Tweening;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;
using static StoveModel.StoveState;

[RequireComponent(typeof(WorldSpaceUI_New))]
public class StoveUIHandler : MonoBehaviour, IDataBinder
{
    [SerializeField] WorldSpaceUI_New ui;
    [SerializeField] UIDocument uiDocument;

    [SerializeField] Color normal = new Color(115, 175, 111);
    [SerializeField] Color error = new Color(220, 0, 0);
    [SerializeField]
    [Range(0.1f, 5f)] float time = 1.0f;

    VisualElement _barMask;
    Tween currentTween;

    private void Awake()
    {
        uiDocument = uiDocument != null ? uiDocument : GetComponent<UIDocument>();
        ui = ui != null ? ui : GetComponent<WorldSpaceUI_New>();
    }

    public void BindData(object data)
    {
        StoveModel Model = data as StoveModel;
        _barMask = uiDocument.rootVisualElement.Q<VisualElement>("BarFill");
        _barMask.dataSource = Model;
        DataBinding widthBinding = new DataBinding
        {
            dataSource = Model,
            dataSourcePath = new PropertyPath(nameof(Model.FryingProgress)),
            bindingMode = BindingMode.ToTarget
        };
        widthBinding.sourceToUiConverters.AddConverter((ref float value) => new StyleLength(new Length(value * 100, LengthUnit.Percent)));

        DataBinding colorBinding = new DataBinding
        {
            dataSource = Model,
            dataSourcePath = new PropertyPath(nameof(Model.CurrentState)),
            bindingMode = BindingMode.ToTarget
        };
        colorBinding.sourceToUiConverters.AddConverter((ref StoveModel.StoveState state) =>
        {
            currentTween?.Kill();
            switch (state)
            {
                case Frying:
                case Idle:
                    return new StyleColor(normal);
                case Fried:
                    currentTween = DOTween.To(() => _barMask.style.backgroundColor.value,
                        x => _barMask.style.backgroundColor = x,
                        error,
                        time)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
                    return new StyleColor(normal);
                case Burnt:
                    return new StyleColor(error);
                default:
                    return new StyleColor(Color.white);
            }
        });

        _barMask.SetBinding("style.width", widthBinding);
        _barMask.SetBinding("style.backgroundColor", colorBinding);
    }

    public void TurnOn()
    {
        ui.CanActivate = true;
        ui.Activate();
    }

    public void TurnOff()
    {
        ui.CanActivate = false;
        ui.Deactivate();
    }
}