using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject enemy2Prefab;

    public float spawnInterval = 3.5f;
    public float minY = 1f;
    public float maxY = 5f;
    public float speed = 3f;

    float timer;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
        timer = spawnInterval;
    }

void Update() { timer += Time.deltaTime; if (timer < spawnInterval) return; timer = 0f; float spawnX = cam.transform.position.x + cam.orthographicSize * cam.aspect + 3f; float spawnY = Random.Range(minY, maxY); int pick = (enemyPrefab != null && enemy2Prefab != null) ? Random.Range(0, 2) : (enemy2Prefab != null ? 1 : 0); GameObject prefab = (pick == 0) ? enemyPrefab : enemy2Prefab; if (prefab == null) return; GameObject e = Instantiate(prefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity); Enemy en = e.GetComponent<Enemy>(); if (en != null) en.speed = speed; Enemy2 en2 = e.GetComponent<Enemy2>(); if (en2 != null) en2.speed = speed; }
}
