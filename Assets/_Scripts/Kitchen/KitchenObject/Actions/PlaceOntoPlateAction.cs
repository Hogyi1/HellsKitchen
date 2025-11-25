public class PlaceOntoPlateAction : IKitchenObjectAction
{
    private PlateObject plate;
    public PlaceOntoPlateAction(PlateObject plate)
    {
        this.plate = plate;
    }

    public bool CanExecute(KitchenObject other)
    {
        if (other is Ingredient ingredient)
            return plate.CanAddIngredient(ingredient);
        return false;
    }

    public void Execute(KitchenObject other)
    {
        plate.AddIngredient(other);
        other.DestroySelf();
    }
}