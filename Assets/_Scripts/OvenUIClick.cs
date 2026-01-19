using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OvenUIClick : MonoBehaviour, IPointerClickHandler
{
    public Sprite dirtyOvenSprite;

    Image img;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Oven UI clicked");
        img.sprite = dirtyOvenSprite;
    }
}
