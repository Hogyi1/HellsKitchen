using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// A ScriptableObject that defines the properties of a sound event.
/// </summary>
[CreateAssetMenu(fileName = "NewAudioEvent", menuName = "Game/Audio/Audio Event")]
public class AudioSO : ScriptableObject
{
    [Header("Clips")]
    [Tooltip("The audio clips to be played. A random one will be chosen if more than one.")]
    public AudioClip[] Clips;

    [Header("Sound Settings")]
    [Tooltip("The mixer group to route this audio to (e.g., SFX, Music).")]
    public AudioMixerGroup Output;

    [Tooltip("The base volume of the sound.")]
    [Range(0f, 1f)] public float Volume = 1f;

    [Tooltip("The minimum pitch of the sound.")]
    [Range(0.1f, 3f)] public float PitchMin = 1f;

    [Tooltip("The maximum pitch of the sound.")]
    [Range(0.1f, 3f)] public float PitchMax = 1f;

    [Tooltip("Should the sound loop? Looping sounds from the pool must be manually stopped and returned.")]
    public bool Loop = false;

    public bool Fade = false;
    public float fadeTime = 0f;

    public AudioClip GetRandomClip()
    {
        if (Clips == null || Clips.Length == 0) return null;
        return Clips[Random.Range(0, Clips.Length)];
    }
}