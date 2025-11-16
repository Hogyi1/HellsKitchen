using UnityEngine;

[CreateAssetMenu(fileName = "NewKitchenObject", menuName = "ScriptableObjects/KitchenObjectSO")]
public class KitchenObjectSO : ScriptableObject
{
    public string Name;
    public Sprite Icon;
    public GameObject Prefab;
}
