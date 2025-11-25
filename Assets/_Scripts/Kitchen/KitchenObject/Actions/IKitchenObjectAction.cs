public interface IKitchenObjectAction
{
    bool CanExecute(KitchenObject other);
    void Execute(KitchenObject other);
}
