using UnityEngine;

public class RocketSpawner : MonoBehaviour
{
    public GameObject rocketPrefab;

    [Header("Spawn Y Range")]
    public float minY = 1f;
    public float maxY = 5f;

    [Header("Difficulty Curve (over time)")]
    [Tooltip("Time in seconds to reach maximum difficulty")]
    public float rampDuration = 30f;

    [Header("Spawn Interval")]
    [Tooltip("Interval at game start (seconds)")]
    public float intervalStart = 5f;
    [Tooltip("Interval at max difficulty (seconds)")]
    public float intervalEnd = 1f;

    [Header("Rocket Speed")]
    [Tooltip("Speed at game start")]
    public float speedStart = 6f;
    [Tooltip("Speed at max difficulty")]
    public float speedEnd = 14f;

    float timer;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
        timer = intervalStart * 0.5f;
    }

    void Update()
    {
        float t = Mathf.Clamp01(Time.timeSinceLevelLoad / rampDuration);
        float currentInterval = Mathf.Lerp(intervalStart, intervalEnd, t);
        float currentSpeed = Mathf.Lerp(speedStart, speedEnd, t);

        timer += Time.deltaTime;
        if (timer < currentInterval) return;
        timer = 0f;
        if (rocketPrefab == null) return;

        float spawnX = cam.transform.position.x + cam.orthographicSize * cam.aspect + 1.5f;
        float spawnY = Random.Range(minY, maxY);
        GameObject r = Instantiate(rocketPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
        RocketEnemy re = r.GetComponent<RocketEnemy>();
        if (re != null) re.speed = currentSpeed;
    }
}
