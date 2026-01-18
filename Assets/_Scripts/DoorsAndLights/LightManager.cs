using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] private Transform lightParent;
    [SerializeField] private AudioSO powerOutConfig;
    [SerializeField] private AudioSO powerBackConfig;
    [SerializeField] private AudioSource audioSource;

    private List<Light> allTheLights;
    private CountDownTimer cd;
    void Awake()
    {
        int n = lightParent.childCount;
        allTheLights = new List<Light>();
        for (int i = 0; i < n; i++) allTheLights.Add(lightParent.GetChild(i).GetComponent<Light>());
    }
    private void Start()
    {
        cd = new CountDownTimer(2.5f);
    }
    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(0f));
    }

    public void FadeIn()
    {
        StartCoroutine(FadeRoutine(1f));
    }
    private IEnumerator FadeRoutine(float target)
    {
        cd.Start();

        float[] startIntensities = new float[allTheLights.Count];
        for (int i = 0; i < allTheLights.Count; i++)
        {
            startIntensities[i] = allTheLights[i].intensity;
        }

        if (target == 0)
            AudioManager.Instance.PlaySFX(powerOutConfig, audioSource);
        else
            AudioManager.Instance.PlaySFX(powerBackConfig, audioSource);

        while (!cd.IsFinished)
        {
            float t = 1 - cd.Progress;

            for (int i = 0; i < allTheLights.Count; i++)
            {
                allTheLights[i].intensity =
                    Mathf.Lerp(startIntensities[i], target, t);
            }

            yield return null;
        }

        foreach (var l in allTheLights)
            l.intensity = target;
    }


}
