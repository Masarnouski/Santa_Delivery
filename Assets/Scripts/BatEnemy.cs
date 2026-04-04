using UnityEngine;

public class BatEnemy : MonoBehaviour
{
    public float speed = 4f;
    public float wingSpeed = 8f;

    float killX;
    SantaController santaCache;
    CircleCollider2D col;
    bool dead = false;
    float baseY;

    void Start()
    {
        Camera cam = Camera.main;
        killX = cam.transform.position.x - cam.orthographicSize * cam.aspect - 4f;
        santaCache = FindObjectOfType<SantaController>();
        col = GetComponent<CircleCollider2D>();
        baseY = transform.position.y;
    }

    void Update()
    {
        if (dead) return;

        // Горизонтальный полёт
        Vector3 p = transform.position;
        p.x -= speed * Time.deltaTime;
        // Лёгкое вертикальное покачивание
        p.y = baseY + Mathf.Sin(Time.time * 3f) * 0.2f;
        transform.position = p;

        // Анимация крыльев — пульсация по Y scale
        float wing = 1f + Mathf.Abs(Mathf.Sin(Time.time * wingSpeed)) * 0.4f;
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x),
            wing * 0.5f,
            1f
        );

        if (p.x < killX) { dead = true; gameObject.SetActive(false); return; }

        if (santaCache == null) return;
        float r = col != null ? col.radius * 0.5f : 0.3f;
        float dist = Vector2.Distance(p, santaCache.transform.position);
        if (dist < r + 0.5f)
        {
            dead = true;
            santaCache.OnHitByEnemy();
            gameObject.SetActive(false);
        }
    }
}
