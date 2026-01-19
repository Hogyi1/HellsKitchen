public interface IDisposer<T>
{
    void OnDispose(T ko);
}
