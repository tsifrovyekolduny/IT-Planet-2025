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
    public bool useDoubleTapMode = false; // false = отпускание, true = двойное нажатие

    [Header("Swipe Integration")]
    public bool disableSwipeDuringZoom = true;

    private float defaultOrthoSize;
    private GameObject lastZoomedObject;
    private Vector3 preZoomPosition;
    private float preZoomOrthoSize;
    private bool isZooming = false;
    private Vector3 zoomTargetPosition;
    private float returnProgress = 0f;
    private bool isReturning = false;
    private SwipeController swipeController;
    private bool hasSwipeController;
    private bool shouldUpdateSwipeBasePosition;
    private bool waitingForSecondTap = false;
    private float lastTapTime;
    private const float doubleTapThreshold = 0.3f;

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

        if (!isReturning && !isZooming && hasSwipeController && shouldUpdateSwipeBasePosition)
        {
            swipeController.UpdateDefaultPosition(targetCamera.transform.position);
            shouldUpdateSwipeBasePosition = false;
        }
    }

    private void HandleInput()
    {
        if (useDoubleTapMode)
        {
            HandleDoubleTapMode();
        }
        else
        {
            HandleReleaseMode();
        }
    }

    private void HandleReleaseMode()
    {
        // Режим "отпускание = возврат"
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 inputPos = Input.mousePosition;
            if (Input.touchCount > 0) inputPos = Input.GetTouch(0).position;

            TryStartZoom(inputPos);
        }
        else if ((Input.GetMouseButtonUp(0) ||
                 (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && isZooming)
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
            if (useDoubleTapMode)
            {
                if (isZooming && hit.collider.gameObject == lastZoomedObject)
                {
                    // Второе нажатие на тот же объект - возврат
                    EndZoom();
                    lastZoomedObject = null;
                }
                else if (!isZooming)
                {
                    // Первое нажатие - зум
                    preZoomPosition = targetCamera.transform.position;
                    preZoomOrthoSize = targetCamera.orthographicSize;
                    StartZoom(hit.point);
                    lastZoomedObject = hit.collider.gameObject;
                }
            }
            else
            {
                // Обычный режим
                preZoomPosition = targetCamera.transform.position;
                preZoomOrthoSize = targetCamera.orthographicSize;
                StartZoom(hit.point);
            }

            if (disableSwipeDuringZoom && hasSwipeController)
            {
                swipeController.SetEnabled(false);
            }
        }
    }

    private void HandleDoubleTapMode()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ProcessTap(Input.mousePosition);
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            ProcessTap(Input.GetTouch(0).position);
        }
    }

    private void ProcessTap(Vector2 inputPosition)
    {
        Ray ray = targetCamera.ScreenPointToRay(inputPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);

        if (hit.collider != null)
        {
            if (isZooming && hit.collider.gameObject == lastZoomedObject)
            {
                // Второе нажатие на тот же объект
                EndZoom();
                lastZoomedObject = null;
            }
            else if (!isZooming)
            {
                // Первое нажатие
                preZoomPosition = targetCamera.transform.position;
                preZoomOrthoSize = targetCamera.orthographicSize;
                StartZoom(hit.point);
                lastZoomedObject = hit.collider.gameObject;
            }
        }
        else if (isZooming)
        {
            // Сброс при нажатии мимо объекта
            waitingForSecondTap = false;
        }
    }
    private void StartZoom(Vector3 worldPosition)
    {
        isZooming = true;
        isReturning = false;
        zoomTargetPosition = new Vector3(worldPosition.x, worldPosition.y, targetCamera.transform.position.z);
        shouldUpdateSwipeBasePosition = true;
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