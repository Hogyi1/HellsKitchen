public interface ISingleton<T>
{
    void BaseAwake();
    T GetInstance();
}