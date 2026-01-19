using System;
using System.Collections;
using System.Xml.Serialization;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class Wardrobe : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineCamera hidingCam;
    [SerializeField] private Transform exitPos;
    [SerializeField] private Animator animator;
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private AudioSO openCloseConfig;
    [SerializeField] private AudioSource source;
    private CountDownTimer timer = new CountDownTimer(4);
    private bool isDying = false;

    public event Action<bool> OnHide = delegate { };
    public InteractionResult TryInteract(PlayerController context)
    {
       
        if(!timer.IsRunning)
        {
            StartCoroutine(ResetAnim(true));
            
            inputHandler.SwitchToHiding();
            inputHandler.Exit += Exit;

            OnHide?.Invoke(true);
            timer.Start();
        }
        return InteractionResult.Ok(" ");
        
    }

    private void Exit()
    {
        if (!timer.IsRunning && !isDying)
        {
            StartCoroutine(ResetAnim(false));
            
            StartCoroutine(Wait());
            inputHandler.Exit -= Exit;          
            OnHide?.Invoke(false);
            
            timer.Start();
        }
    }


    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.2f);
        CameraController.Instance.ReleaseFocus(hidingCam);
        inputHandler.SwitchToFirstPerson();
    }

    private IEnumerator ResetAnim(bool entering)
    {
        animator.SetBool("CanOpenAndClose", true);
        AudioManager.Instance.PlaySFX(openCloseConfig, source);
        yield return new WaitForSeconds(1f);
        if(entering)
        {
            CameraController.Instance.RequestFocus(hidingCam);
        }
        
        animator.SetBool("CanOpenAndClose", false);
    }
    public bool CanInteract(PlayerController context)
    {
        return true;
    }

    public void Eject()
    {
        isDying = true;
        animator.SetBool("CanOpen", true);
        AudioManager.Instance.PlaySFX(openCloseConfig, source);
    }
}
