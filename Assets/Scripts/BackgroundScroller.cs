using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;

    SpriteRenderer sr;
    GameObject mirror;
    float w;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        w = sr.sprite.bounds.size.x * Mathf.Abs(transform.localScale.x);

        Camera cam = Camera.main;
        float camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // Ставим оригинал так чтобы его левый край совпадал с левым краем камеры
        transform.position = new Vector3(camLeft + w * 0.5f, transform.position.y, transform.position.z);

        // Зеркало вплотную справа
        mirror = new GameObject("BG_Mirror");
        mirror.transform.position = new Vector3(transform.position.x + w, transform.position.y, transform.position.z);
        mirror.transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        SpriteRenderer msr = mirror.AddComponent<SpriteRenderer>();
        msr.sprite = sr.sprite;
        msr.sortingLayerID = sr.sortingLayerID;
        msr.sortingOrder = sr.sortingOrder;
        msr.color = sr.color;
    }

    void Update()
    {
        float step = scrollSpeed * Time.deltaTime;
        transform.position += Vector3.left * step;
        mirror.transform.position += Vector3.left * step;

        Camera cam = Camera.main;
        float camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // Если левый край оригинала ушёл левее camLeft — телепортируем его вправо за зеркало
        if (transform.position.x + w * 0.5f < camLeft)
            transform.position = new Vector3(mirror.transform.position.x + w, transform.position.y, transform.position.z);

        // Если левый край зеркала ушёл левее camLeft — телепортируем его вправо за оригинал
        if (mirror.transform.position.x + w * 0.5f < camLeft)
            mirror.transform.position = new Vector3(transform.position.x + w, mirror.transform.position.y, mirror.transform.position.z);
    }
}
