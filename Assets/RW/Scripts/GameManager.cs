﻿/*
 * Copyright (c) 2021 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
using System;
using System.Runtime.Versioning;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
//using static System.Net.Mime.MediaTypeNames;

//namespace RayWenderlich.SpaceInvadersUnity
namespace KelvinAndrean.NebulaSiege
{
    public class GameManager : MonoBehaviour
    {
        internal static GameManager Instance;

        [SerializeField]
        private AudioSource sfx;

        //[SerializeField]
        //private GameObject explosionPrefab;

        [SerializeField]
        private float explosionTime = 1f;

        [SerializeField]
        private AudioClip explosionClip;

        [SerializeField]
        private int maxLives = 3;

        [SerializeField]
        private UnityEngine.UI.Text livesLabel;

        private int lives;

        [SerializeField]
        private MusicControl music;

        [SerializeField]
        private UnityEngine.UI.Text scoreLabel;

        [SerializeField]
        private UnityEngine.UI.Text powerUpLabel;

        [SerializeField]
        private GameObject gameOver;

        [SerializeField]
        private GameObject allClear;

        [SerializeField]
        private Button restartButton;

        private int score;

        internal void UpdateScore(int value)
        {
            score += value;
            scoreLabel.text = $"Score: {score}";
        }

        internal void UpdatePowerUp(string text, Color? color = null)
        {
            if (powerUpLabel != null)
            {
                powerUpLabel.text = text;
                if (color.HasValue)
                {
                    powerUpLabel.color = color.Value;
                }
            }
        }

        internal void TriggerGameOver(bool failure = true)
        {
            gameOver.SetActive(failure);
            allClear.SetActive(!failure);
            restartButton.gameObject.SetActive(true);

            Time.timeScale = 0f;
            music.StopPlaying();
        }


        internal void UpdateLives()
        {
            lives = Mathf.Clamp(lives - 1, 0, maxLives);
            livesLabel.text = $"Lives: {lives}";

            if (lives > 0)
            {
                return;
            }

            TriggerGameOver();

        }


        internal void CreateExplosion(Vector2 position)
        {
            PlaySfx(explosionClip);

            var explosion = Instantiate(Resources.Load("Prefabs/Explosion"), position,
                Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-180f, 180f)));
            Destroy(explosion, explosionTime);
        }


        internal void PlaySfx(AudioClip clip) => sfx.PlayOneShot(clip);

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            ApplyGameSettings();

            lives = maxLives;
            livesLabel.text = $"Lives: {lives}";

            score = 0;
            scoreLabel.text = $"Score: {score}";
            powerUpLabel.text = $"";
            gameOver.gameObject.SetActive(false);
            allClear.gameObject.SetActive(false);

            restartButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                Time.timeScale = 1f;
            });
            restartButton.gameObject.SetActive(false);


        }

      
        private void ApplyGameSettings()
        {
            if (GameSettings.Instance == null) return;

            // Apply difficulty multiplier to game parameters
            float difficultyMultiplier = GameSettings.Instance.GetDifficultyMultiplier();
            int levelNumber = GameSettings.Instance.GetLevelNumber();

            // Adjust game parameters based on difficulty and level
            // For example:
            // - Increase invader speed based on difficulty
            // - Increase number of invaders based on level
            // - Adjust power-up duration based on difficulty
            // - etc.
        }

    }
}