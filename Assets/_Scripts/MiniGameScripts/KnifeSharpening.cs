using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.Cinemachine;
using System;

public class KnifeSharpening : PatternMinigame
{
    public Image pointer;
    public TextMeshProUGUI counterText;
    public Image circle;
    public CinemachineCamera cam;

    public Vector2 circleCenter = new Vector2(-308.69f, 105.33f);
    public float radius = 100f;
    public float rotationSpeed = 90f;

    float currentAngle = 0f;
    float circleAngle = 0f;
    public float circleSpeed = -90f;
    int successCount = 0;

    RectTransform pointerRect, circleRect;

    float greenStartAngleLocal = 54f;
    float greenEndAngleLocal = 126f;

    private bool activated = false;

    private bool completed = false;

    void Start()
    {
        pointerRect = pointer.rectTransform;
        circleRect = circle.rectTransform;
        parentCanvas = GetComponentInParent<Canvas>();
        counterText.text = "0/4";
    }

    void Update()
    {
        if (!activated) return;
        float rad = currentAngle * Mathf.Deg2Rad;

        Vector2 pos = circleCenter + new Vector2(Mathf.Sin(rad), Mathf.Cos(rad)) * radius;
        pointerRect.anchoredPosition = pos;

        Vector2 dir = circleCenter - pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180f;
        pointerRect.localEulerAngles = new Vector3(0, 0, angle);

        
        currentAngle += rotationSpeed * Time.deltaTime;
        currentAngle %= 360f;

        if (Input.GetMouseButtonDown(0))
        {
            
            Vector2 pointerPosLocal = pointerRect.anchoredPosition - circleCenter;
            float pointerAngle = Mathf.Atan2(pointerPosLocal.y, pointerPosLocal.x) * Mathf.Rad2Deg;
            pointerAngle = (pointerAngle + 360f) % 360f;

            
            float greenStartWorld = (greenStartAngleLocal + circleAngle) % 360f;
            float greenEndWorld = (greenEndAngleLocal + circleAngle) % 360f;

            bool inGreen = AngleInRange(pointerAngle, greenStartWorld, greenEndWorld);

            if (inGreen && successCount < 4)
            {
                successCount++;
                counterText.text = $"{successCount}/4";
            }

            
            circleAngle = (circleAngle + UnityEngine.Random.Range(0f, 360f)) % 360;
            circleRect.localEulerAngles = new Vector3(0, 0, circleAngle);

            
        }
        if (successCount == 4 && !completed)
        {
            GameCompleted();
            completed = true;
        }
        if (successCount >= 2)
        {
            circleAngle = (circleAngle + circleSpeed * Time.deltaTime * ((float)(successCount / 5) + 1)) % 360;
            circleRect.localEulerAngles = new Vector3(0, 0, circleAngle);
        }
    }

    bool AngleInRange(float angle, float start, float end)
    {

        if (start <= end)
        {
            Debug.Log("debug: " + angle + ", " + start + ", " + end);
            return angle >= start && angle <= end;
        }

        return angle >= start || angle <= end;
    }

    public override void StartGame()
    {
        activated = true;
        parentCanvas.enabled = true;
    }

    public override void EndGame()
    {
        successCount = 0;
        activated = false;
        parentCanvas.enabled = false;
    }

    public override CinemachineCamera GetCamera()
    {
        return cam;
    }
}
