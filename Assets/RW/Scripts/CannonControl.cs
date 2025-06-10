/*
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
using System.Runtime.Versioning;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

//namespace RayWenderlich.SpaceInvadersUnity
namespace KelvinAndrean.NebulaSiege
{
    public class CannonControl : MonoBehaviour
    {
        [SerializeField]
        private float speed = 500f;

        [SerializeField]
        private Transform muzzle;

        [SerializeField]
        private AudioClip shooting;

        [SerializeField]
        private float coolDownTime = 0.5f;

        public float CoolDownTime
        {
            get => coolDownTime;
            set => coolDownTime = value;
        }

        [Header("Power-up Settings")]
        [SerializeField] private float powerUpDuration = 30f;
        [SerializeField] private float bulletSpeedMultiplier = 3f;
        [SerializeField] private float cooldownMultiplier = 0.2f;
        [SerializeField] private float moveSpeedMultiplier = 1.2f;

        [Header("Power-up UI")]
        [SerializeField] private TextMeshProUGUI powerUpText;
        [SerializeField] private Color bulletSpeedColor = Color.yellow;
        [SerializeField] private Color invincibilityColor = Color.red;
        [SerializeField] private Color moveSpeedColor = Color.green;

        private float shootTimer;
        private float originalCoolDownTime;
        private float originalBulletSpeed;
        private float originalSpeed;

        [SerializeField]
        private float respawnTime = 2f;

        [SerializeField]
        private SpriteRenderer sprite;

        [SerializeField]
        private Collider2D cannonCollider;

        private Vector2 startPos;

        private Coroutine currentPowerUpCoroutine;
        private OtherColorInvader.PowerUpType? currentPowerUpType;

        private void Start()
        {
            startPos = transform.position;
            originalCoolDownTime = coolDownTime;
            originalBulletSpeed = Bullet.DefaultSpeed;
            originalSpeed = speed;
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(speed * Time.deltaTime, 0, 0);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(-speed * Time.deltaTime, 0, 0);
            }

            shootTimer += Time.deltaTime;
            if (shootTimer > coolDownTime && Input.GetKey(KeyCode.Space))
            {
                shootTimer = 0f;

                var bulletObj = Instantiate(Resources.Load("Prefabs/Bullet"), muzzle.position, Quaternion.identity) as GameObject;
                var bullet = bulletObj.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.SetSourceCannon(this);
                }
                GameManager.Instance.PlaySfx(shooting);
            }
        }

        // private void UpdatePowerUpText(string text, Color? color = null)
        // {
        //     if (powerUpText != null)
        //     {
        //         powerUpText.text = text;
        //         if (color.HasValue)
        //         {
        //             powerUpText.color = color.Value;
        //         }
        //     }
        // }

        public void ApplyPowerUp(OtherColorInvader.PowerUpType powerUpType)
        {
            // If there's an active power-up, stop it first
            if (currentPowerUpCoroutine != null)
            {
                StopCoroutine(currentPowerUpCoroutine);
                RevertCurrentPowerUp();
            }

            currentPowerUpType = powerUpType;
            switch (powerUpType)
            {
                case OtherColorInvader.PowerUpType.BulletSpeed:
                    currentPowerUpCoroutine = StartCoroutine(ApplyBulletSpeedPowerUp());
                    GameManager.Instance.UpdatePowerUp("Bullet Speed", bulletSpeedColor);
                    break;
                case OtherColorInvader.PowerUpType.Invincibility:
                    currentPowerUpCoroutine = StartCoroutine(ApplyInvincibilityPowerUp());
                    GameManager.Instance.UpdatePowerUp("Invincibility", invincibilityColor);
                    break;
                case OtherColorInvader.PowerUpType.MoveSpeed:
                    currentPowerUpCoroutine = StartCoroutine(ApplyMoveSpeedPowerUp());
                    GameManager.Instance.UpdatePowerUp("Speed Boost", moveSpeedColor);
                    break;
            }
        }

        private void RevertCurrentPowerUp()
        {
            if (!currentPowerUpType.HasValue) return;

            switch (currentPowerUpType.Value)
            {
                case OtherColorInvader.PowerUpType.BulletSpeed:
                    coolDownTime = originalCoolDownTime;
                    Bullet.DefaultSpeed = originalBulletSpeed;
                    break;
                case OtherColorInvader.PowerUpType.Invincibility:
                    cannonCollider.enabled = true;
                    ChangeSpriteAlpha(1.0f);
                    break;
                case OtherColorInvader.PowerUpType.MoveSpeed:
                    speed = originalSpeed;
                    break;
            }
            currentPowerUpType = null;
            GameManager.Instance.UpdatePowerUp("", null);
        }

        private IEnumerator ApplyBulletSpeedPowerUp()
        {
            // Apply power-ups
            coolDownTime = originalCoolDownTime * cooldownMultiplier;
            Bullet.DefaultSpeed = originalBulletSpeed * bulletSpeedMultiplier;

            // Wait for duration
            yield return new WaitForSeconds(powerUpDuration);

            // Revert changes
            RevertCurrentPowerUp();
            currentPowerUpCoroutine = null;
        }

        private IEnumerator ApplyInvincibilityPowerUp()
        {
            // Store original state
            bool originalColliderState = cannonCollider.enabled;
            float originalAlpha = sprite.color.a;

            // Apply power-up
            cannonCollider.enabled = false;
            ChangeSpriteAlpha(0.5f);

            // Wait for duration
            yield return new WaitForSeconds(powerUpDuration);

            // Revert changes
            RevertCurrentPowerUp();
            currentPowerUpCoroutine = null;
        }

        private IEnumerator ApplyMoveSpeedPowerUp()
        {
            // Apply power-up
            speed = originalSpeed * moveSpeedMultiplier;

            // Wait for duration
            yield return new WaitForSeconds(powerUpDuration);

            // Revert changes
            RevertCurrentPowerUp();
            currentPowerUpCoroutine = null;
        }

        private void ResetPowerUp()
        {
            // Stop any ongoing power-up coroutines
            if (currentPowerUpCoroutine != null)
            {
                StopCoroutine(currentPowerUpCoroutine);
                currentPowerUpCoroutine = null;
            }
            
            // Reset to original values
            RevertCurrentPowerUp();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            GameManager.Instance.UpdateLives();
            ResetPowerUp(); // Reset power-ups before respawning
            StopAllCoroutines();
            StartCoroutine(Respawn());
        }

        System.Collections.IEnumerator Respawn()
        {
            enabled = false;
            cannonCollider.enabled = false;
            ChangeSpriteAlpha(0.0f);

            yield return new WaitForSeconds(0.25f * respawnTime);

            transform.position = startPos;
            enabled = true;
            ChangeSpriteAlpha(0.25f);

            yield return new WaitForSeconds(0.75f * respawnTime);

            ChangeSpriteAlpha(1.0f);
            cannonCollider.enabled = true;
        }

        private void ChangeSpriteAlpha(float value)
        {
            var color = sprite.color;
            color.a = value;
            sprite.color = color;
        }
    }
}