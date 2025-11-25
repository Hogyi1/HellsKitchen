public interface IObjectParent<T> : IObjectParent where T : IObjectChild
{
    void SetChild(T child);
    new T GetChild();
}
