using UnityEngine;
using InputSystemKeyboard = UnityEngine.InputSystem.Keyboard;
using InputSystemTouchscreen = UnityEngine.InputSystem.Touchscreen;
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
    public GameObject boostFlame;

    [Header("Gift")]
    public GameObject giftPrefab;
    public float giftGravity = 12f;
    public float giftDriftX = -1.5f;

    [Header("Touch")]
    public float doubleTapInterval = 0.3f;
    public float holdBoostThreshold = 0.4f;

    struct GiftData { public GameObject obj; public float velocityX; public float velocityY; public SpriteRenderer sr; }

    List<GiftData> gifts = new List<GiftData>();
    float camBottom, camLeft, stamina;
    bool isBoosting, staminaEmpty;

    // touch
    float touchVertical = 0f;
    bool touchBoostWanted = false;
    bool touchGiftWanted = false;
    float lastTapTime = -99f;
    float holdTimer = 0f;
    int activeId = -1;
    Vector2 touchOrigin;

    void Start()
    {
        Camera cam = Camera.main;
        camBottom = cam.transform.position.y - cam.orthographicSize - 3f;
        camLeft = cam.transform.position.x - cam.orthographicSize * cam.aspect - 3f;
        stamina = maxStamina;
    }

void ReadTouches() { touchVertical = 0f; touchBoostWanted = false; touchGiftWanted = false; var ts = InputSystemTouchscreen.current; if (ts == null) return; float halfW = Screen.width * 0.5f; var touches = ts.touches; bool giftFired = false; for (int ti = 0; ti < touches.Count; ti++) { var t = touches[ti]; var phase = t.phase.ReadValue(); var pos = t.position.ReadValue(); bool isLeft = pos.x <= halfW; bool isRight = pos.x > halfW; if (phase == UnityEngine.InputSystem.TouchPhase.Began) { if (!giftFired) { touchGiftWanted = true; giftFired = true; } if (isLeft && activeId == -1) { activeId = t.touchId.ReadValue(); touchOrigin = pos; holdTimer = 0f; } if (isRight) holdTimer = 0f; } if (t.touchId.ReadValue() == activeId && isLeft) { if (phase == UnityEngine.InputSystem.TouchPhase.Moved || phase == UnityEngine.InputSystem.TouchPhase.Stationary) { float delta = pos.y - touchOrigin.y; if (Mathf.Abs(delta) >= 20f) touchVertical = delta > 0f ? 1f : -1f; } if (phase == UnityEngine.InputSystem.TouchPhase.Ended || phase == UnityEngine.InputSystem.TouchPhase.Canceled) { activeId = -1; holdTimer = 0f; } } if (isRight) { if (phase == UnityEngine.InputSystem.TouchPhase.Moved || phase == UnityEngine.InputSystem.TouchPhase.Stationary) { holdTimer += Time.deltaTime; if (holdTimer >= holdBoostThreshold) touchBoostWanted = true; } if (phase == UnityEngine.InputSystem.TouchPhase.Ended || phase == UnityEngine.InputSystem.TouchPhase.Canceled) holdTimer = 0f; } } bool anyActive = false; foreach (var t in touches) if (t.isInProgress) { anyActive = true; break; } if (!anyActive) { activeId = -1; holdTimer = 0f; } }

    void HandleStamina(bool wantBoost)
    {
        if (stamina <= 0f) staminaEmpty = true;
        if (staminaEmpty && stamina >= maxStamina * 0.3f) staminaEmpty = false;

        bool can = wantBoost && stamina > 0f && !staminaEmpty;
        isBoosting = can;
        WorldSpeed.SetBoost(can);
        if (boostFlame != null) boostFlame.SetActive(can);

        if (can) { stamina -= staminaDrainRate * Time.deltaTime; if (stamina < 0f) { stamina = 0f; staminaEmpty = true; } }
        else { stamina += staminaRegenRate * Time.deltaTime; if (stamina > maxStamina) stamina = maxStamina; }

        if (staminaBar != null)
        {
            float r = stamina / maxStamina;
            staminaBar.localScale = new Vector3(r, 1f, 1f);
            Image img = staminaBar.GetComponent<Image>();
            if (img != null) img.color = r > 0.3f ? Color.Lerp(new Color(1f,.6f,0f), new Color(0f,.9f,1f), r) : new Color(1f,.2f,.2f);
        }
    }

    void Update()
    {
        ReadTouches();

        float keyV = 0f;
        bool shiftDown = false;
        bool spaceDown = false;
        var kb = InputSystemKeyboard.current;
        if (kb != null)
        {
            shiftDown = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) keyV = 1f;
            else if (kb.sKey.isPressed || kb.downArrowKey.isPressed) keyV = -1f;
            spaceDown = kb.spaceKey.wasPressedThisFrame;
        }

        bool wantBoost = shiftDown || touchBoostWanted;
        HandleStamina(wantBoost);

        float vertical = keyV != 0f ? keyV : touchVertical;
        float spd = isBoosting ? boostSpeed : moveSpeed;
        Vector3 p = transform.position;
        p.y += vertical * spd * Time.deltaTime;
        p.y = Mathf.Clamp(p.y, minY, maxY);
        transform.position = p;

        if ((spaceDown || touchGiftWanted) && giftPrefab != null)
            DoSpawnGift(giftDriftX, 0f);

        for (int i = gifts.Count - 1; i >= 0; i--)
        {
            GiftData g = gifts[i];
            if (g.obj == null) { gifts.RemoveAt(i); continue; }
            g.velocityY -= giftGravity * Time.deltaTime;
            Vector3 gp = g.obj.transform.position;
            gp.x += g.velocityX * Time.deltaTime;
            gp.y += g.velocityY * Time.deltaTime;
            g.obj.transform.position = gp;
            gifts[i] = g;

            if (GameManager.Instance != null)
            {
                HouseMover tgt = GameManager.Instance.TargetHouse;
                if (tgt != null && tgt.IsTarget)
                {
                    if (tgt.GetComponent<SpriteRenderer>().bounds.Contains(new Vector3(gp.x, gp.y, 0f)))
                    {
                        tgt.ClearTarget();
                        GameManager.Instance.OnGiftDelivered();
                        Destroy(g.obj);
                        gifts.RemoveAt(i);
                        continue;
                    }
                }
            }

            if (gp.y < camBottom || gp.x < camLeft) { Destroy(g.obj); gifts.RemoveAt(i); }
        }
    }

    void DoSpawnGift(float vx, float vy)
    {
        if (giftPrefab == null) return;
        GameObject ng = Instantiate(giftPrefab, transform.position, Quaternion.identity);
        SpriteRenderer gsr = ng.GetComponent<SpriteRenderer>();
        if (GameManager.Instance != null) { GlowEffect gl = ng.AddComponent<GlowEffect>(); gl.SetGlow(GameManager.Instance.CurrentTargetColor); }
        gifts.Add(new GiftData { obj = ng, velocityX = vx, velocityY = vy, sr = gsr });
    }

    public void OnHitByEnemy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnEnemyHit();
        float[] angles = { -120f, -90f, -60f };
        for (int k = 0; k < 3; k++) { float rad = angles[k] * Mathf.Deg2Rad; DoSpawnGift(Mathf.Cos(rad)*3.5f+giftDriftX, Mathf.Sin(rad)*3.5f); }
    }
}
