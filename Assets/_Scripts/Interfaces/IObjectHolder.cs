public interface IObjectHolder<T>
{
    bool HasChild();
    bool CanRelease();
    bool CanPlace(T other);
    void OnPlace(T other);
    void OnRelease();
}