using System;
using UnityEngine;
using UnityEngine.Events;
public class ShotgunScript : MonoBehaviour
{
   
    [SerializeField] private KeyCode castKey = KeyCode.F;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private LayerMask enemyLayer;    
    [SerializeField] private float cooldownTime = 10f;
    [SerializeField] private AudioSO shootingAndReloading;
    [SerializeField] private AudioSource source;
    private CountDownTimer cooldownTimer;
    public int ammocount = 3;
    public event Action<int> OnShoot = delegate { };
    void Start()
    {
        cooldownTimer = new CountDownTimer(cooldownTime);
    }
    void Update()
    {
        if (!cooldownTimer.IsRunning && Input.GetKeyDown(castKey) && ammocount > 0)
        {         
            ammocount--;
            AudioManager.Instance.PlaySFX(shootingAndReloading,source);
            CastRay();
            OnShoot?.Invoke(ammocount);
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
}
