using UnityEngine;

public interface IObjectParent
{
    bool HasChild();
    void SetChild(IObjectChild child);
    IObjectChild GetChild();
    Transform GetTransform();
}
