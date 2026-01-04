using UnityEngine;

/// <summary>
/// Manages the visual and audio presentation for the Cutting Board.
/// It handles playing cutting animations, sound effects, and controls the cutting UI.
/// </summary>
[RequireComponent(typeof(CuttingController), typeof(Animator), typeof(AudioSource))]
public class CuttingBoardView : CounterView
{
    /// <summary>
    /// The Audio ScriptableObject to play during cutting actions.
    /// </summary>
    public AudioSO cuttingSound;
    /// <summary>
    /// Animator hash for the "Cutting" animation parameter.
    /// </summary>
    public static int CuttingAnimation = Animator.StringToHash("Cutting");

    /// <summary>
    /// Gets the associated CuttingBoard model.
    /// </summary>
    private CuttingModel Model => GetModel<CuttingModel>();

    [SerializeField] AudioSource audioSource;
    [SerializeField] CuttingUIHandler cuttingUIHandler;
    [SerializeField] Animator anim;

    /// <summary>
    /// Plays the cutting animation when a cutting action occurs.
    /// </summary>
    /// <param name="so">The KitchenObjectSO involved in the cutting action.</param>
    private void OnCuttingAction(KitchenObjectSO so) => anim.Play(CuttingAnimation);

    /// <summary>
    /// Toggles the visibility of the cutting UI based on whether an item is on the board.
    /// </summary>
    /// <param name="ko">The KitchenObject currently on the board (null if empty).</param>
    private void OnItemChanged(KitchenObject ko)
    {
        if (ko != null)
            cuttingUIHandler.TurnOn();
        else
            cuttingUIHandler.TurnOff();
    }

    /// <summary>
    /// Plays the cutting sound effect. This method is typically called as an Animation Event.
    /// </summary>
    private void PlayAudioSFX() => AudioManager.Instance.PlaySFX(cuttingSound, audioSource);

    /// <summary>
    /// Unsubscribes from model events when the GameObject is disabled to prevent memory leaks.
    /// </summary>
    private void OnDisable()
    {
        Model.OnItemChanged -= OnItemChanged;
        Model.OnCuttingAction -= OnCuttingAction;
    }

    /// <summary>
    /// Initializes the view by subscribing to model events, binding UI data,
    /// and stopping any lingering audio.
    /// </summary>
    protected override void Initialize()
    {
        AudioManager.Instance.StopSFX(audioSource);

        cuttingUIHandler.BindData(Model);
        Model.OnItemChanged += OnItemChanged;
        Model.OnCuttingAction += OnCuttingAction;
    }

    /// <summary>
    /// Sets up and assigns required components like AudioSource, CuttingUIHandler, and Animator.
    /// </summary>
    protected override void SetupComponents()
    {
        audioSource = audioSource != null ? audioSource : GetComponentInChildren<AudioSource>();
        cuttingUIHandler = cuttingUIHandler != null ? cuttingUIHandler : GetComponentInChildren<CuttingUIHandler>();
        anim = anim != null ? anim : GetComponentInChildren<Animator>();
    }
}