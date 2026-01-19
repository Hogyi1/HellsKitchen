using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class OvenCleaning : PatternMinigame
{
    [Range(0f, 1f)] public float cleanProgress = 0f;
    public float cleanSpeed = 0.25f;
    public Sprite cleanOvenSprite;
    [SerializeField] private CinemachineCamera cam;

    private Image img;
    private bool isDirty = false;
    
    void Awake()
    {
        img = GetComponent<Image>();
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void ResetCleaning()
    {
        cleanProgress = 0f;
        isDirty = true;
        Color c = img.color;
        c.a  = 1f;
        img.color = c;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Sponge") || !isDirty) return;

        Debug.Log("Sponge over oven " +isDirty );
        cleanProgress = Mathf.Clamp01(cleanProgress + cleanSpeed * Time.deltaTime);
        
        Color c = img.color;
        c.a = 1f - cleanProgress;
        img.color = c;

        if (cleanProgress >= 1f)
        {
            img.sprite = cleanOvenSprite;
            img.color = Color.white;
            isDirty = false;
            GameCompleted();
        }
    }
    public override void StartGame()
    {
        parentCanvas.enabled = true;  
    }

    public override void EndGame()
    {
        parentCanvas.enabled = false;
        ResetCleaning();
    }

    public override CinemachineCamera GetCamera()
    {
        return cam;
    }
}
