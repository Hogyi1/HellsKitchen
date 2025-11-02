using System;
using Unity.Cinemachine;
using UnityEngine;

// Under construction
public class CameraController : MonoBehaviour
{
    #region Singleton
    public static CameraController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        mainCamera.Priority = mainCamPriority;
    }
    #endregion


    #region Fields
    [SerializeField] private CinemachineCamera mainCamera;
    private CinemachineCamera focusCamera;

    private int activePriority = 20;
    private int mainCamPriority = 10;
    #endregion


    #region Methods
    public void RequestFocus(CinemachineCamera request)
    {
        if (request != null)
        {
            focusCamera = request;
            focusCamera.Priority = activePriority;
            return;
        }
    }

    public void Over()
    {
        focusCamera.Priority = 0;
    }
    #endregion
}
