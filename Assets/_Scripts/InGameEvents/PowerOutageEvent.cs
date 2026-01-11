using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerOutageEvent : MonoBehaviour
{

    public float checkInterval = 10f;
    public float outageChancePercent = 1f;
    public Transform doorparent;

    private List<DoorManager> doorManagers;
    private LightManager lightManager;

    public event Action<bool> PowerOutage = delegate { };

    void Awake()
    {
        int n = doorparent.childCount;
        doorManagers = new List<DoorManager>();
        for (int i = 0; i < n; i++) doorManagers.Add(doorparent.GetChild(i).GetComponent<DoorManager>());
        lightManager = FindAnyObjectByType<LightManager>();
    }
    private void PowerChange()
    {
        Debug.Log("Áramkimaradás történt!");

        foreach (DoorManager door in doorManagers)
        {
            door.OnPowerOutage();
        }
        lightManager.FadeOut();


        PowerOutage?.Invoke(false);
    }
   
}
