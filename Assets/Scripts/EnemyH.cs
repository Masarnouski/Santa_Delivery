using UnityEngine;

public class EnemyH : MonoBehaviour
{
    public float speed = 4f;

    float killX;
    bool dead = false;
    SantaController santaCache;
    CircleCollider2D col;

    void Start()
    {
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
        transform.position = p;

        if (p.x < killX) { dead = true; gameObject.SetActive(false); return; }

        if (santaCache == null) return;
        float r = col != null ? col.radius * transform.localScale.x : 0.25f;
        float dist = Vector2.Distance(p, santaCache.transform.position);
        if (dist < r + 0.5f) { dead = true; santaCache.OnHitByEnemy(); gameObject.SetActive(false); }
    }
}
