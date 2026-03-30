using UnityEngine;

public class HouseMover : MonoBehaviour
{
    public float speed = 2f;
    public bool IsTarget { get; private set; } = false;

    private Camera cam;
    private float destroyX;
    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        cam = Camera.main;
        destroyX = cam.transform.position.x - cam.orthographicSize * cam.aspect - 10f;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        // Сообщаем GameManager что новый дом появился
        if (GameManager.Instance != null)
            GameManager.Instance.RegisterHouse(this);
    }

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        if (transform.position.x < destroyX)
        {
            if (GameManager.Instance != null)
                GameManager.Instance.UnregisterHouse(this);
            Destroy(gameObject);
        }
    }

    public void SetAsTarget(Color color)
    {
        IsTarget = true;
        if (sr != null)
            sr.color = color;
    }

    public void ClearTarget()
    {
        IsTarget = false;
        if (sr != null)
            sr.color = originalColor;
    }
}
