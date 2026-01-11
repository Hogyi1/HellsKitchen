public interface IKitchenObjectAction
{
    bool CanExecute(KitchenObjectController other);
    void Execute(KitchenObjectController other);
}
