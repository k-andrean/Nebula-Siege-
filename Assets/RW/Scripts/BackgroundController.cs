using UnityEngine;

namespace KelvinAndrean.NebulaSiege
{
    public class BackgroundController : MonoBehaviour
    {
        [SerializeField] private Sprite nebula1;
        [SerializeField] private Sprite nebula2;
        [SerializeField] private Sprite nebula3;
        private SpriteRenderer spriteRenderer;
        private Camera mainCamera;


        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            // Randomly select one of the nebula backgrounds
            Sprite selectedBackground = GetRandomBackground();
            if (selectedBackground != null)
            {
                spriteRenderer.sprite = selectedBackground;
                ScaleBackground();
            }
        }

        private Sprite GetRandomBackground()
        {
            int randomIndex = Random.Range(0, 3);
            switch (randomIndex)
            {
                case 0:
                    return nebula1;
                case 1:
                    return nebula2;
                case 2:
                    return nebula3;
                default:
                    return nebula1;
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