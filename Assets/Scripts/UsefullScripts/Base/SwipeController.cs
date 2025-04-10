using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [Header("Настройки свайпа")]
    [Rename("Чувствительность свайпа")]
    public float swipeSensitivity = 1f;
    [Rename("Плавность свайпа")]
    public float swipeSmoothness = 10f;
    [Rename("Инверсия свайпа")]
    [Tooltip("Если включено, свайпы будут работать в обратном направлении")]
    public bool reverseControls = false;

    [Header("Границы перемещения")]
    [Rename("Верхняя граница (объект)")]
    [Tooltip("Объект, выше которого камера не поднимется")]
    public Transform upperBoundaryObject;
    [Rename("Нижняя граница (объект)")]
    [Tooltip("Объект, ниже которого камера не опустится")]
    public Transform lowerBoundaryObject;

    [Header("Настройка камеры")]
    [Rename("Камера")]
    public Camera targetCamera;

    [Header("Sensitivity Settings")]
    [Rename("Множитель чувствительности свайпа")]
    [Range(0.01f, 1f)]
    public float swipeSensitivityMultiplier = 0.5f; // Уменьшает общую чувствительность
    [Rename("")]
    [Range(0.1f, 2f)]
    public float swipeSpeedDamping = 0.8f;

    protected float returnProgress; // Трекер прогресса возврата

    private float currentYPosition; // Текущая позиция камеры по Y
    private float targetYPosition; // Целевая позиция после свайпа
    private Vector2 touchStartPos;
    private bool isEnabled = true;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        // Начальная позиция камеры — текущая, но ограниченная границами
        currentYPosition = targetCamera.transform.position.y;
        targetYPosition = currentYPosition;

        if (upperBoundaryObject == null || lowerBoundaryObject == null)
        {
            Debug.LogWarning("Границы не назначены! Камера может двигаться без ограничений.");
        }
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
                if (reverseControls) deltaY *= -1;

                targetYPosition += deltaY;
                ClampTargetPosition(); // Ограничиваем целевую позицию границами
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
                if (reverseControls) deltaY *= -1;

                targetYPosition += deltaY;
                ClampTargetPosition(); // Ограничиваем целевую позицию границами
                touchStartPos = Input.mousePosition;
            }
        }
    }

    private void ClampTargetPosition()
    {
        if (upperBoundaryObject != null && lowerBoundaryObject != null)
        {
            targetYPosition = Mathf.Clamp(
                targetYPosition,
                lowerBoundaryObject.position.y,
                upperBoundaryObject.position.y
            );
        }
    }

    private void ApplyMovement()
    {
        // Плавное перемещение к целевой позиции
        currentYPosition = Mathf.Lerp(
            currentYPosition,
            targetYPosition,
            swipeSmoothness * Time.deltaTime
        );

        // Обновляем позицию камеры (сохраняем X и Z)
        Vector3 newPos = targetCamera.transform.position;
        newPos.y = currentYPosition;
        targetCamera.transform.position = newPos;
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
}