using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class SantaController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float boostSpeed = 7f;
    public float minY = 1f;
    public float maxY = 6f;

    [Header("Stamina")]
    public float maxStamina = 3f;
    public float staminaDrainRate = 1f;
    public float staminaRegenRate = 0.5f;
    public RectTransform staminaBar;

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
    private float stamina;
    private bool isBoosting = false;
    private bool staminaEmpty = false;

void Start() { Camera cam = Camera.main; camBottom = cam.transform.position.y - cam.orthographicSize - 3f; camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect - 3f; stamina = maxStamina; }

void Update() { var keyboard = Keyboard.current; if (keyboard == null) return; bool shiftHeld = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed; if (stamina <= 0f) staminaEmpty = true; if (staminaEmpty && stamina >= maxStamina * 0.3f) staminaEmpty = false; bool canBoost = shiftHeld && stamina > 0f && !staminaEmpty; if (canBoost) { isBoosting = true; WorldSpeed.SetBoost(true); stamina -= staminaDrainRate * Time.deltaTime; if (stamina < 0f) { stamina = 0f; staminaEmpty = true; } } else { isBoosting = false; WorldSpeed.SetBoost(false); stamina += staminaRegenRate * Time.deltaTime; if (stamina > maxStamina) stamina = maxStamina; } if (staminaBar != null) { float ratio = stamina / maxStamina; staminaBar.localScale = new Vector3(ratio, 1f, 1f); Image img = staminaBar.GetComponent<Image>(); if (img != null) img.color = ratio > 0.3f ? Color.Lerp(new Color(1f, 0.6f, 0f), new Color(0f, 0.9f, 1f), ratio) : new Color(1f, 0.2f, 0.2f); } float input = 0f; if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input = 1f; else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input = -1f; float currentSpeed = isBoosting ? boostSpeed : moveSpeed; Vector3 pos = transform.position; pos.y += input * currentSpeed * Time.deltaTime; pos.y = Mathf.Clamp(pos.y, minY, maxY); transform.position = pos; if (keyboard.spaceKey.wasPressedThisFrame && giftPrefab != null) SpawnGift(giftDriftX, 0f); for (int i = activeGifts.Count - 1; i >= 0; i--) { GiftData g = activeGifts[i]; if (g.obj == null) { activeGifts.RemoveAt(i); continue; } g.velocityY -= giftGravity * Time.deltaTime; Vector3 gpos = g.obj.transform.position; gpos.x += g.velocityX * Time.deltaTime; gpos.y += g.velocityY * Time.deltaTime; g.obj.transform.position = gpos; activeGifts[i] = g; if (GameManager.Instance != null) { HouseMover target = GameManager.Instance.TargetHouse; if (target != null && target.IsTarget) { Bounds houseBounds = target.GetComponent<SpriteRenderer>().bounds; if (houseBounds.Contains(new Vector3(gpos.x, gpos.y, 0f))) { target.ClearTarget(); GameManager.Instance.OnGiftDelivered(); Destroy(g.obj); activeGifts.RemoveAt(i); continue; } } } if (gpos.y < camBottom || gpos.x < camLeft) { Destroy(g.obj); activeGifts.RemoveAt(i); } } }

    void SpawnGift(float vx, float vy)
    {
        if (giftPrefab == null) return;
        GameObject ng = Instantiate(giftPrefab, transform.position, Quaternion.identity);
        SpriteRenderer gsr = ng.GetComponent<SpriteRenderer>();
        if (GameManager.Instance != null)
        {
            GlowEffect glow = ng.AddComponent<GlowEffect>();
            glow.SetGlow(GameManager.Instance.CurrentTargetColor);
        }
        activeGifts.Add(new GiftData { obj = ng, velocityX = vx, velocityY = vy, sr = gsr });
    }

    public void OnHitByEnemy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnEnemyHit();

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
