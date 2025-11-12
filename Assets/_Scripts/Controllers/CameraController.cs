using Unity.Cinemachine;
using UnityEngine;

// Add extra features for special effects custom blending path etc
public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; } // Call this to get the instance

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        brain = brain != null ? brain : GetComponentInChildren<CinemachineBrain>();
        firstPersonCamera.Priority = 100;
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    [SerializeField] CinemachineBrain brain;
    [SerializeField] CinemachineCamera firstPersonCamera;
    readonly int activePriority = 100;

    public void RequestFocus(CinemachineCamera camera)
    {
        if (camera == null) return;
        camera.Priority = activePriority + 1;
    }

    public void ReleaseFocus(CinemachineCamera camera)
    {
        if (camera == null) return;
        camera.Priority = 0;
    }

    public bool IsBlending() => brain.IsBlending;
}
