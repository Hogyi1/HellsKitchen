using UnityEngine.Events;

public interface ISpawner<T>
{
    event UnityAction<T> OnObjectSpawned;
    T SpawnObject(IObjectParent context);
}