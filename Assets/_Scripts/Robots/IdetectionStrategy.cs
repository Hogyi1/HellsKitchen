using UnityEngine;
public interface IdetectionStrategy
{
    bool Execute(Transform player, Transform detector);
}

