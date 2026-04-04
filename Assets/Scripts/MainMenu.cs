using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);
    }

void StartGame() { UnityEngine.SceneManagement.SceneManager.LoadScene(1); }

    void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
