using UnityEngine;

namespace KelvinAndrean.NebulaSiege
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private Sprite backgroundSprite;
        private SpriteRenderer spriteRenderer;
        private Camera mainCamera;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            if (backgroundSprite != null)
            {
                spriteRenderer.sprite = backgroundSprite;
                ScaleBackground();
            }
        }

        private void ScaleBackground()
        {
            if (spriteRenderer == null || mainCamera == null) return;

            // Get the camera's viewport dimensions
            float worldScreenHeight = mainCamera.orthographicSize * 2.0f;
            float worldScreenWidth = worldScreenHeight * mainCamera.aspect;

            // Get the sprite's dimensions
            float spriteWidth = spriteRenderer.sprite.bounds.size.x;
            float spriteHeight = spriteRenderer.sprite.bounds.size.y;

            // Calculate the scale needed to cover the screen
            float scaleX = worldScreenWidth / spriteWidth;
            float scaleY = worldScreenHeight / spriteHeight;

            // Apply the larger scale to ensure full coverage
            float scale = Mathf.Max(scaleX, scaleY);
            transform.localScale = new Vector3(scale, scale, 1f);

            // Position the background at the camera's position
            transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, 10f);
        }
    }
} 