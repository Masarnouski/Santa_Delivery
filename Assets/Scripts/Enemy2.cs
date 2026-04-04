using UnityEngine;

public class Enemy2 : MonoBehaviour
{
    public float speed = 4f;
    public float wobbleAmp = 0.15f;
    public float wobbleFreq = 8f;

    float startY;
    float timeOffset;
    float killX;
    bool dead = false;
    SantaController santaCache;
    CircleCollider2D col;

    void Start()
    {
        startY = transform.position.y;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
        Camera cam = Camera.main;
        killX = cam.transform.position.x - cam.orthographicSize * cam.aspect - 4f;
        santaCache = FindObjectOfType<SantaController>();
        col = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (dead) return;

        Vector3 p = transform.position;
        p.x -= speed * Time.deltaTime;
        p.y = startY + Mathf.Sin(Time.time * wobbleFreq + timeOffset) * wobbleAmp;
        transform.position = p;

        // Небольшой наклон в зависимости от вертикального движения
        float tilt = Mathf.Cos(Time.time * wobbleFreq + timeOffset) * wobbleAmp * wobbleFreq * 20f;
        transform.rotation = Quaternion.Euler(0f, 0f, tilt);

        if (p.x < killX) { dead = true; gameObject.SetActive(false); return; }

        if (santaCache == null) return;
        float r = col != null ? col.radius * transform.localScale.x : 0.25f;
        float dist = Vector2.Distance(p, santaCache.transform.position);
        if (dist < r + 0.5f) { dead = true; santaCache.OnHitByEnemy(); gameObject.SetActive(false); }
    }
}
