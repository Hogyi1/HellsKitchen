using UnityEngine;
using UnityEngine.EventSystems;

public class LoaderDrop : MonoBehaviour, IDropHandler
{
    public RectTransform[] slots;
    public int filledSlots = 0;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Drop detected!");

        if (eventData.pointerDrag == null) return;

        PlateDrag plate = eventData.pointerDrag.GetComponent<PlateDrag>();
        if (plate != null && filledSlots < slots.Length)
        {
            Debug.Log($"Snapping to slot {filledSlots}");
            plate.SnapToSlot(slots[filledSlots]);
            filledSlots++;

            // Reset for new plates when all slots full
            if (filledSlots >= slots.Length)
            {
                Invoke("ResetSlots", 2f); // Reset after 2 seconds
            }
        }
    }

    void ResetSlots()
    {
        filledSlots = 0;
        Debug.Log("Slots reset - ready for new plates");
    }
}
