using Assets.Scripts.Core.Data;
using Assets.Scripts.Interfaces.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SwipeController = Assets.Scripts.Core.Controllers.SwipeController;
using UnityEngine;
using Assets.Scripts.Interfaces.Controllers;
using System.Collections;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Core.Effects
{
    public class CameraVerticalMovement : MonoBehaviour, ICameraMover
    {
        public ISwipeController swipeController;
        private float maxVerticalMovement = 2f;
        private float swipeSmoothness = 10f;

        private Vector3 defaultCameraPosition;
        private float verticalOffset = 0f;

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

            defaultCameraPosition = transform.position;
            swipeController.OnSwipe.AddListener(HandleSwipe);
        }

        protected virtual void HandleSwipe(SwipeOrientation orientation)
        {
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
            UpdateDefaultPosition(); // Обновляем позицию по умолчанию при движении
        }

        private void UpdateDefaultPosition()
        {
            defaultCameraPosition = transform.position; // Обновляем значение defaultCameraPosition
        }

        private void LateUpdate()
        {
            ApplyMovement();
        }

        private void ApplyMovement()
        {
            // Динамическое обновление позиции камеры
            Vector3 newPos = defaultCameraPosition + Vector3.up * verticalOffset;
            newPos.z = transform.position.z; // Сохраняем оригинальную позицию по оси Z
            transform.position = Vector3.Lerp(transform.position, newPos, swipeSmoothness * Time.deltaTime);
        }
    }
}
