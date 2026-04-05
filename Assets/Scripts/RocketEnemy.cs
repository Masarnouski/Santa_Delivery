using UnityEngine;
public class RocketEnemy : MonoBehaviour {
    public float speed = 6f;
    float killX; bool dead = false;
    SantaController santa; CircleCollider2D col;
void Start() { transform.rotation = Quaternion.Euler(0f, 0f, 90f); Camera cam = Camera.main; killX = cam.transform.position.x - cam.orthographicSize * cam.aspect - 4f; santa = FindObjectOfType<SantaController>(); col = GetComponent<CircleCollider2D>(); }
    void Update() {
        if (dead) return;
        transform.position += Vector3.left * speed * Time.deltaTime;
        if (transform.position.x < killX) { gameObject.SetActive(false); return; }
        if (santa == null) return;
        float r = col != null ? col.radius * transform.localScale.x : 0.3f;
        if (Vector2.Distance(transform.position, santa.transform.position) < r + 0.5f) {
            dead = true; santa.OnHitByEnemy(); gameObject.SetActive(false);
        }
    }
}
