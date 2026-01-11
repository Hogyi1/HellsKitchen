using UnityEngine;
using TMPro;
public class TestingScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    float pollingTime = 0.1f;
    uint frameCount = 0;
    float time;

    private void Update()
    {
        time += Time.deltaTime;
        frameCount++;

        if (time >= pollingTime)
        {
            int frameRate = Mathf.RoundToInt(frameCount / time);
            text.text = "FPS: " + frameRate.ToString();


            time -= pollingTime;
            frameCount = 0;
        }
    }
}
