using Unity.Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioSettings", menuName = "Game/Player/Settings/AudioSettings")]
public class PlayerAudioSettings : ScriptableObject
{
    [Range(0.0001f, 1f)] public float MasterVolume = 1.0f;
    [Range(0.0001f, 1f)] public float MusicVolume = 0.8f;
    [Range(0.0001f, 1f)] public float SFXVolume = 0.8f;
}