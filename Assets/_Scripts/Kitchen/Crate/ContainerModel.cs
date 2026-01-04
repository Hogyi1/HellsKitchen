using System;

/// <summary>
/// Represents the model for a kitchen container (crate) that can spawn specific KitchenObjects.
/// It includes a cooldown timer after each spawn.
/// </summary>
public class ContainerModel : CounterModel
{
    /// <summary>
    /// Event triggered when a new KitchenObject is spawned from this container.
    /// </summary>
    public event Action<KitchenObject> OnObjectSpawned = delegate { };

    /// <summary>
    /// Gets the ScriptableObject representing the type of KitchenObject this container spawns.
    /// </summary>
    public KitchenObjectSO CrateObject
    {
        get => _crateObject;
        private set => _crateObject = value;
    }

    private KitchenObjectSO _crateObject;
    private float cooldownTime;

    /// <summary>
    /// Initializes a new instance of the KitchenContainer.
    /// </summary>
    /// <param name="interactionTimer">The cooldown duration after spawning an object.</param>
    /// <param name="so">The ScriptableObject for the KitchenObject to be spawned.</param>
    public ContainerModel(float interactionTimer, KitchenObjectSO so)
    {
        cooldownTime = interactionTimer;
        CrateObject = so;
    }

    /// <summary>
    /// Notifies listeners that an object has been spawned and starts the cooldown timer.
    /// </summary>
    /// <param name="ko">The KitchenObject that was spawned.</param>
    public void NotifyObjectSpawned(KitchenObject ko)
    {
        OnObjectSpawned.Invoke(ko);
    }
}