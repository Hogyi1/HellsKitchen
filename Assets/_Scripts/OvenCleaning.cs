using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class OvenCleaning : MonoBehaviour
{
    [Range(0f, 1f)] public float cleanProgress = 0f;
    public float cleanSpeed = 0.25f;
    public Sprite cleanOvenSprite;

    Image img;
    bool isDirty = false;
    bool cleanStarted = false;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    public void ResetCleaning()
    {
        cleanProgress = 0f;
        isDirty = true;
        cleanStarted = false;
        Color c = img.color;
        c.a  = 1f;
        img.color = c;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Sponge") || !isDirty) return;

        Debug.Log("Sponge over oven");
        cleanProgress += cleanSpeed * Time.deltaTime;
        cleanProgress = Mathf.Clamp01(cleanProgress);

        if (cleanProgress > 0.01f && !cleanStarted)
        {
            cleanStarted = true;
        }

        Color c = img.color;
        c.a = 1f - cleanProgress;
        img.color = c;

        if (cleanProgress >= 1f)
        {
            img.sprite = cleanOvenSprite;
            img.color = Color.white;
            isDirty = false;
        }
    }
}
