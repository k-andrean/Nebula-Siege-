using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace KelvinAndrean.NebulaSiege
{
    public class DifficultySelectionManager : MonoBehaviour
    {
        [SerializeField] private Button easyButton;
        [SerializeField] private Button hardButton;
        [SerializeField] private string levelSelectionSceneName = "LevelSelection";

        private void Start()
        {
            easyButton.onClick.AddListener(() => SelectDifficulty(GameSettings.Difficulty.Easy));
            hardButton.onClick.AddListener(() => SelectDifficulty(GameSettings.Difficulty.Hard));
        }

        private void SelectDifficulty(GameSettings.Difficulty difficulty)
        {
            GameSettings.Instance.SetDifficulty(difficulty);
            SceneManager.LoadScene(levelSelectionSceneName);
        }
    }
} 