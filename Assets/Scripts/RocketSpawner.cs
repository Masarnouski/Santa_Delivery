using UnityEngine;
public class RocketSpawner : MonoBehaviour {
    public GameObject rocketPrefab;
    public float spawnInterval = 5f;
    public float minY = 1f;
    public float maxY = 5f;
    float timer; Camera cam;
void Start() { cam = Camera.main; timer = spawnInterval * 0.5f; }
void Update() { float elapsed = Time.timeSinceLevelLoad; float t = Mathf.Clamp01(elapsed / 30f); float currentInterval = Mathf.Lerp(spawnInterval, 1f, t); float currentSpeed = Mathf.Lerp(6f, 14f, t); timer += Time.deltaTime; if (timer < currentInterval) return; timer = 0f; if (rocketPrefab == null) return; float spawnX = cam.transform.position.x + cam.orthographicSize * cam.aspect + 1.5f; float spawnY = Random.Range(minY, maxY); GameObject r = Instantiate(rocketPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity); RocketEnemy re = r.GetComponent<RocketEnemy>(); if (re != null) re.speed = currentSpeed; }
}
