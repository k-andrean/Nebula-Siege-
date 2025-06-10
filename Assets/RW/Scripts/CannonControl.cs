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

        private float shootTimer;
        private float originalCoolDownTime;
        private float originalBulletSpeed;

        [SerializeField]
        private float respawnTime = 2f;

        [SerializeField]
        private SpriteRenderer sprite;

        [SerializeField]
        private Collider2D cannonCollider;

        private Vector2 startPos;

        private void Start()
        {
            startPos = transform.position;
            originalCoolDownTime = coolDownTime;
            originalBulletSpeed = Bullet.DefaultSpeed;
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

        public void ApplyPowerUp()
        {
            StartCoroutine(PowerUpRoutine());
        }

        private IEnumerator PowerUpRoutine()
        {
            // Apply power-ups
            coolDownTime = originalCoolDownTime * cooldownMultiplier;
            Bullet.DefaultSpeed = originalBulletSpeed * bulletSpeedMultiplier;

            // Wait for duration
            yield return new WaitForSeconds(powerUpDuration);

            // Revert changes
            coolDownTime = originalCoolDownTime;
            Bullet.DefaultSpeed = originalBulletSpeed;
        }

        private void ResetPowerUp()
        {
            // Stop any ongoing power-up coroutines
            StopAllCoroutines();
            
            // Reset to original values
            coolDownTime = originalCoolDownTime;
            Bullet.DefaultSpeed = originalBulletSpeed;
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