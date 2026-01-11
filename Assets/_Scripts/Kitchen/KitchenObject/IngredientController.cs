using UnityEngine;

[RequireComponent(typeof(KitchenObjectView))]
public class IngredientController : KitchenObjectController, IDisposable
{
    public void Dispose() => DestroySelf();

    public override void Hold() { }

    public override void Initialize()
    {
        model = new KitchenObjectModel(so);
    }
}
