using Assets.Scripts.Interfaces.Controllers;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Core.Controllers
{
    internal class ZoomController : MonoBehaviour, IZoomeController
    {
        [Header("Настройки приближения")]
        [Rename("Скорость приближения")]
        public float zoomSpeed = 5f;
        [Rename("Степень приближения")]
        public float targetOrthoSize = 3f;
        [Rename("Скорость отдаления")]
        public float releaseSpeed = 5f;

        // Защищенные поля для наследования
        protected float defaultOrthoSize;
        protected Vector3 preZoomPosition;
        protected float preZoomOrthoSize;
        protected bool isZooming = false;
        protected Vector3 zoomTargetPosition;
        protected float returnProgress = 0f;
        protected bool isReturning = false;
        protected GameObject lastZoomedObject;
        protected Vector3 returnStartPosition;
        protected float returnStartOrthoSize;

        public UnityEvent OnZoomStart;
        public UnityEvent OnZoomEnd;
        public UnityEvent OnReleaseStart;
        public UnityEvent OnReleaseEnd;

        [Header("Настройка камеры")]
        [Rename("Камера")]
        public Camera targetCamera;

        // Прогресс зума
        private float zoomProgress = 0f;
        bool isFocused = false;

        public bool IsFocused { get => isFocused; }

        protected virtual void Start()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;

            defaultOrthoSize = targetCamera.orthographicSize;
        }

        protected virtual void Update()
        {
            ProcessZoom();
            ProcessReturn();
        }

        protected virtual Vector2 GetInputPosition()
        {
            return Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
        }

        protected virtual void StartZoom(Vector3 worldPosition, GameObject targetObject)
        {
            isFocused = true;
            preZoomPosition = targetCamera.transform.position;
            preZoomOrthoSize = targetCamera.orthographicSize;

            isZooming = true;
            isReturning = false;
            lastZoomedObject = targetObject;
            zoomTargetPosition = new Vector3(worldPosition.x, worldPosition.y, targetCamera.transform.position.z);
            zoomProgress = 0f; // Сброс прогресса зума
            OnZoomStart?.Invoke();
        }

        protected virtual void EndZoom()
        {
            if (!isZooming) return;

            isZooming = false;
            returnProgress = 0f;
            targetCamera.transform.position = zoomTargetPosition;
            targetCamera.orthographicSize = targetOrthoSize;
            OnZoomEnd?.Invoke();
        }

        protected virtual void ReturnZoom()
        {
            isReturning = true;
            returnStartPosition = targetCamera.transform.position;
            returnStartOrthoSize = targetCamera.orthographicSize;
        }

        protected virtual void ProcessZoom()
        {
            if (!isZooming) return;

            // Увеличиваем прогресс зума
            zoomProgress += zoomSpeed * Time.deltaTime;
            float t = Mathf.Clamp01(zoomProgress);

            targetCamera.transform.position = Vector3.Lerp(
                targetCamera.transform.position,
                zoomTargetPosition,
                t
            );

            targetCamera.orthographicSize = Mathf.Lerp(
                targetCamera.orthographicSize,
                targetOrthoSize,
                t
            );

            // Если прогресс зума завершен, автоматически вызываем EndZoom
            if (t >= 1f)
            {
                EndZoom();
            }
        }

        protected virtual void ProcessReturn()
        {
            if (!isReturning) return;

            returnProgress += releaseSpeed * Time.deltaTime;
            float t = Mathf.Clamp01(returnProgress);

            Vector3 swipeAdjustedPosition = preZoomPosition;

            targetCamera.transform.position = Vector3.Lerp(
                returnStartPosition,
                swipeAdjustedPosition,
                t
            );

            targetCamera.orthographicSize = Mathf.Lerp(
                returnStartOrthoSize,
                preZoomOrthoSize,
                t
            );

            if (t >= 1f)
            {
                isReturning = false;
                isFocused = false;
                OnReleaseEnd?.Invoke();
            }
        }

        void IZoomeController.StartZoom(Vector3 worldPosition, GameObject targetObject)
        {
            StartZoom(worldPosition, targetObject);
        }
        void IZoomeController.ReturnZoom()
        {
            ReturnZoom();
        }
    }
}