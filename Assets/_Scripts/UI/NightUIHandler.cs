using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

public class NightUIHandler : MonoBehaviour, IDataBinder
{
    [SerializeField] UIDocument uiDocument;

    Label _timeLabel;
    private void Awake()
    {
        uiDocument = uiDocument != null ? uiDocument : GetComponent<UIDocument>();

        _timeLabel = uiDocument.rootVisualElement.Q<Label>("TimeLabel");
    }

    public void BindData(object data)
    {
        if (data == null)
            return;

        var root = uiDocument.rootVisualElement;
        var nightData = data as NightDataModel;

        root.dataSource = nightData;

        DataBinding timeBinding = new DataBinding
        {
            dataSource = nightData,
            dataSourcePath = new PropertyPath(nameof(nightData.Seconds)),
            bindingMode = BindingMode.ToTarget
        };
        timeBinding.sourceToUiConverters.AddConverter((ref int value) => nightData.GetFormattedTime());

        _timeLabel.SetBinding("text", timeBinding);
    }
}
