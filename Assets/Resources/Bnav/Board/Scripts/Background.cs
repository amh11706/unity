using UnityEngine;

namespace BnavBoard
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BackgroundTiler : MonoBehaviour
    {
        public float maxZoom = 10f;
        private Camera cam;
        private SpriteRenderer spriteRenderer;
        private float textureWidth;
        private float textureHeight;

        private void Start()
        {
            cam = Camera.main;
            spriteRenderer = GetComponent<SpriteRenderer>();
            textureWidth =
                spriteRenderer.sprite.texture.width / spriteRenderer.sprite.pixelsPerUnit;
            textureHeight =
                spriteRenderer.sprite.texture.height / spriteRenderer.sprite.pixelsPerUnit;
            SetMaxSize();
        }

        private void LateUpdate()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            var viewPos = cam.transform.position;

            float newX = Mathf.Round(viewPos.x / textureWidth) * textureWidth;
            float newY = Mathf.Round(viewPos.y / textureHeight) * textureHeight;

            var newPosition = new Vector3(newX, newY, transform.position.z);

            if (newPosition != transform.position)
            {
                transform.position = newPosition;
            }
        }

        private void SetMaxSize()
        {
            if (cam == null || spriteRenderer == null)
                return;

            var viewHeight = maxZoom * 2; // 10 is the nax size of the camera
            var viewWidth = viewHeight * cam.aspect;

            var spriteSize = spriteRenderer.sprite.bounds.size;

            var tilesY = Mathf.CeilToInt(viewHeight / spriteSize.y) + 1;
            var tilesX = Mathf.CeilToInt(viewWidth / spriteSize.x) + 1;

            spriteRenderer.size = new Vector2(tilesX * spriteSize.x, tilesY * spriteSize.y);
        }
    }
}
