public interface IObjectParent<T> : IObjectParent where T : IObjectChild
{
    void SetChild(T child);
    void ClearChild();
    new T GetChild();
}
