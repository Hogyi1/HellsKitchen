using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private Transform doorModel;
    [SerializeField] private float openHeight = 6.8f;
    [SerializeField] private float speed = 4f;
    [SerializeField] private AudioSO sound;
    [SerializeField] private AudioSource source;
    
    private Vector3 closedPos;
    private Vector3 openPos;

    private bool shouldOpen = false;
    private bool hasPower = true;
   
    private HashSet<Collider> inRoom = new HashSet<Collider>();
    private HashSet<Collider> canExitOnce = new HashSet<Collider>();
    private HashSet<Collider> entitiesInTrigger = new HashSet<Collider>();

    private CountDownTimer soundCoolDown;

    void Start()
    {
        soundCoolDown = new CountDownTimer(0.5f);
        closedPos = doorModel.localPosition;
        openPos = closedPos + Vector3.up * openHeight;
    }

    void Update()
    {
        Vector3 targetPos = shouldOpen ? openPos : closedPos;

        doorModel.localPosition = Vector3.MoveTowards(doorModel.localPosition, targetPos, speed * Time.deltaTime);
    }
    // ROOM TRACKING
    void OnTriggerEnter(Collider other)
    {
        if (!IsValidEntity(other)) return;
        entitiesInTrigger.Add(other);

        EvaluateDoorState();
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsValidEntity(other)) return;
        entitiesInTrigger.Remove(other);
        EvaluateDoorState();
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

    // POWER LOGIC
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

    void EvaluateDoorState()
    {
        entitiesInTrigger.RemoveWhere(e => e == null);
        inRoom.RemoveWhere(e => e == null);
        canExitOnce.RemoveWhere(e => e == null);
        bool previousState = shouldOpen;

        if (entitiesInTrigger.Count == 0)
        {
            shouldOpen = false;
            PlaySound(previousState);
            return;
        }

        if (hasPower)
        {
            shouldOpen = true;
            PlaySound(previousState);
            return;
        }
        
        foreach (var e in entitiesInTrigger)
        {
            if (canExitOnce.Contains(e))
            {
                shouldOpen = true;
                canExitOnce.Remove(e);
                PlaySound(previousState);
                return;
            }
        }

        shouldOpen = false;
        PlaySound(previousState);
        
    }

    private void PlaySound(bool prev)
    {
        if (shouldOpen != prev && !soundCoolDown.IsRunning)
        {
            soundCoolDown.Start();
            AudioManager.Instance.PlaySFX(sound, source);
        }
    }
}
