using UnityEngine;

public class SwipeController : MonoBehaviour
{

    [Header("Swipe Settings")]
    public float maxVerticalMovement = 2f;
    public float swipeSensitivity = 1f;
    public float swipeSmoothness = 10f;
    [Tooltip("Если включено, свайпы будут работать в обратном направлении")]
    public bool reverseControls = false; // Новая переменная

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

    private void LateUpdate()
    {
        if (!isEnabled) return;

        HandleInput();
        ApplyMovement();
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

                // Реверс направления при необходимости
                if (reverseControls) deltaY *= -1;

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

                // Реверс направления при необходимости
                if (reverseControls) deltaY *= -1;

                targetVerticalOffset = Mathf.Clamp(verticalOffset + deltaY, -maxVerticalMovement, maxVerticalMovement);
                touchStartPos = Input.mousePosition;
            }
        }
    }

    private void ApplyMovement()
    {
        verticalOffset = Mathf.Lerp(verticalOffset, targetVerticalOffset, swipeSmoothness * Time.deltaTime);

        Vector3 newPos = defaultCameraPosition + Vector3.up * verticalOffset;
        newPos.z = targetCamera.transform.position.z;
        targetCamera.transform.position = newPos;
    }

    public float GetVerticalOffset()
    {
        return verticalOffset;
    }

    public void UpdateDefaultPosition(Vector3 newPosition)
    {
        defaultCameraPosition = newPosition;
        verticalOffset = 0f;
        targetVerticalOffset = 0f;
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
        if (!enabled)
        {
            verticalOffset = 0f;
            targetVerticalOffset = 0f;
        }
    }
}