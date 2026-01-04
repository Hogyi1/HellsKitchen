using UnityEngine;
using static StoveModel.StoveState;

[RequireComponent(typeof(StoveController))]
public class StoveCounterView : CounterView
{
    public AudioSO stoveFryingSound;
    public AudioSO stoveBurningSound;

    [SerializeField] Light stoveLight;
    [SerializeField] ParticleSystem fireParticles;
    [SerializeField] ParticleSystem smokeParticles;
    [SerializeField] AudioSource audioSource;
    [SerializeField] StoveUIHandler stoveUIHandler;

    public StoveModel Model => GetModel<StoveModel>();

    private void Awake()
    {
        audioSource = audioSource != null ? audioSource : GetComponentInChildren<AudioSource>();
        stoveLight = stoveLight != null ? stoveLight : GetComponentInChildren<Light>();
        fireParticles = fireParticles != null ? fireParticles : GetComponentInChildren<ParticleSystem>();
        smokeParticles = smokeParticles != null ? smokeParticles : GetComponent<ParticleSystem>();
        stoveUIHandler = stoveUIHandler != null ? stoveUIHandler : GetComponentInChildren<StoveUIHandler>();

        stoveUIHandler.BindData(Model);
        Model.OnStateChanged += HandleStateChanged;
        TurnOffStove();
    }

    private void Start()
    {
        fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        smokeParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        stoveLight.enabled = false;
        AudioManager.Instance.StopSFX(audioSource);
    }

    private void HandleStateChanged(StoveModel.StoveState state)
    {
        switch (state)
        {
            case Idle:
                TurnOffStove();
                break;
            case Frying:
                TurnOnStove();
                break;
            case Burnt:
                TurnOnFire();
                break;
        }
    }

    private void TurnOnStove()
    {
        fireParticles.Play();
        smokeParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        stoveLight.enabled = true;
        stoveUIHandler.TurnOn();
        AudioManager.Instance.PlaySFX(stoveFryingSound, audioSource);
    }

    private void TurnOffStove()
    {
        fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        smokeParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        stoveLight.enabled = false;
        stoveUIHandler.TurnOff();
        AudioManager.Instance.StopSFX(audioSource, 1.5f);
    }

    private void TurnOnFire()
    {
        smokeParticles.Play();
        AudioManager.Instance.PlaySFX(stoveBurningSound, audioSource);
    }
}
