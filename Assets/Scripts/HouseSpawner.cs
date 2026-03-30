using UnityEngine;

public class HouseSpawner : MonoBehaviour
{
    public GameObject housePrefab;
    public float spawnInterval = 3f;
    public float scrollSpeed = 2f;
    public float minY = -4f;
    public float maxY = -2f;

    private float timer = 0f;
    private Camera cam;

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
        if (housePrefab == null) return;
        float rightEdge = cam.transform.position.x + cam.orthographicSize * cam.aspect + 8f;
        float y = Random.Range(minY, maxY);
        GameObject h = Instantiate(housePrefab, new Vector3(rightEdge, y, 0f), Quaternion.identity);
        h.AddComponent<HouseMover>().speed = scrollSpeed;
    }
}
