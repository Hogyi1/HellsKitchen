using UnityEngine;

[RequireComponent(typeof(PlateDispenser))]
public class PlateDispenserVisual : MonoBehaviour
{
    [SerializeField] PlateDispenser myDispenser;
    [SerializeField] Animator anim;
    [SerializeField] Sprite mySprite;

    private void Awake()
    {
        myDispenser = myDispenser != null ? myDispenser : GetComponentInChildren<PlateDispenser>();
        anim = anim != null ? anim : gameObject.AddComponent<Animator>();
        mySprite = mySprite != null ? mySprite : myDispenser.plateObject.Icon;

        myDispenser.OnObjectSpawned += InteractedWith;
        myDispenser.OnRefill += MyDispenser_OnRefill;
    }

    private void MyDispenser_OnRefill()
    {
        // refill animation etc
    }

    private void InteractedWith(KitchenObject ko)
    {
        // play animation grab etc
    }
}
