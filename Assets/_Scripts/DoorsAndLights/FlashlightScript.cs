using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [SerializeField] private Light flashlight;
    [SerializeField] private float maxBatteryTime = 100f;
    [SerializeField] private float rayDistance = 10f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private KeyCode inputKey = KeyCode.F;
    private float currentBattery;
    private bool isOn = false;
    private LoopTimer rayLoop;
    void Start()
    {
        currentBattery = maxBatteryTime;
        rayLoop = new LoopTimer(1,9999);
        rayLoop.OnLoop += OnLoop;
    }

    void Update()
    {
        if (Input.GetKeyDown(inputKey))
        {
            ToggleFlashlight();
            
        }
    }

    private void OnLoop(int i)
    {
        if (isOn && currentBattery > 0f)
        {
            currentBattery -= 1f;

            if (currentBattery <= 0f)
            {
                currentBattery = 0f;
                TurnOff();
                
            }


            RaycastHit hit;
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;

            
            if (Physics.Raycast(origin, direction, out hit, rayDistance, enemyLayer))
            {

                hit.transform.TryGetComponent(out IStunnable target);
                target?.OnFlashed();
            }
        }
    }
    void ToggleFlashlight()
    {
        if (currentBattery <= 0f)
            return;

        isOn = !isOn;
        flashlight.enabled = isOn;
        if(isOn)
        {
            rayLoop.Start();
        }
        else
        {
            rayLoop.Stop();
        }
    }

    void TurnOff()
    {
        isOn = false;
        flashlight.enabled = false;
    }
}
