using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorModel;
    public float openHeight = 6.8f;
    public float speed = 4f;

    private Vector3 closedPos;
    private Vector3 openPos;

    private bool shouldOpen = false;
    private bool hasPower = true;

    
    private HashSet<Collider> inRoom = new HashSet<Collider>();
    private HashSet<Collider> canExitOnce = new HashSet<Collider>();

    void Start()
    {
        closedPos = doorModel.localPosition;
        openPos = closedPos + Vector3.up * openHeight;
    }

    void Update()
    {
        Vector3 targetPos = shouldOpen ? openPos : closedPos;
        doorModel.localPosition =
            Vector3.Lerp(doorModel.localPosition, targetPos, Time.deltaTime * speed);
    }

    // ======================
    // ROOM TRACKING
    // ======================

    void OnTriggerEnter(Collider other)
    {
        if (!IsValidEntity(other)) return;

        
        if (hasPower || canExitOnce.Contains(other))
        {
            shouldOpen = true;

            if (!hasPower) canExitOnce.Remove(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsValidEntity(other)) return;

        shouldOpen = false;
    }

    public void OnRoomEnter(Collider other)
    {
        if (!IsValidEntity(other)) return;
        inRoom.Add(other);
    }

    public void OnRoomExit(Collider other)
    {
        if (!IsValidEntity(other)) return;
        inRoom.Remove(other);
    }

    // ======================
    // POWER LOGIC
    // ======================

    public void OnPowerOutage()
    {
        hasPower = false;
        foreach (var entity in inRoom)
            canExitOnce.Add(entity);
    }

    public void OnPowerReturn()
    {
        hasPower = true;
        foreach (var entity in inRoom)
            canExitOnce.Remove(entity);
    }

    bool IsValidEntity(Collider other)
    {
        return other.CompareTag("Player") || other.CompareTag("Enemy");
    }
}
