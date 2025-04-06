using Assets.Scripts.Interfaces.Controllers;
using global::Assets.Scripts.Core.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Core.Controllers
{
    public class SwipeController : MonoBehaviour, ISwipeController
    {
        [Header("Настройки свайпа")]
        public float maxVerticalMovement = 2f;
        public float swipeSensitivity = 1f;
        public bool reverseControls = false;

        public float MaxVerticalMovement => maxVerticalMovement;
        public float SwipeSmoothness => swipeSensitivity;

        private Vector2 touchStartPos;

        public UnityEvent<SwipeOrientation> OnSwipe;

        private void Start()
        {
            OnSwipe = new UnityEvent<SwipeOrientation>();
        }
        UnityEvent<SwipeOrientation> ISwipeController.OnSwipe
        {
            get { return this.OnSwipe; }
            set { this.OnSwipe = value; }
        }


        private void LateUpdate()
        {
            HandleInput();
        }

        virtual protected void HandleInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                ProcessTouch(touch);
            }
            else if (Input.GetMouseButton(0))
            {
                ProcessMouse();
            }
        }

        virtual protected void ProcessTouch(Touch touch)
        {
            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                DetectSwipe(touch.position);
            }
        }

        virtual protected void ProcessMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
            }
            else
            {
                DetectSwipe(Input.mousePosition);
            }
        }

        virtual protected void DetectSwipe(Vector2 currentTouchPos)
        {
            float deltaY = (currentTouchPos.y - touchStartPos.y) * swipeSensitivity;

            if (reverseControls) deltaY *= -1;

            if (Mathf.Abs(deltaY) > maxVerticalMovement)
            {
                SwipeOrientation orientation = deltaY > 0 ? SwipeOrientation.Up : SwipeOrientation.Down;
                OnSwipe.Invoke(orientation);
                touchStartPos = currentTouchPos; // Reset start position for next swipe
            }
        }
    }
}