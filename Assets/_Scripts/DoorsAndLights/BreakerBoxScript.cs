using System;
using System.Collections;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class PowerBoxScript : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private Animator DoorAnimator;
    [SerializeField] private Animator LeverAnimator;
    [SerializeField] private AudioSO openConfig;
    [SerializeField] private AudioSO closeConfig;
    [SerializeField] private AudioSource openCloseSource;

    public event Action OnPowerRestoreEvent = delegate { };

    private bool stepOneDone = false;
    public bool isActive { get; set; }
    private CountDownTimer timer = new CountDownTimer(4.5f);
    private void StartInteraction()
    {
        Debug.Log("Start");
        AudioManager.Instance.PlaySFX(openConfig, openCloseSource);
        DoorAnimator.SetTrigger("Open");
            
        stepOneDone = true;
        timer.Start();
    }


    private IEnumerator CompleteInteraction()
    {
        timer.Start();
        Debug.Log("Continnue");
        AudioManager.Instance.PlaySFX(closeConfig, openCloseSource);
        LeverAnimator.SetTrigger("PullDown");
        
        yield return new WaitForSeconds(0.6f);
        DoorAnimator.SetTrigger("Close");

        OnPowerRestoreEvent?.Invoke();
        stepOneDone = false;
    }
    private IEnumerator ResetAnim(float sec)
    {
        yield return new WaitForSeconds(sec);
        LeverAnimator.SetTrigger("Reset");
        DoorAnimator.SetTrigger("Reset");
        
    }
    public InteractionResult TryInteract(PlayerController context)
    {
        if (!CanInteract(context)) return InteractionResult.Fail(""); 
        if (!stepOneDone && !timer.IsRunning)
        {
            StartInteraction();
            return InteractionResult.Ok("Started interacting");
        }
        else if(!timer.IsRunning)
        {
            StartCoroutine(CompleteInteraction());
            StartCoroutine(ResetAnim(4f));
           
            return InteractionResult.Ok("Completed interaction");
        }
        return InteractionResult.Fail("Too frequent clicks");
    }

    public bool CanInteract(PlayerController context)
    {
        return isActive;
    }
}
