using UnityEngine;

public class FlashlightScript : MonoBehaviour
{
    [Header("Flashlight Settings")]
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
        rayLoop = new LoopTimer(1,99999);
        rayLoop.OnLoop += OnLoop;
    }

    void Update()
    {
        if (Input.GetKeyDown(inputKey))
        {
            ToggleFlashlight();
            Debug.Log($"A zseblámpa bekapcsolva és {currentBattery}% battery-nk van.");
        }
    }

    private void OnLoop(int i)
    {
        if (isOn && currentBattery > 0f)
        {
            currentBattery -= 1.25f;

            if (currentBattery <= 0f)
            {
                currentBattery = 0f;
                TurnOff();
                Debug.Log("A zseblámpa lemerült!");
            }


            RaycastHit hit;
            Vector3 origin = transform.position;
            Vector3 direction = transform.forward;

            Debug.Log($"A zseblámpa tickelt és {currentBattery} % battery-nk van.");
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
