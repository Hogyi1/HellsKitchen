using UnityEngine;
using UnityEngine.Events;

public interface ISpawner<T>
{
    T SpawnObject(IObjectParent context, Transform transform);
    T GetSpawnerObject();
    Transform GetSpawnPosition();
}