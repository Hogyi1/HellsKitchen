using UnityEngine;

/// <summary>
/// Handles the visual and audio feedback for the ClearCounter.
/// Plays a sound when an item is placed on it.
/// </summary>
[RequireComponent(typeof(ClearCounterController))]
public class ClearCounterView : CounterView
{
    public AudioSO placeSound;
    [SerializeField] AudioSource audioSource;

    /// <summary>
    /// Subscribes to the model's item change event to trigger audio feedback.
    /// </summary>
    protected override void Initialize() => _model.OnItemChanged += OnItemPlaced;


    /// <summary>
    /// Triggered when an item is placed on the counter. Plays a placement sound effect.
    /// </summary>
    /// <param name="ko">The KitchenObject that was placed. Can be null if an item was picked up.</param>
    public void OnItemPlaced(KitchenObjectController ko)
    {
        if (ko != null)
            AudioManager.Instance.PlaySFX(placeSound, audioSource);
    }

    /// <summary>
    /// Ensures the AudioSource component is assigned.
    /// </summary>
    protected override void SetupComponents() => audioSource = audioSource != null ? audioSource : GetComponentInChildren<AudioSource>();
}