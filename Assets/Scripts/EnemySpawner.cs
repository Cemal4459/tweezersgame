using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float respawnDelay = 2f;

    GameObject current;
    Coroutine respawnRoutine;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("EnemySpawner: enemyPrefab veya spawnPoints eksik.");
            return;
        }

        var p = spawnPoints[Random.Range(0, spawnPoints.Length)];
        current = Instantiate(enemyPrefab, p.position, Quaternion.identity);

        // Ölüm eventine bağlan
        var dummy = current.GetComponent<EnemyDummy>();
        if (dummy != null)
        {
            dummy.OnDead += HandleEnemyDead;
        }
        else
        {
            // EnemyDummy yoksa yine de güvenli respawn
            HandleEnemyDead();
        }
    }

    void HandleEnemyDead()
    {
        if (respawnRoutine != null) StopCoroutine(respawnRoutine);
        respawnRoutine = StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        Spawn();
        respawnRoutine = null;
    }
}