using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

// Példa egy egyszerű kis játékra
public class Minigame : MonoBehaviour, IInteractable
{
    [SerializeField] CinemachineCamera cam;
    [SerializeField] InputHandler input;

    public void OnInteract(Interactor interactor)
    {
        // Can interact etc
        input.SwitchToMap(InputHandler.ActionMap.Minigame);
        input.Exit += StopInteraction;
        input.MyInteract += Handle;
        Debug.Log("OnInteract");
    }

    public void StopInteraction()
    {
        // Deregister
        input.Exit -= StopInteraction;
        input.MyInteract -= Handle;


        StartCoroutine(WaitForBlend()); // Érdemes megvárni míg visszamegy a kamera annak érdekében hogy ne mozoghassunk el ameddig nem tért vissza a kamera
    }
    IEnumerator WaitForBlend()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => !cam.IsParticipatingInBlend());
        input.SwitchToMap(InputHandler.ActionMap.FirstPerson);
    }
    void Handle(bool pressed)
    {
        if (pressed)
            Debug.Log("Interacted");
    }
}