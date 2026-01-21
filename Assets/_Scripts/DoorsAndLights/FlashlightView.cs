using System;
using UnityEngine;
using UnityEngine.UI;

public class FlashlightView : MonoBehaviour
{
    [SerializeField] private FlashlightScript flashlight;
    [SerializeField] private Slider flashlightProgress;
    private Image fillImage;
    void Start()
    {
        flashlight.OnProgress += ProgressDecrese;
        fillImage = flashlightProgress.fillRect.GetComponent<Image>();
    }

    private void ProgressDecrese(float progressValue)
    {
        flashlightProgress.value = progressValue;
        if(progressValue < 0.6f &&progressValue > 0.3f)
        {
            fillImage.color = Color.yellow;
        }
        else if(progressValue <=0.3f)
        {
            fillImage.color = Color.red;
        }
    }
}
