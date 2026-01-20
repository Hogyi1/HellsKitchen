using UnityEngine;

public class LightFlickeringEffect : MonoBehaviour
{
    [Tooltip("Minimum intensity multiplier. Final intensity is base intensity * this value.")]
    [SerializeField] float minIntensity = 0.8f;
    [Tooltip("Maximum intensity multiplier. Final intensity is base intensity * this value.")]
    [SerializeField] float maxIntensity = 1.2f;
    [SerializeField] float frequency = 5f;
    [SerializeField] Light lightSource;

    private float baseIntensity;
    private float seed;
    public bool Animate = true;

    private void Awake()
    {
        lightSource = lightSource != null ? lightSource : GetComponent<Light>();

        if (lightSource != null)
            baseIntensity = lightSource.intensity;

        seed = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        if (lightSource != null && Animate)
        {
            float noise = Mathf.PerlinNoise(seed, Time.time * frequency);
            lightSource.intensity = Mathf.Lerp(baseIntensity * minIntensity, baseIntensity * maxIntensity, noise);
        }
    }
}
