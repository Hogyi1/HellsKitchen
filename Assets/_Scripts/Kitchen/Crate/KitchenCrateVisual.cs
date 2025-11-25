using UnityEngine;

[RequireComponent(typeof(KitchenCrate), typeof(Animator))]
public class KitchenCrateVisual : MonoBehaviour
{
    [SerializeField] KitchenCrate myCrate;
    [SerializeField] Animator anim;
    [SerializeField] Sprite mySprite;

    private void Awake()
    {
        myCrate = myCrate != null ? myCrate : GetComponentInChildren<KitchenCrate>();
        anim = anim != null ? anim : gameObject.AddComponent<Animator>();
        mySprite = mySprite != null ? mySprite : myCrate.crateObject.Icon;

        myCrate.OnObjectSpawned += InteractedWith;
    }

    void InteractedWith(KitchenObject ko)
    {
        // play animation open close
    }
}