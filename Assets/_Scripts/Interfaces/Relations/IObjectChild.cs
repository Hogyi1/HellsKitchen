public interface IObjectChild
{
    void SetParent(IObjectParent parent);
    void ClearParent();
    void SwapParent(IObjectChild swap);
    IObjectParent GetParent();
}
