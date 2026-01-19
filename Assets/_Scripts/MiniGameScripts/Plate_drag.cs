using UnityEngine;
using UnityEngine.EventSystems;

public class PlateDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rt;
    private Canvas canvas;
    private Vector2 originalPos;
    public GameObject loadedPlatePrefab;
    public bool snapped = false;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalPos = rt.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;
        rt.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null) cg.blocksRaycasts = true; 

        if (!snapped)
        {
            rt.anchoredPosition = originalPos;
        }
        snapped = false;
    }

    public void SnapToSlot(RectTransform slot)
    {
        transform.SetParent(slot);
        rt.anchoredPosition = Vector2.zero; 
        transform.SetAsLastSibling();

        if (loadedPlatePrefab)
        {
            RectTransform loadedRT = Instantiate(loadedPlatePrefab, slot)
                .GetComponent<RectTransform>();
            loadedRT.anchoredPosition = Vector2.zero;
        }
        gameObject.SetActive(false);
        snapped = true;
    }
}
