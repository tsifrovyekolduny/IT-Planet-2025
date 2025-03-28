using UnityEngine;

public class TouchZoomToObject : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;          // Скорость приближения
    public float targetOrthoSize = 3f;    // Размер камеры при зуме
    public float releaseSpeed = 3f;       // Скорость возврата
    public LayerMask interactableLayer;   // Слой объектов для взаимодействия

    private Camera mainCamera;
    private float defaultOrthoSize;
    private Vector3 defaultPosition;
    private bool isZooming = false;
    private Vector3 targetPosition;

    private void Start()
    {
        mainCamera = Camera.main;
        defaultOrthoSize = mainCamera.orthographicSize;
        defaultPosition = mainCamera.transform.position;
    }

    void Update()
    {
        // Обработка касаний
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = mainCamera.ScreenPointToRay(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);

            if (touch.phase == TouchPhase.Began && hit.collider != null)
            {
                isZooming = true;
                targetPosition = hit.point; // Фиксируем точку касания
                targetPosition.z = mainCamera.transform.position.z; // Сохраняем Z-координату камеры
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isZooming = false;
            }
        }

        else // Для теста в редакторе (если нужно)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);
                if (hit.collider != null)
                {
                    isZooming = true;
                    targetPosition = hit.point; // Фиксируем точку касания
                    targetPosition.z = mainCamera.transform.position.z;
                }
            }
            if (Input.GetMouseButtonUp(0)) isZooming = false;
        }

        // Плавный зум и движение камеры
        if (isZooming)
        {
            // Приближаем камеру к targetPosition
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                targetPosition,
                zoomSpeed * Time.deltaTime
            );

            // Уменьшаем orthographicSize
            mainCamera.orthographicSize = Mathf.Lerp(
                mainCamera.orthographicSize,
                targetOrthoSize,
                zoomSpeed * Time.deltaTime
            );
        }
        else
        {
            // Возвращаем камеру в исходное положение
            mainCamera.transform.position = Vector3.Lerp(
                mainCamera.transform.position,
                defaultPosition,
                releaseSpeed * Time.deltaTime
            );

            // Восстанавливаем размер камеры
            mainCamera.orthographicSize = Mathf.Lerp(
                mainCamera.orthographicSize,
                defaultOrthoSize,
                releaseSpeed * Time.deltaTime
            );
        }
    }
}