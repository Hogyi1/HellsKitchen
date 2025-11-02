using UnityEngine;

// Még nemtudom melyik lenne jelenleg a jobb
public interface IInteractable<T>
{
    public void OnInteract(T context);
}
public interface IInteractable
{
    void OnInteract();
    void StopInteraction();
}