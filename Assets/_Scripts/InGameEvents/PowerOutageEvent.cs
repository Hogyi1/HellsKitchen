using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerOutageEvent : MonoBehaviour
{
    [SerializeField] private Transform doorparent;
    [SerializeField] private LightManager lightManager;

    private List<DoorManager> doorManagers;
    public event Action PowerOutage = delegate { };
    public event Action PowerBack = delegate { };

    void Awake()
    {
        int n = doorparent.childCount;
        doorManagers = new List<DoorManager>();
        for (int i = 0; i < n; i++) doorManagers.Add(doorparent.GetChild(i).GetComponent<DoorManager>());
    }
    public void PowerChange()
    {
        Debug.Log("Áramkimaradás történt!");

        foreach (DoorManager door in doorManagers)
        {
            door.OnPowerOutage();
        }
        lightManager.FadeOut();

        PowerOutage?.Invoke();
    }

    public void PowerComesBack()
    {
        Debug.Log("Visszajött az áram!");

        foreach (DoorManager door in doorManagers)
        {
            door.OnPowerReturn();
        }
        lightManager.FadeIn();

        PowerBack?.Invoke();
    }
   
}
