using UnityEngine;
using UnityEngine.Events;

public class Robot02VentScript: MonoBehaviour
{
    public event UnityAction<Transform> OnEnteringVent = delegate { };
    public void OnTriggerEnter(Collider other)
    {
       if(other.CompareTag("Enemy"))
       {
            other.transform.TryGetComponent<ICrawlable>(out ICrawlable target);
            target?.OnVentEnter();
       }
    }
}
