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

    [Header("Swipe Integration")]
    public bool disableSwipeDuringZoom = true;

    private float defaultOrthoSize;
    private Vector3 preZoomPosition;
    private float preZoomOrthoSize;
    private bool isZooming = false;
    private Vector3 zoomTargetPosition;
    private float returnProgress = 0f;
    private bool isReturning = false;
    private SwipeController swipeController;
    private bool hasSwipeController;
    private bool shouldUpdateSwipeBasePosition; // Новый флаг

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        defaultOrthoSize = targetCamera.orthographicSize;

        swipeController = GetComponent<SwipeController>();
        hasSwipeController = swipeController != null;
        shouldUpdateSwipeBasePosition = true;
    }

    private void Update()
    {
        HandleInput();
        ProcessZoom();
        ProcessReturn();

        // Обновляем базовую позицию свайпа после завершения возврата
        if (!isReturning && !isZooming && hasSwipeController && shouldUpdateSwipeBasePosition)
        {
            swipeController.UpdateDefaultPosition(targetCamera.transform.position);
            shouldUpdateSwipeBasePosition = false;
        }
    }

    private void HandleInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                TryStartZoom(touch.position);
            }
            else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isZooming)
            {
                EndZoom();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            TryStartZoom(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && isZooming)
        {
            EndZoom();
        }
    }

    private void TryStartZoom(Vector2 screenPosition)
    {
        Ray ray = targetCamera.ScreenPointToRay(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);

        if (hit.collider != null)
        {
            preZoomPosition = targetCamera.transform.position;
            preZoomOrthoSize = targetCamera.orthographicSize;

            StartZoom(hit.point);

            if (disableSwipeDuringZoom && hasSwipeController)
            {
                swipeController.SetEnabled(false);
            }
        }
    }

    private void StartZoom(Vector3 worldPosition)
    {
        isZooming = true;
        isReturning = false;
        zoomTargetPosition = new Vector3(worldPosition.x, worldPosition.y, targetCamera.transform.position.z);
        shouldUpdateSwipeBasePosition = true; // Готовимся к обновлению позиции
    }

    private void EndZoom()
    {
        isZooming = false;
        isReturning = true;
        returnProgress = 0f;
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

        targetCamera.transform.position = Vector3.Lerp(
            targetCamera.transform.position,
            preZoomPosition,
            t
        );

        targetCamera.orthographicSize = Mathf.Lerp(
            targetCamera.orthographicSize,
            preZoomOrthoSize,
            t
        );

        if (t >= 1f)
        {
            isReturning = false;

            if (disableSwipeDuringZoom && hasSwipeController)
            {
                swipeController.SetEnabled(true);
            }
        }
    }
}