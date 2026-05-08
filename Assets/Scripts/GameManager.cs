using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button restartButton;

    private int score = 0;
    private bool isGameOver = false;

    private void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
    }

    public void AddScore(int points)
    {
        if (isGameOver) return;

        score += points;
        scoreText.text = $"Счет: {score}";
    }

    public void GameOver()
    {
        if (isGameOver) return; // Защита от повторного вызова

        isGameOver = true;
        finalScoreText.text = $"Счет: {score}";
        gameOverPanel.SetActive(true);

        var snakeMovements = FindObjectsOfType<SnakeKeyboardInputHandler>();
        foreach (var movement in snakeMovements)
        {
            movement.enabled = false;
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // Останавливаем время
        AudioListener.pause = true; // Отключаем звуки (опционально)
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // Возобновляем время
        AudioListener.pause = false; // Включаем звуки обратно
    }

    private void RestartGame()
    {
        ResumeGame(); // Снимаем паузу перед перезагрузкой
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}