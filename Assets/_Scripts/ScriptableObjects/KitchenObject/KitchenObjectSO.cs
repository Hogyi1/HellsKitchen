using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewKitchenObject", menuName = "Kitchen/KitchenObjectSO")]
public class KitchenObjectSO : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    [UnityEngine.Range(-1.0f, 2.0f)] public float VisualOffset;

    [Header("Splittable")]
    public bool Splittable = false;
    public GameObject BottomPrefab;
    public GameObject TopPrefab;
    public float SplitVisualOffset;
    public float TopVisualOffset;

    [Header("Sounds")]
    public AudioSO PlaceSound;
    public AudioSO PickUpSound;
    public AudioSO DisposeSound;

    [Header("Other")]
    public Sprite Sprite;
    public bool IsTwoHanded = true;
}