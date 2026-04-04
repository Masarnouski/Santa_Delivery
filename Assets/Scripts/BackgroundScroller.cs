using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;

    SpriteRenderer sr;
    GameObject[] tiles;
    float w;
    int count = 3;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        w = sr.sprite.bounds.size.x * Mathf.Abs(transform.localScale.x);

        Camera cam = Camera.main;
        float camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // Ставим оригинал у левого края камеры
        transform.position = new Vector3(camLeft + w * 0.5f, transform.position.y, transform.position.z);

        tiles = new GameObject[count];
        tiles[0] = gameObject;

        // Второй тайл — зеркальный
        tiles[1] = MakeTile(transform.position.x + w, true);
        // Третий тайл — оригинал (буфер справа чтобы не было дырки)
        tiles[2] = MakeTile(transform.position.x + w * 2f, false);
    }

    GameObject MakeTile(float x, bool flip)
    {
        GameObject go = new GameObject("BG_Tile");
        go.transform.position = new Vector3(x, transform.position.y, transform.position.z);
        go.transform.localScale = new Vector3(flip ? -transform.localScale.x : transform.localScale.x, transform.localScale.y, transform.localScale.z);
        SpriteRenderer r = go.AddComponent<SpriteRenderer>();
        r.sprite = sr.sprite;
        r.sortingLayerID = sr.sortingLayerID;
        r.sortingOrder = sr.sortingOrder;
        r.color = sr.color;
        return go;
    }

    void Update()
    {
        float step = scrollSpeed * WorldSpeed.Multiplier * Time.deltaTime;

        for (int i = 0; i < count; i++)
            tiles[i].transform.position += Vector3.left * step;

        Camera cam = Camera.main;
        float camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        // Находим самый правый тайл
        for (int i = 0; i < count; i++)
        {
            if (tiles[i].transform.position.x + w * 0.5f < camLeft)
            {
                // Найдём самый правый тайл
                float maxX = tiles[0].transform.position.x;
                for (int j = 1; j < count; j++)
                    if (tiles[j].transform.position.x > maxX) maxX = tiles[j].transform.position.x;

                tiles[i].transform.position = new Vector3(maxX + w, tiles[i].transform.position.y, tiles[i].transform.position.z);
            }
        }
    }
}
