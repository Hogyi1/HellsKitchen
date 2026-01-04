using System;
using UnityEngine;

/// <summary>
/// Controls the logic for a KitchenContainer, handling the spawning of KitchenObjects
/// based on the model's state and cooldown.
/// </summary>
public class ContainerController : CounterController, ISpawner<KitchenObject>, IHasCooldown
{
    [Range(0, 3f)] float interactionTimer = 2f;
    [SerializeField] KitchenObjectSO KitchenObjectSO;
    private CountDownTimer _timer;

    /// <summary>
    /// Gets the associated KitchenContainer model.
    /// </summary>
    public ContainerModel Model => GetModel<ContainerModel>();

    /// <summary>
    /// Initializes the controller by creating its KitchenContainer model
    /// and setting up predicates for interaction.
    /// </summary>
    protected override void Initialize()
    {
        model = new ContainerModel(interactionTimer, KitchenObjectSO);
        _timer = new(interactionTimer);

        var isReady = new FunctionPredicate(() => IsReady());
        predicateList.Add(isReady);
    }

    /// <summary>
    /// Spawns a KitchenObject from the container.
    /// </summary>
    /// <param name="parent">The parent object for the newly spawned KitchenObject.</param>
    /// <param name="parentTransform">The transform to parent the visual representation to.</param>
    /// <returns>The newly spawned KitchenObject.</returns>
    public KitchenObject SpawnObject(IObjectParent parent, Transform parentTransform)
    {
        var ko = KitchenObject.SpawnVisual(Model.CrateObject, parent, parentTransform);
        _timer.Start();
        Model.NotifyObjectSpawned(ko);
        return ko;
    }

    /// <summary>
    /// Checks if the container is ready to spawn another object (i.e., the cooldown timer has finished).
    /// </summary>
    /// <returns>True if the container is ready, false otherwise.</returns>
    public bool IsReady() => !_timer.IsRunning;

    public KitchenObject GetSpawnerObject() => SpawnObject(Model, GetSpawnPosition()); //TODO refactor

    public Transform GetSpawnPosition() => view.GetTransformPosition();
}