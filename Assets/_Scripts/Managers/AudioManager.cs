using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;
using static Unity.VisualScripting.Member;

/// <summary>
/// A centralized audio manager that handles playback of music and sound effects.
/// It uses an object pool for efficient playback of 3D sound effects.
/// </summary>
public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [Tooltip("The AudioSource for the primary music track.")]
    [SerializeField] private AudioSource musicSource1;
    [Tooltip("The AudioSource for crossfading to the secondary music track.")]
    [SerializeField] private AudioSource musicSource2;
    [Tooltip("The AudioSource for 2D UI sounds.")]
    [SerializeField] private AudioSource uiSfxSource;
    [Tooltip("The AudioSource for quick feedback.")]
    [SerializeField] private AudioSource feedbackSource;

    private ObjectPool<AudioSource> sfxPool;
    private bool isFirstMusicSourceActive = true;
    private Coroutine activeMusicFade;
    private Dictionary<int, AudioSource> activeSources = new();
    private Dictionary<AudioSource, Tween> activeTweens = new();

    protected override void Awake()
    {
        base.Awake();

        sfxPool = new ObjectPool<AudioSource>(
            createFunc: () =>
            {
                var go = new GameObject("Pooled_AudioSource");
                go.transform.SetParent(transform);
                return go.AddComponent<AudioSource>();
            },
            actionOnGet: (source) => source.gameObject.SetActive(true),
            actionOnRelease: (source) => source.gameObject.SetActive(false),
            actionOnDestroy: (source) => Destroy(source.gameObject),
            collectionCheck: true, defaultCapacity: 10, maxSize: 25
        );
    }

    #region Public Play Methods

    /// <summary>
    /// Plays a sound effect at a specific position in the world. Uses the pool.
    /// </summary>
    /// <param name="audioSO">The audio event to play.</param>
    /// <param name="position">The world position to play the sound at.</param>
    public int PlaySFX(AudioSO audioSO, Vector3 position)
    {
        if (audioSO == null) return -1;
        int ID = IDGenerator.GenerateID();
        AudioSource source = sfxPool.Get();
        source.transform.position = position;
        ConfigureAndPlay(audioSO, source);

        if (!audioSO.Loop)
        {
            StartCoroutine(ReturnToPoolAfterPlaying(source, ID));
        }

        activeSources.Add(ID, source);
        return ID;
    }

    /// <summary>
    /// Plays a sound on a pre-existing AudioSource component. Does not use the pool.
    /// Ideal for sounds tied to a specific object's lifecycle (e.g., engine sound).
    /// </summary>
    /// <param name="audioSO">The audio event to play.</param>
    /// <param name="source">The dedicated AudioSource to use.</param>
    public void PlaySFX(AudioSO audioSO, AudioSource source)
    {
        if (audioSO == null || source == null) return;
        ConfigureAndPlay(audioSO, source);
    }

    /// <summary>
    /// Plays a simple 2D sound effect, ideal for UI.
    /// </summary>
    /// <param name="audioSO">The audio event to play.</param>
    public void PlaySFXUI(AudioSO audioSO)
    {
        if (audioSO == null) return;
        // PlayOneShot is used for UI sounds so they can overlap without cutting each other off.
        uiSfxSource.outputAudioMixerGroup = audioSO.Output;
        uiSfxSource.PlayOneShot(audioSO.GetRandomClip(), audioSO.Volume);
    }

    /// <summary>
    /// Plays a music track with an optional crossfade.
    /// </summary>
    /// <param name="musicSO">The music track to play.</param>
    /// <param name="fadeDuration">How long the crossfade should take.</param>
    public void PlayMusic(AudioSO musicSO, float fadeDuration = 1f)
    {
        if (activeMusicFade != null)
        {
            StopCoroutine(activeMusicFade);
        }
        activeMusicFade = StartCoroutine(CrossfadeMusic(musicSO, fadeDuration));
    }

    public bool StopSFX(AudioSO audioSO, AudioSource source)
    {
        if (!source.isPlaying)
            return false;

        activeTweens.TryGetValue(source, out Tween tween);
        tween?.Kill();

        if (audioSO.Fade)
            StopSFX(source, audioSO.fadeTime);
        else
            source.Stop();

        return true;
    }

    public bool StopSFX(AudioSO audioSO, int audioID)
    {
        bool hasKey = activeSources.ContainsKey(audioID);
        if (!hasKey)
            return false;

        AudioSource source = activeSources[audioID];
        activeSources.Remove(audioID);

        StopSFX(audioSO, source);
        return true;
    }

    public bool StopSFX(AudioSource source)
    {
        activeTweens.TryGetValue(source, out Tween tween);
        tween?.Kill();

        source.Stop();
        return true;
    }

    public bool StopSFX(AudioSource source, float fadeTime)
    {
        if (!source.isPlaying)
            return false;

        Tween outTween = DOTween.To(() => source.volume,
                x => source.volume = x,
                0f,
                fadeTime)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    source.Stop();
                    activeTweens.Remove(source);
                });

        activeTweens[source] = outTween;
        return true;
    }

    #endregion

    #region Internal Logic

    private void ConfigureAndPlay(AudioSO audioSO, AudioSource source)
    {
        source.clip = audioSO.GetRandomClip();
        source.volume = audioSO.Volume;
        source.pitch = Random.Range(audioSO.PitchMin, audioSO.PitchMax);
        source.loop = audioSO.Loop;
        if (audioSO.Output != null)
            source.outputAudioMixerGroup = audioSO.Output;

        activeTweens.TryGetValue(source, out Tween tween);
        tween?.Kill();

        if (audioSO.Fade)
        {
            source.volume = 0f;
            Tween inTween = DOTween.To(() => source.volume,
                x => source.volume = x,
                audioSO.Volume,
                audioSO.fadeTime)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => activeTweens.Remove(source));

            activeTweens[source] = inTween;
        }
        source.Play();
    }

    private IEnumerator ReturnToPoolAfterPlaying(AudioSource source, int ID)
    {
        yield return new WaitWhile(() => source.isPlaying);
        sfxPool.Release(source);
        activeSources.Remove(ID);
    }

    private IEnumerator CrossfadeMusic(AudioSO musicSO, float duration)
    {
        AudioSource oldSource = isFirstMusicSourceActive ? musicSource1 : musicSource2;
        AudioSource newSource = isFirstMusicSourceActive ? musicSource2 : musicSource1;
        isFirstMusicSourceActive = !isFirstMusicSourceActive;

        // Configure and start the new music track
        ConfigureAndPlay(musicSO, newSource);

        float time = 0f;
        float oldSourceStartVolume = oldSource.volume;
        float newSourceTargetVolume = newSource.volume; // Volume is set by ConfigureAndPlay
        newSource.volume = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float progress = time / duration;
            newSource.volume = Mathf.Lerp(0, newSourceTargetVolume, progress);
            oldSource.volume = Mathf.Lerp(oldSourceStartVolume, 0, progress);
            yield return null;
        }

        oldSource.Stop();
        oldSource.clip = null;
        newSource.volume = newSourceTargetVolume;
        activeMusicFade = null;
    }

    public override void BaseAwake()
    {
    }

    #endregion
}
