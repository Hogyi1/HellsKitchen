using UnityEngine;
using UnityEngine.Events;
public class ShotgunScript : MonoBehaviour
{
    [Header("Raycast Settings")]
    public KeyCode castKey = KeyCode.E;
    public float rayDistance = 10f;
    public LayerMask enemyLayer;
    public int ammocount = 2;
    public bool isEnabled = false;

    [Header("Cooldown")]
    public float cooldownTime = 10f;
    private CountDownTimer cooldownTimer;

    void Start()
    {
        cooldownTimer = new CountDownTimer(cooldownTime);
    }


    void Update()
    {
        cooldownTimer.Tick();

        if (!cooldownTimer.IsRunning && Input.GetKeyDown(castKey) && ammocount > 0)
        {         
            ammocount--;
            Debug.Log($"Shooting, ammo left: {ammocount}");
            CastRay();
        }
    }


    void CastRay()
    {
        cooldownTimer.Reset();
        cooldownTimer.Start();


        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;


        if (Physics.Raycast(origin, direction, out hit, rayDistance, enemyLayer))
        {

            hit.transform.TryGetComponent<IShotable>(out IShotable target);
            target?.OnHit();
        }
    }

    public void SetAmmoCount(int ac)
    {
        ammocount = ac;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * rayDistance);
        Gizmos.DrawSphere(transform.position + transform.forward * rayDistance, 0.1f);
    }
}
