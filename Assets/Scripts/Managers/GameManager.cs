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

    private int _score;
    private bool _isGameOver;
    private SnakeKeyboardInputHandler[] _cachedSnakeInputs;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        gameOverPanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
        _cachedSnakeInputs = FindObjectsOfType<SnakeKeyboardInputHandler>();
    }

    private void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);

        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayBackgroundMusic();
    }

    public void AddScore(int points)
    {
        if (_isGameOver) return;

        int mult = 1;
        if (BoostManager.Instance != null)
            mult = BoostManager.Instance.ScoreMultiplier;

        _score += points * mult;
        if (scoreText != null)
            scoreText.SetText("Счет: {0}", _score);
    }

    public void GameOver()
    {
        if (_isGameOver) return;

        _isGameOver = true;
        if (finalScoreText != null)
            finalScoreText.SetText("Счет: {0}", _score);

        gameOverPanel.SetActive(true);

        for (int i = 0; i < _cachedSnakeInputs.Length; i++)
        {
            if (_cachedSnakeInputs[i] != null)
                _cachedSnakeInputs[i].enabled = false;
        }
    }

    private static void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void RestartGame()
    {
        ResumeGame();
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
}
