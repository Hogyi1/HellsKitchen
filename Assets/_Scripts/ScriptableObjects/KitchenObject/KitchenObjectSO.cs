using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKitchenObject", menuName = "Kitchen/KitchenObjectSO")]
public class KitchenObjectSO : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public GameObject Prefab;
    public float VisualOffset;

    [Header("Splittable")]
    public bool Splittable = false;
    public GameObject BottomPrefab;
    public GameObject TopPrefab;
    public float SplitVisualOffset;
    public float TopVisualOffset;
}