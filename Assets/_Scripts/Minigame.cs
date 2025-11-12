using NUnit.Framework;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.OSX;

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
        var ir = new InteractionResult(pressed, pressed ? "Interacted" : "No interaction");
    }

    public InteractionResult CanInteract(Interactor context)
    {
        ContextualPredicate<Interactor> cp = new ContextualPredicate<Interactor>(context, (c) => c.enabled);

        var ir = new InteractionResult(context.enabled, "Yee");
        return ir;

    }
}