using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        HandleSceneTransition(nextSceneIndex);
    }

    public void LoadPreviousScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int previousSceneIndex = (currentSceneIndex - 1 + sceneCount) % sceneCount;
        HandleSceneTransition(previousSceneIndex);
    }

    private void HandleSceneTransition(int targetSceneIndex)
    {
        string targetSceneName = GetSceneNameByBuildIndex(targetSceneIndex);

        if (targetSceneName == "Main")
        {
            DestroyAndRecreateAudioManager();
        }

        SceneManager.LoadScene(targetSceneIndex);
    }

    private void DestroyAndRecreateAudioManager()
    {
        // Находим существующий AudioManager
        AudioManager existingManager = FindObjectOfType<AudioManager>();

        if (existingManager != null)
        {
            // Сохраняем текущие настройки аудио
            float musicVol = existingManager.musicVolume;
            float sfxVol = existingManager.sfxVolume;
            bool musicOn = existingManager.musicEnabled;
            bool sfxOn = existingManager.sfxEnabled;

            // Уничтожаем старый AudioManager
            Destroy(existingManager.gameObject);

            // Создаем новый AudioManager
            GameObject newAudioManager = new GameObject("AudioManager");
            AudioManager newManager = newAudioManager.AddComponent<AudioManager>();

            // Восстанавливаем настройки
            newManager.musicVolume = musicVol;
            newManager.sfxVolume = sfxVol;
            newManager.musicEnabled = musicOn;
            newManager.sfxEnabled = sfxOn;

            // Инициализируем
            newManager.InitializeAudio();
        }
    }

    private string GetSceneNameByBuildIndex(int buildIndex)
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
        return sceneName;
    }
}