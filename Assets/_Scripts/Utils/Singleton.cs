using UnityEngine;

/// <summary>
/// A generic singleton base class for MonoBehaviour components.
/// Creates a globally accessible instance and ensures only one exists.
/// </summary>
/// <typeparam name="T">The type of the component to create a singleton for.</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// The static instance of the singleton.
    /// </summary>
    public static T Instance { get; private set; }

    /// <summary>
    /// On Awake, this method enforces the singleton pattern.
    /// It ensures that only one instance of this component exists.
    /// If an instance already exists, it destroys the new one.
    /// It also marks the singleton to not be destroyed on scene loads.
    /// </summary>
    protected virtual void Awake()
    {
        if (Instance != null && Instance != this as T)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// On Destroy, if this is the singleton instance, it nullifies the static reference.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (Instance == this as T)
        {
            Instance = null;
        }
    }
}
