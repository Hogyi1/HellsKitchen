using UnityEngine;

/// <summary>
/// Manages the visual and audio presentation of a KitchenContainer.
/// Handles animations, sprite display, and plays sound effects when objects are spawned.
/// </summary>
[RequireComponent(typeof(ContainerController), typeof(Animator), typeof(AudioSource))]
public class ContainerView : CounterView
{
    /// <summary>
    /// Animator hash for the "OpenClose" animation parameter.
    /// </summary>
    public static int OpenClose = Animator.StringToHash("OpenClose");

    /// <summary>
    /// The Audio ScriptableObject to play when the container interacts (e.g., opens).
    /// </summary>
    public AudioSO openingAudio;

    /// <summary>
    /// The sprite to display for this container.
    /// </summary>
    public Sprite mySprite;

    /// <summary>
    /// Gets the associated KitchenContainer model.
    /// </summary>
    private ContainerModel Model => GetModel<ContainerModel>();

    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] AudioSource audioSource;

    /// <summary>
    /// Unsubscribes from model events when the GameObject is disabled.
    /// </summary>
    private void OnDisable()
    {
        Model.OnObjectSpawned -= InteractedWith;
    }

    /// <summary>
    /// Handles visual and audio feedback when an object is interacted with (e.g., spawned).
    /// </summary>
    /// <param name="ko">The KitchenObject involved in the interaction.</param>
    private void InteractedWith(KitchenObject ko)
    {
        if (ko.GetParent() as ContainerModel != Model)
        {
            anim.Play(OpenClose);
            AudioManager.Instance.PlaySFX(openingAudio, audioSource);
        }
    }

    /// <summary>
    /// Sets up and assigns required components like Animator, SpriteRenderer, and AudioSource.
    /// Also applies the designated sprite.
    /// </summary>
    protected override void SetupComponents()
    {
        anim = anim != null ? anim : gameObject.GetComponent<Animator>();
        spriteRenderer = spriteRenderer != null ? spriteRenderer : gameObject.GetComponentInChildren<SpriteRenderer>();
        audioSource = audioSource != null ? audioSource : gameObject.GetComponentInChildren<AudioSource>();

        if (spriteRenderer != null)
            spriteRenderer.sprite = mySprite;
    }

    /// <summary>
    /// Initializes the view by subscribing to model events and setting initial animation/audio states.
    /// </summary>
    protected override void Initialize()
    {
        Model.OnObjectSpawned += InteractedWith;

        anim.StopPlayback();
        AudioManager.Instance.StopSFX(audioSource);
    }
}