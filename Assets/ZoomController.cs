using UnityEngine;

public class ZoomController : MonoBehaviour
{
    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float targetOrthoSize = 3f;
    public float releaseSpeed = 5f;
    public LayerMask interactableLayer;

    [Header("Camera Reference")]
    public Camera targetCamera;

    [Header("Behavior Mode")]
    public bool useDoubleTapMode = false;

    private bool disableSwipeDuringZoom = true;

    // Приватные переменные
    private float defaultOrthoSize;
    private Vector3 preZoomPosition;
    private float preZoomOrthoSize;
    private bool isZooming = false;
    private Vector3 zoomTargetPosition;
    private float returnProgress = 0f;
    private bool isReturning = false;
    private SwipeController swipeController;
    private GameObject lastZoomedObject;
    private Vector3 returnStartPosition;
    private float returnStartOrthoSize;
    private float lastTapTime;
    private const float doubleTapThreshold = 0.5f;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        defaultOrthoSize = targetCamera.orthographicSize;
        swipeController = GetComponent<SwipeController>();
    }

    private void Update()
    {
        HandleInput();
        ProcessZoom();
        ProcessReturn();
    }

    private void HandleInput()
    {
        if (useDoubleTapMode)
        {
            HandleDoubleTapInput();
        }
        else
        {
            HandleStandardInput();
        }
    }

    private void HandleDoubleTapInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 inputPos = Input.mousePosition;
            if (Input.touchCount > 0) inputPos = Input.GetTouch(0).position;

            Ray ray = targetCamera.ScreenPointToRay(inputPos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);

            if (hit.collider != null)
            {
                if (isZooming)
                {
                    // Если уже в режиме зума - возвращаем камеру
                    EndZoom();
                }
                else
                {
                    // Если не в режиме зума - приближаем
                    StartZoom(hit.point, hit.collider.gameObject);
                }
                lastTapTime = Time.time;
            }
            else if (isZooming)
            {
                // Нажатие вне объекта - возврат
                EndZoom();
            }
        }
    }

    private void HandleStandardInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 inputPos = Input.mousePosition;
            if (Input.touchCount > 0) inputPos = Input.GetTouch(0).position;

            Ray ray = targetCamera.ScreenPointToRay(inputPos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);

            if (hit.collider != null && !isZooming)
            {
                StartZoom(hit.point, hit.collider.gameObject); // Заменяем TryStartZoom на StartZoom
            }
        }
        else if ((Input.GetMouseButtonUp(0) ||
                 (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && isZooming)
        {
            EndZoom();
        }
    }

    private void StartZoom(Vector3 worldPosition, GameObject targetObject)
    {
        preZoomPosition = targetCamera.transform.position;
        preZoomOrthoSize = targetCamera.orthographicSize;

        isZooming = true;
        isReturning = false;
        lastZoomedObject = targetObject;
        zoomTargetPosition = new Vector3(worldPosition.x, worldPosition.y, targetCamera.transform.position.z);

        if (swipeController != null && disableSwipeDuringZoom)
        {
            swipeController.enabled = false;
        }
    }

    private void EndZoom()
    {
        if (!isZooming) return;

        isZooming = false;
        isReturning = true;
        returnProgress = 0f;
        returnStartPosition = targetCamera.transform.position;
        returnStartOrthoSize = targetCamera.orthographicSize;
    }

    private void ProcessZoom()
    {
        if (!isZooming) return;

        targetCamera.transform.position = Vector3.Lerp(
            targetCamera.transform.position,
            zoomTargetPosition,
            zoomSpeed * Time.deltaTime
        );

        targetCamera.orthographicSize = Mathf.Lerp(
            targetCamera.orthographicSize,
            targetOrthoSize,
            zoomSpeed * Time.deltaTime
        );
    }

    private void ProcessReturn()
    {
        if (!isReturning) return;

        returnProgress += releaseSpeed * Time.deltaTime;
        float t = Mathf.Clamp01(returnProgress);

        // Создаем целевую позицию с учетом свайпа
        Vector3 swipeAdjustedPosition = preZoomPosition;
        if (swipeController != null && !disableSwipeDuringZoom)
        {
            swipeAdjustedPosition += Vector3.up * swipeController.GetVerticalOffset();
        }

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
            if (swipeController != null)
            {
                swipeController.enabled = true;
                swipeController.UpdateDefaultPosition(targetCamera.transform.position);
            }
        }
    }
}