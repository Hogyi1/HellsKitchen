using System.Collections.Generic;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;

    [Header("Settings")]
    public float spawnInterval = 5f;      // Mennyi idõközönként próbálkozzon
    [Range(0f, 100f)]
    public float spawnChancePercent = 1f; // 1% esély spawnra

    [Header("Enable/Disable Enemies")]
    public bool canSpawnEnemy1 = true;
    public bool canSpawnEnemy2 = true;

    [Header("Spawn Position")]
    public Transform spawnPoint; // Hol spawnoljon az enemy

    private List<GameObject> activeEnemies = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating(nameof(TrySpawnEnemy), spawnInterval, spawnInterval);
    }

    void TrySpawnEnemy()
    {
        // Ha már két enemy aktív, ne spawnoljon
        activeEnemies.RemoveAll(item => item == null); // törli a már elpusztult enemy-ket
        if (activeEnemies.Count >= 2)
            return;

        float roll = Random.Range(0f, 100f);
        if (roll > spawnChancePercent)
            return; // nem spawnol semmi

        // Lista az engedélyezett enemy-bõl, de mindig max 1 elem
        List<GameObject> possibleEnemies = new List<GameObject>();
        if (canSpawnEnemy1 && enemy1Prefab != null)
            possibleEnemies.Add(enemy1Prefab);
        if (canSpawnEnemy2 && enemy2Prefab != null)
            possibleEnemies.Add(enemy2Prefab);

        if (possibleEnemies.Count == 0)
            return; // nincs spawnolható enemy

        // Véletlenszerûen kiválasztjuk melyik jön létre
        GameObject toSpawn = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
        GameObject spawned = Instantiate(toSpawn, spawnPoint.position, spawnPoint.rotation);

        activeEnemies.Add(spawned); // hozzáadjuk a listához
        Debug.Log(spawned.name + " spawned! Total active: " + activeEnemies.Count);
    }
}
