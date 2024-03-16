using UnityEngine;
using Utility;

namespace CameraControls
{
    class CameraZoomController : MonoBehaviour
    {
        public float zoomStep = 4f;
        public float zoomSpeed = 10f;
        public float minZoomStep = 1f;
        public float minZoom = 2f,
            maxZoom = 10f;
        private float zoomTarget;
        private new Camera camera;
        public DebouncedSave prefs;

        private void Awake()
        {
            if (prefs == null)
                throw new System.Exception("prefs is null");
            camera = GetComponent<Camera>();
            zoomTarget = camera.orthographicSize;
            zoomTarget = PlayerPrefs.GetFloat("zoom", zoomTarget);
            camera.orthographicSize = zoomTarget;

            // Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;
        }

        private void Update()
        {
            UpdateZoomTarget();
            MoveTowardsZoomTarget();
        }

        private void MoveTowardsZoomTarget()
        {
            if (camera.orthographicSize == zoomTarget)
                return;

            Vector3 oldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

            float maxZoomDelta = Mathf.Abs(camera.orthographicSize - zoomTarget) * zoomSpeed;
            maxZoomDelta = Mathf.Max(maxZoomDelta, minZoomStep);
            camera.orthographicSize = Mathf.MoveTowards(
                camera.orthographicSize,
                zoomTarget,
                maxZoomDelta * Time.deltaTime
            );

            Vector3 newPosition = camera.ScreenToWorldPoint(Input.mousePosition);

            transform.position += oldPosition - newPosition;
        }

        private void UpdateZoomTarget()
        {
            float scrollData = Input.GetAxis("Mouse ScrollWheel");
            if (scrollData == 0f)
                return;
            if (!camera.pixelRect.Contains(Input.mousePosition))
                return;

            var oldTarget = zoomTarget;
            zoomTarget -= scrollData * zoomStep;
            zoomTarget = Mathf.Clamp(zoomTarget, minZoom, maxZoom);

            if (oldTarget != zoomTarget)
            {
                PlayerPrefs.SetFloat("zoom", zoomTarget);
                prefs.Save();
            }
        }
    }
}
