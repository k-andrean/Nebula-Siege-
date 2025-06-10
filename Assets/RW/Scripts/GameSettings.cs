using UnityEngine;

namespace KelvinAndrean.NebulaSiege
{
    public class GameSettings : MonoBehaviour
    {
        public static GameSettings Instance { get; private set; }

        public enum Difficulty
        {
            Easy,
            Hard
        }

        public enum Level
        {
            Level1,
            Level2,
            Level3
        }

        public Difficulty CurrentDifficulty { get; private set; }
        public Level CurrentLevel { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void SetDifficulty(Difficulty difficulty)
        {
            CurrentDifficulty = difficulty;
        }

        public void SetLevel(Level level)
        {
            CurrentLevel = level;
        }

        public float GetDifficultyMultiplier()
        {
            return CurrentDifficulty == Difficulty.Easy ? 1f : 1.5f;
        }

        public int GetLevelNumber()
        {
            return (int)CurrentLevel + 1;
        }

        public int GetTorchkaCount()
        {
            return CurrentDifficulty == Difficulty.Easy ? 4 : 2;
        }

        public int GetRowCount()
        {
            switch (CurrentLevel)
            {
                case Level.Level1:
                    return 1;
                case Level.Level2:
                    return 2;
                case Level.Level3:
                    return 3;
                default:
                    return 1;
            }
        }
    }
} 