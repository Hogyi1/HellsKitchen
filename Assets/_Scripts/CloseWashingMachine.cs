using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CloseWashingMachine : MonoBehaviour, IPointerClickHandler
{
    public Sprite closedWM;
    public LoaderDrop loader; // Drag your Loader GameObject here
    public Vector2 closedPosition = Vector2.zero; // Set this in Inspector
    
    Image img;
    RectTransform rect;
    private bool canClick = false;
    private Vector2 originalPos;

    void Awake()
    {
        img = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
    }

    void Update()
    {
        canClick = (loader != null && loader.filledSlots >= 8);
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (canClick)
        {
            img.sprite = closedWM;
            rect.anchoredPosition = closedPosition; // Move to your position
        }
    }
}
