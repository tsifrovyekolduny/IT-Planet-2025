using Assets.Scripts.Core.Data;
using Assets.Scripts.Interfaces.Effects;
using System;
using UnityEngine;
using Assets.Scripts.Interfaces.Controllers;

namespace Assets.Scripts.Core.Effects
{
    public class CameraVerticalMovement : MonoBehaviour, ICameraMover
    {
        public ISwipeController swipeController;
        private float maxVerticalMovement = 2f;
        private float swipeSmoothness = 10f;

        private Vector3 defaultCameraPosition;
        private float verticalOffset = 0f;

        private bool isBlockingMoving = false;
        bool ICameraMover.IsBlockingMoving { get => isBlockingMoving; set => isBlockingMoving = value; }

        private bool isCameraAtTarget = false; // Флаг для отслеживания, достигла ли камера целевой позиции

        private void Start()
        {
            if (swipeController == null)
                swipeController = GetComponent<ISwipeController>();

            if (swipeController == null)
            {
                Debug.Log("swipeController not set");
                return;
            }

            maxVerticalMovement = swipeController.MaxVerticalMovement;
            swipeSmoothness = swipeController.SwipeSmoothness;

            defaultCameraPosition = transform.position; // Сохраняем начальную позицию камеры
            swipeController.OnSwipe.AddListener(HandleSwipe);
        }

        protected virtual void HandleSwipe(SwipeOrientation orientation)
        {
            if (isCameraAtTarget == false) return; // Если камера не на целевой игнорируем

            switch (orientation)
            {
                case SwipeOrientation.Up:
                    MoveCamera(maxVerticalMovement);
                    break;
                case SwipeOrientation.Down:
                    MoveCamera(-maxVerticalMovement);
                    break;
            }
        }

        public void MoveCamera(float offset)
        {
            verticalOffset = Mathf.Clamp(verticalOffset + offset, -maxVerticalMovement, maxVerticalMovement);
            isCameraAtTarget = false;
        }

        private void LateUpdate()
        {
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            if (isCameraAtTarget) return;

            // Динамическое обновление позиции камеры
            Vector3 newPos = defaultCameraPosition + Vector3.up * verticalOffset;
            newPos.z = transform.position.z; // Сохраняем оригинальную позицию по оси Z

            // Проверяем, достигла ли камера целевой позиции
            if (Vector3.Distance(transform.position, newPos) < 0.01f)
            {
                isCameraAtTarget = true; // Устанавливаем флаг, если камера достигла целевой позиции
                return; // Прекращаем обновление позиции
            }

            transform.position = Vector3.Lerp(transform.position, newPos, swipeSmoothness * Time.deltaTime);
        }

        // Метод для сброса состояния камеры, если это необходимо
        public void ResetCamera()
        {
            verticalOffset = 0f;
            isCameraAtTarget = false; // Сбрасываем флаг, чтобы камера могла снова двигаться
        }
    }
}