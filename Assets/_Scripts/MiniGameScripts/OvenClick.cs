using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OvenClick : MonoBehaviour, IPointerClickHandler
{
    public Sprite dirtyOvenSprite;
    public Sprite cleanOvenSprite;
    private bool clicked = false;
    Image img;
    OvenCleaning cleaning;

    void Awake()
    {
        img = GetComponent<Image>();
        cleaning = GetComponent<OvenCleaning>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("TEST OVEN CLICK");
        img.sprite = dirtyOvenSprite;
        if (cleaning != null && !clicked)
        {
            cleaning.ResetCleaning();
            clicked = true;
        }
    }
}
