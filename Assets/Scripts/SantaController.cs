using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class SantaController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float minY = 1f;
    public float maxY = 6f;

    [Header("Gift")]
    public GameObject giftPrefab;
    public float giftGravity = 12f;
    public float giftDriftX = -1.5f;

    private struct GiftData
    {
        public GameObject obj;
        public float velocityX;
        public float velocityY;
        public SpriteRenderer sr;
    }

    private List<GiftData> activeGifts = new List<GiftData>();
    private float camBottom;
    private float camLeft;

    void Start()
    {
        Camera cam = Camera.main;
        camBottom = cam.transform.position.y - cam.orthographicSize - 3f;
        camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect - 3f;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float input = 0f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            input = 1f;
        else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            input = -1f;

        Vector3 pos = transform.position;
        pos.y += input * moveSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

        if (keyboard.spaceKey.wasPressedThisFrame && giftPrefab != null)
        {
            SpawnGift(giftDriftX, 0f);
        }

        for (int i = activeGifts.Count - 1; i >= 0; i--)
        {
            GiftData g = activeGifts[i];
            if (g.obj == null) { activeGifts.RemoveAt(i); continue; }

            g.velocityY -= giftGravity * Time.deltaTime;
            Vector3 gpos = g.obj.transform.position;
            gpos.x += g.velocityX * Time.deltaTime;
            gpos.y += g.velocityY * Time.deltaTime;
            g.obj.transform.position = gpos;
            activeGifts[i] = g;

            if (GameManager.Instance != null)
            {
                HouseMover target = GameManager.Instance.TargetHouse;
                if (target != null && target.IsTarget)
                {
                    Bounds houseBounds = target.GetComponent<SpriteRenderer>().bounds;
                    if (houseBounds.Contains(new Vector3(gpos.x, gpos.y, 0f)))
                    {
                        target.ClearTarget();
                        GameManager.Instance.OnGiftDelivered();
                        Destroy(g.obj);
                        activeGifts.RemoveAt(i);
                        continue;
                    }
                }
            }

            if (gpos.y < camBottom || gpos.x < camLeft)
            {
                Destroy(g.obj);
                activeGifts.RemoveAt(i);
            }
        }
    }

    void SpawnGift(float vx, float vy)
    {
        if (giftPrefab == null) return;
        GameObject ng = Instantiate(giftPrefab, transform.position, Quaternion.identity);
        SpriteRenderer gsr = ng.GetComponent<SpriteRenderer>();
        if (gsr != null && GameManager.Instance != null)
            gsr.color = GameManager.Instance.CurrentTargetColor;
        activeGifts.Add(new GiftData { obj = ng, velocityX = vx, velocityY = vy, sr = gsr });
    }

    public void OnHitByEnemy()
    {
        float[] angles = { -120f, -90f, -60f };
        for (int k = 0; k < 3; k++)
        {
            float rad = angles[k] * Mathf.Deg2Rad;
            float vx = Mathf.Cos(rad) * 3.5f + giftDriftX;
            float vy = Mathf.Sin(rad) * 3.5f;
            SpawnGift(vx, vy);
        }
    }
}
