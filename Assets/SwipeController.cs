using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [Header("Swipe Settings")]
    public float maxVerticalMovement = 2f;
    public float swipeSensitivity = 1f;
    public float swipeSmoothness = 5f;

    [Header("Camera Reference")]
    public Camera targetCamera;

    private Vector3 defaultCameraPosition;
    private Vector2 touchStartPos;
    private float verticalOffset = 0f;
    private float targetVerticalOffset = 0f;
    private bool isEnabled = true;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        defaultCameraPosition = targetCamera.transform.position;
    }

    private void Update()
    {
        if (!isEnabled) return;

        HandleInput();
        ApplySwipeMovement();
    }

    public void UpdateDefaultPosition(Vector3 newPosition)
    {
        defaultCameraPosition = newPosition;
        // Сбрасываем смещение, чтобы не было резкого прыжка
        verticalOffset = 0f;
        targetVerticalOffset = 0f;
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                float deltaY = (touch.position.y - touchStartPos.y) * swipeSensitivity;
                targetVerticalOffset = Mathf.Clamp(verticalOffset + deltaY, -maxVerticalMovement, maxVerticalMovement);
                touchStartPos = touch.position;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
            }
            else
            {
                float deltaY = (Input.mousePosition.y - touchStartPos.y) * swipeSensitivity;
                targetVerticalOffset = Mathf.Clamp(verticalOffset + deltaY, -maxVerticalMovement, maxVerticalMovement);
                touchStartPos = Input.mousePosition;
            }
        }
    }

    private void ApplySwipeMovement()
    {
        verticalOffset = Mathf.Lerp(verticalOffset, targetVerticalOffset, swipeSmoothness * Time.deltaTime);

        Vector3 newPos = defaultCameraPosition + Vector3.up * verticalOffset;
        newPos.z = targetCamera.transform.position.z;
        targetCamera.transform.position = newPos;
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        if (!enabled)
        {
            // Плавный сброс смещения при отключении
            verticalOffset = 0f;
            targetVerticalOffset = 0f;
        }
    }
}