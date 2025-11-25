using Unity.Cinemachine;
using UnityEngine;

// Add extra features for special effects custom blending path etc
public class CameraController : Singleton<CameraController>
{
    [SerializeField] CinemachineBrain brain;
    [SerializeField] CinemachineCamera firstPersonCamera;
    readonly int activePriority = 100;

    protected override void Awake()
    {
        base.Awake();

        brain = brain != null ? brain : GetComponentInChildren<CinemachineBrain>();
        firstPersonCamera.Priority = activePriority;
    }

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
