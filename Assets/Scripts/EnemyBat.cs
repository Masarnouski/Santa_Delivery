using UnityEngine;
public class EnemyBat : MonoBehaviour {
    public float speed = 4f;
    float killX; bool dead = false; SantaController santaCache; CircleCollider2D col;
    void Start() {
        Camera cam = Camera.main;
        killX = cam.transform.position.x - cam.orthographicSize * cam.aspect - 4f;
        santaCache = FindObjectOfType<SantaController>();
        col = GetComponent<CircleCollider2D>();
        // Лёгкое покачивание по Y для красоты
        StartCoroutine(Bob());
    }
    System.Collections.IEnumerator Bob() {
        float baseY = transform.position.y; float t = Random.Range(0f, 6.28f);
        while (true) { t += Time.deltaTime * 3f; transform.position = new Vector3(transform.position.x, baseY + Mathf.Sin(t) * 0.15f, 0f); yield return null; }
    }
    void Update() {
        if (dead) return;
        Vector3 p = transform.position; p.x -= speed * Time.deltaTime; transform.position = p;
        if (p.x < killX) { dead = true; gameObject.SetActive(false); return; }
        if (santaCache == null) return;
        float r = col != null ? col.radius * transform.localScale.x : 0.25f;
        if (Vector2.Distance(p, santaCache.transform.position) < r + 0.5f) { dead = true; santaCache.OnHitByEnemy(); gameObject.SetActive(false); }
    }
}
