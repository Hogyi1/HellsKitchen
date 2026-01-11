using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : MonoBehaviour
{
    [SerializeField] Transform lightParent;
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
        cd = new CountDownTimer(2f);
    }
    public void FadeOut()
    {
        StartCoroutine(FadeRoutine(0));
    }

    public void FadeIn()
    {
        StartCoroutine(FadeRoutine(1));
    }
    private IEnumerator FadeRoutine(float target)
    {
        cd.Start();

        // playsound
        
        while(!cd.IsFinished)
        {
            float t = cd.Progress;

            foreach (var l in allTheLights)
            {
                l.intensity = Mathf.Lerp(l.intensity, target, t);
            }
            yield return null;

        }

        foreach (var l in allTheLights)
        {
            l.intensity = target;
        }
    }

   
}
