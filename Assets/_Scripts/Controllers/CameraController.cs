using Unity.Cinemachine;
using UnityEngine;

// Add extra features for special effects custom blending path etc
public class CameraController : Singleton<CameraController>
{
    [SerializeField] CinemachineBrain brain;
    [SerializeField] CinemachineCamera firstPersonCamera;

    [SerializeField] Camera uiCamera;
    [SerializeField] Camera mainCamera;
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

    private void Update()
    {
        uiCamera.fieldOfView = mainCamera.fieldOfView;
    }

    public override void BaseAwake()
    {
        brain = brain != null ? brain : GetComponentInChildren<CinemachineBrain>();
        mainCamera = mainCamera != null ? mainCamera : brain.GetComponent<Camera>();
        uiCamera = uiCamera != null ? uiCamera : brain.GetComponentInChildren<Camera>();
        firstPersonCamera.Priority = activePriority;
    }
}
