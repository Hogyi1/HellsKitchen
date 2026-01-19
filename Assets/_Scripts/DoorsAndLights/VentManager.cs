using System.Collections.Generic;
using UnityEngine;

public class VentManager : MonoBehaviour, IInteractable
{
    private int screwCount;
    [SerializeField] private GameObject[] screws;
    [SerializeField] private GameObject vent;
    [SerializeField] private CountDownTimer timer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSO screwSO;
    [SerializeField] private AudioSO ventSO;
    public bool CanInteract(PlayerController context)
    {
        //context.GetBaseTool();
        // return basetool == wrench)
        return true;
    }

    public void Awake()
    {
        screwCount = screws.Length;
        timer = new CountDownTimer(2);
    }
    public InteractionResult TryInteract(PlayerController context)
    {
        if(!timer.IsRunning)
        {
            if (screwCount == 0)
            {
                Destroy(vent);
                transform.position += new Vector3(0, 100, 0);
                AudioManager.Instance.PlaySFX(ventSO, audioSource);
                return InteractionResult.Ok("Lefutott");             
            }
            Destroy(screws[screwCount - 1]);
            screwCount--;
            timer.Start();
            AudioManager.Instance.PlaySFX(screwSO, audioSource);
            return InteractionResult.Ok("Lefutott");
        }
        return InteractionResult.Ok("Nem futott le.");
    }
}
