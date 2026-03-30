using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
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

    void Update()
    {
        timer += Time.deltaTime;
        if (timer < spawnInterval) return;
        timer = 0f;
        if (enemyPrefab == null) return;
        float spawnX = cam.transform.position.x + cam.orthographicSize * cam.aspect + 3f;
        float spawnY = Random.Range(minY, maxY);
        GameObject e = Instantiate(enemyPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
        Enemy en = e.GetComponent<Enemy>();
        if (en != null) en.speed = speed;
    }
}
