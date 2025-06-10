using UnityEngine;

namespace KelvinAndrean.NebulaSiege
{
    public class OtherColorInvader : MonoBehaviour
    {
        public enum PowerUpType
        {
            BulletSpeed,    // Yellow
            Invincibility,  // Red
            MoveSpeed      // Green
        }

        [SerializeField] public PowerUpType powerUpType = PowerUpType.BulletSpeed;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // Apply color based on power-up type
            if (spriteRenderer != null)
            {
                switch (powerUpType)
                {
                    case PowerUpType.BulletSpeed:
                        spriteRenderer.color = Color.yellow;
                        break;
                    case PowerUpType.Invincibility:
                        spriteRenderer.color = Color.red;
                        break;
                    case PowerUpType.MoveSpeed:
                        spriteRenderer.color = Color.green;
                        break;
                }
            }
        }
    }
} 