using System;
using UnityEngine;

public class PowerBoxScript : MonoBehaviour
{
    [Header("Settings")]
    public KeyCode interactKey = KeyCode.E;
    public KeyCode confirmKey = KeyCode.F;
    public Animator animator;

    public bool powerPreviouslyOut = false; 

    public event Action OnPowerRestoreEvent = delegate { };

    private bool isInteracting = false;
    private bool stepOneDone = false;


    void Update()
    {
        if (!isInteracting && Input.GetKeyDown(interactKey) && (!powerPreviouslyOut))
        {
            StartInteraction();
        }


        if (isInteracting && stepOneDone && Input.GetKeyDown(confirmKey))
        {
            CompleteInteraction();
        }
    }


    void StartInteraction()
    {
        isInteracting = true;
        if (animator != null)
        {
            animator.SetTrigger("Step1");
        }
    }


    void CompleteInteraction()
    {
        if (animator != null)
        {
            animator.SetTrigger("Step2");
        }
       
        OnPowerRestoreEvent?.Invoke();
        isInteracting = false;
        stepOneDone = false;
        powerPreviouslyOut = true; 
    }


    // Ez hívható animációs eventbõl a Step1 animáció végén
    public void OnStep1AnimationEnd()
    {
        stepOneDone = true;
    }
}
