using UnityEngine;

public class EnemyBird : MonoBehaviour
{
    public float speedX = 4f;
    public float amplitude = 1.8f;
    public float frequency = 2f;

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
        p.x -= speedX * Time.deltaTime;
        p.y = startY + Mathf.Sin(Time.time * frequency + timeOffset) * amplitude;
        transform.position = p;

        // Flip спрайта не нужен — он всегда летит влево
        float r = col != null ? col.radius * transform.localScale.x : 0.25f;
        if (santaCache != null)
        {
            float dist = Vector2.Distance(p, santaCache.transform.position);
            if (dist < r + 0.5f)
            {
                dead = true;
                santaCache.OnHitByEnemy();
                gameObject.SetActive(false);
                return;
            }
        }

        if (p.x < killX)
        {
            dead = true;
            gameObject.SetActive(false);
        }
    }
}
