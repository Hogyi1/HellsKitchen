using UnityEngine;

[RequireComponent(typeof(TrashBinController))]
public class TrashBinView : CounterView
{
    [SerializeField] AudioSource audioSource;

    public TrashBinModel Model => GetModel<TrashBinModel>();

    /// <summary>
    /// Subscribes to the model's item change event to trigger audio feedback.
    /// </summary>
    protected override void Initialize() => Model.OnItemDisposed += OnItemPlaced;


    /// <summary>
    /// Triggered when an item is placed on the counter. Plays a placement sound effect.
    /// </summary>
    /// <param name="ko">The KitchenObject that was placed. Can be null if an item was picked up.</param>
    public void OnItemPlaced(KitchenObjectController ko)
    {
        var audio = ko.GetKitchenObjectSO().DisposeSound;
        if (ko != null)
            AudioManager.Instance.PlaySFX(audio, audioSource);
    }

    /// <summary>
    /// Ensures the AudioSource component is assigned.
    /// </summary>
    protected override void SetupComponents() => audioSource = audioSource != null ? audioSource : GetComponentInChildren<AudioSource>();
}