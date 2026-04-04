using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LivesDisplay : MonoBehaviour
{
    public Sprite heartSprite;
    public int maxLives = 3;
    public float heartSize = 40f;
    public float heartSpacing = 45f;
    public GameObject gameOverPanel;
    public Button restartButton;

    Image[] hearts;
    static readonly Color FULL = new Color(1f, 1f, 1f, 1f);
    static readonly Color EMPTY = new Color(1f, 1f, 1f, 0.2f);

    void OnValidate() { if (gameOverPanel != null) gameOverPanel.SetActive(false); }

    
void Awake()
    {
        // Скрыть панель Game Over при старте
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    void Start()
    {
        // Автоподключение кнопки
        if (restartButton != null)
            restartButton.onClick.AddListener(Restart);

        // Создаём сердечки
        hearts = new Image[maxLives];
        Vector2 anchor = new Vector2(0f, 1f);
        for (int i = 0; i < maxLives; i++)
        {
            GameObject go = new GameObject("Heart_" + i);
            go.transform.SetParent(transform, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = anchor;
            rt.anchoredPosition = new Vector2(i * heartSpacing, 0f);
            rt.sizeDelta = new Vector2(heartSize, heartSize);
            Image img = go.AddComponent<Image>();
            img.sprite = heartSprite;
            img.preserveAspect = true;
            img.color = FULL;
            hearts[i] = img;
        }
    }

    public void UpdateHearts(int current)
    {
        if (hearts == null) return;
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].color = i < current ? FULL : EMPTY;
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public static void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
