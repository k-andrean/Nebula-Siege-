using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace KelvinAndrean.NebulaSiege
{
    public class LevelSelectionManager : MonoBehaviour
    {
        [SerializeField] private Button level1Button;
        [SerializeField] private Button level2Button;
        [SerializeField] private Button level3Button;
        [SerializeField] private string mainGameSceneName = "Main";

        private void Start()
        {
            level1Button.onClick.AddListener(() => SelectLevel(GameSettings.Level.Level1));
            level2Button.onClick.AddListener(() => SelectLevel(GameSettings.Level.Level2));
            level3Button.onClick.AddListener(() => SelectLevel(GameSettings.Level.Level3));
        }

        private void SelectLevel(GameSettings.Level level)
        {
            GameSettings.Instance.SetLevel(level);
            SceneManager.LoadScene(mainGameSceneName);
        }
    }
} 