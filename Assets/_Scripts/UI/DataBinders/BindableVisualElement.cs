using UnityEngine.UIElements;

[UxmlElement]
public partial class BindableVisualElement : VisualElement
{
    [UxmlAttribute]
    public float Opacity
    {
        get => style.opacity.value;
        set => style.opacity = value;
    }
}
