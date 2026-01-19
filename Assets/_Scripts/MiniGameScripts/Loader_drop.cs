using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class LoaderDrop : PatternMinigame, IDropHandler
{
    public RectTransform[] slots;
    public int filledSlots = 0;
    [SerializeField] private CinemachineCamera cam;

    private void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
    }
    public override void StartGame()
    {
        parentCanvas.enabled = true;
        filledSlots = 0;
    }
    public override void EndGame()
    {
        parentCanvas.enabled = false;
    }

    public override CinemachineCamera GetCamera()
    {
        return cam;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        PlateDrag plate = eventData.pointerDrag.GetComponent<PlateDrag>();
        if (plate != null && filledSlots < slots.Length)
        {
            Debug.Log($"Snapping to slot {filledSlots}");
            plate.SnapToSlot(slots[filledSlots]);
            filledSlots++;

            if (filledSlots >= slots.Length)
            {
                GameCompleted();         
            }
        }
    }

    
}
