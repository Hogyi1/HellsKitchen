using TMPro;
using UnityEngine;

public class ShotgunView : MonoBehaviour
{
    [SerializeField] private ShotgunScript shotgun;
    [SerializeField] private TMP_Text ammoText;
    private int maxAmmo = 0;
    void Start()
    {
        maxAmmo = shotgun.ammocount;
        shotgun.OnShoot += ShotgunOnShoot;
        ammoText.text = $"Ammo: {maxAmmo}/{maxAmmo}";
    }

    private void ShotgunOnShoot(int ammoCount)
    {
        
        ammoText.text = $"Ammo: {ammoCount}/{maxAmmo}";
    }
}
