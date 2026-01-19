using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DispenserController))]
public class DispenserView : CounterView
{
    [SerializeField] AudioSource audioSoruce;
    [SerializeField] ParticleSystem particles;
    [SerializeField] AudioSO refillAudio;

    public float PlateHeight = 0.05f;
    public float MoveTime = 0.2f;
    public float ParticleDuration = 1.0f;
    private DispenserModel _myModel => GetModel<DispenserModel>();
    private CountDownTimer _particleTimer;

    protected override void SetupComponents()
    {
        audioSoruce = audioSoruce != null ? audioSoruce : GetComponentInChildren<AudioSource>();
        particles = particles != null ? particles : GetComponentInChildren<ParticleSystem>();

        _particleTimer = new(ParticleDuration);
        _particleTimer.OnTimerStop += () =>
        {
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        };

        particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    protected override void Initialize()
    {
        _myModel.OnPlateAdded += PlayRefill;
    }

    private void PlayRefill(PlateObjectController controller)
    {
        AudioManager.Instance.PlaySFX(refillAudio, audioSoruce);
        particles.Play();
        _particleTimer.Start();
    }

    public void AdjustPlateheight()
    {
        List<PlateObjectController> plates = _myModel.GetPlates();
        plates.Reverse();
        for (int i = 0; i < plates.Count; i++)
        {
            var targetPosition = new Vector3(plates[i].transform.localPosition.x,
                                             PlateHeight * i,
                                             plates[i].transform.localPosition.z);
            plates[i].transform.DOLocalMove(targetPosition, MoveTime).SetEase(Ease.OutQuad);
        }
    }
}
