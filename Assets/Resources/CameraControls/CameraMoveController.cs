using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CameraControls
{
    public class CameraMoveController : MonoBehaviour
    {
        public float doubleClickTimeLimit = 0.3f;

        private float lastClickTime = 0f;
        private new Camera camera;

        private Vector3 dragOrigin;

        private void Awake()
        {
            camera = GetComponent<Camera>();
        }

        private void Update()
        {
            if (CheckForUI())
                return;

            CheckMouseMove();
            CheckDoubleClick();
        }

        private bool CheckForUI()
        {
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }

        private void CheckDoubleClick()
        {
            if (!Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
                return;

            if (Time.time - lastClickTime < doubleClickTimeLimit)
            {
                var newPosition = Vector3.zero;
                newPosition.z = -10;
                transform.position = newPosition;
                dragOrigin = camera.ScreenToWorldPoint(Input.mousePosition);
            }
            lastClickTime = Time.time;
        }

        private void CheckMouseMove()
        {
            foreach (int i in Enum.GetValues(typeof(MouseButton)))
            {
                if (Input.GetMouseButtonDown(i))
                {
                    dragOrigin = camera.ScreenToWorldPoint(Input.mousePosition);
                }

                if (Input.GetMouseButton(i))
                {
                    var difference = dragOrigin - camera.ScreenToWorldPoint(Input.mousePosition);
                    camera.transform.position += difference;
                }
            }
        }
    }
}
