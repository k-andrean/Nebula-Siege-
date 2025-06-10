using UnityEngine;

namespace KelvinAndrean.NebulaSiege
{
    public class OtherColorInvader : MonoBehaviour
    {
        [SerializeField] private Color specialColor = Color.yellow;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            // Apply special color to the invader
            if (spriteRenderer != null)
            {
                spriteRenderer.color = specialColor;
            }
        }
    }
} 