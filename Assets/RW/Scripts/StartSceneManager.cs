using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace KelvinAndrean.NebulaSiege
{
    public class StartSceneManager : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private string difficultySelectionSceneName = "DifficultySelection";

        private void Start()
        {
            // Add click listeners to buttons
            startButton.onClick.AddListener(StartGame);
            quitButton.onClick.AddListener(QuitGame);

            // Make sure GameSettings exists
            if (GameSettings.Instance == null)
            {
                var gameSettingsObj = new GameObject("GameSettings");
                gameSettingsObj.AddComponent<GameSettings>();
            }
        }

        private void StartGame()
        {
            SceneManager.LoadScene(difficultySelectionSceneName);
        }

        private void QuitGame()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
} 