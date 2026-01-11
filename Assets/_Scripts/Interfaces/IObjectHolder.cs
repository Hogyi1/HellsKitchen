public interface IObjectHolder<T>
{
    bool CanRelease();
    bool CanPlace(T other);
    void OnPlace(T other);
    void OnRelease();
}