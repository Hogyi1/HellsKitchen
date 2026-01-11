public class PlaceOntoPlateAction : IKitchenObjectAction
{
    private PlateObjectController plate;
    public PlaceOntoPlateAction(PlateObjectController plate)
    {
        this.plate = plate;
    }

    public bool CanExecute(KitchenObjectController other)
    {
        if (other is IngredientController ingredient)
            return plate.CanAddIngredient(ingredient);
        return false;
    }

    public void Execute(KitchenObjectController other)
    {
        plate.AddIngredient(other);
        other.DestroySelf();
    }
}