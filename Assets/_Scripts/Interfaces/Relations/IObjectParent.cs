using UnityEngine;

public interface IObjectParent
{
    Transform GetParentPosition();
    void SetChild(IObjectChild child);
    IObjectChild GetChild();
}
