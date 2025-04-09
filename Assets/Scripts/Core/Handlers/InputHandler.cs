using Assets.Scripts.Interfaces.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Core.Handlers
{
    public class InputHandler : TouchInputHandler, IInputHandler
    {
        [Rename("Рабочие слои")]
        [Tooltip("Скрипт будет работать на элементах на выбранном слое")]
        public LayerMask interactableLayer;

        public UnityEvent<Vector3, GameObject> OnObjectSelected = new UnityEvent<Vector3, GameObject>();
        public UnityEvent<Vector3> OnSelectionEmpty = new UnityEvent<Vector3>();
        UnityEvent<Vector3, GameObject> IInputHandler.OnObjectSelected => this.OnObjectSelected;

        UnityEvent<Vector3> IInputHandler.OnSelectionEmpty => this.OnSelectionEmpty;

        [Rename("Режим двойного нажатия(не работает)")]
        [Tooltip("Если настройка включена, то отдаление будет происходить при втором нажатии")]
        public bool useDoubleTapMode = false;

        [Header("Настройка камеры")]
        [Rename("Камера")]
        public Camera targetCamera;

        private bool isWasSwipe = false;

        protected virtual void Start()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            OnSingleTap.AddListener(HandeSingleTap);
            OnSwipe.AddListener(HandeSwipeTap);
            OnTouchEnd.AddListener(HandleInput);
        }

        private void HandeSingleTap(Vector2 pos)
        {
            isWasSwipe = false;
        }
        private void HandeSwipeTap(Vector2 pos)
        {
            isWasSwipe = true;
        }

        protected virtual void HandleInput(Vector2 pos)
        {
            if (isWasSwipe == false)
            {
                if (useDoubleTapMode)
                {
                    HandleDoubleTapInput();
                }
                else
                {
                    HandleStandardInput(pos);
                }
            }
        }

        protected virtual void HandleDoubleTapInput()
        {
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                Vector2 inputPos = GetInputPosition();
                Ray ray = targetCamera.ScreenPointToRay(inputPos);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);

                if (hit.collider != null)
                {
                    OnObjectSelected?.Invoke(hit.point, hit.collider.gameObject);
                } else
                {
                    OnSelectionEmpty.Invoke(inputPos);
                }
            }
        }

        protected virtual void HandleStandardInput(Vector2 inputPos)
        {
                Ray ray = targetCamera.ScreenPointToRay(inputPos);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);

                if (hit.collider != null)
                {
                    OnObjectSelected?.Invoke(hit.point, hit.collider.gameObject);
                }else
                {
                    OnSelectionEmpty.Invoke(inputPos);
                }
        }

        private Vector2 GetInputPosition()
        {
            return Input.mousePosition;
        }
    }
}
