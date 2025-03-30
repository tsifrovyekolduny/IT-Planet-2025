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
    public bool disableSwipeDuringZoom = true;

    // Защищенные поля для наследования
    protected float defaultOrthoSize;
    protected Vector3 preZoomPosition;
    protected float preZoomOrthoSize;
    protected bool isZooming = false;
    protected Vector3 zoomTargetPosition;
    protected float returnProgress = 0f;
    protected bool isReturning = false;
    protected SwipeController swipeController;
    protected GameObject lastZoomedObject;
    protected Vector3 returnStartPosition;
    protected float returnStartOrthoSize;
    protected float lastTapTime;
    protected const float doubleTapThreshold = 0.5f;

    protected virtual void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        defaultOrthoSize = targetCamera.orthographicSize;
        swipeController = GetComponent<SwipeController>();
    }

    protected virtual void Update()
    {
        HandleInput();
        ProcessZoom();
        ProcessReturn();
    }

    protected virtual void HandleInput()
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

    protected virtual void HandleDoubleTapInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 inputPos = GetInputPosition();
            Ray ray = targetCamera.ScreenPointToRay(inputPos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);

            if (hit.collider != null)
            {
                if (isZooming)
                {
                    EndZoom();
                }
                else
                {
                    StartZoom(hit.point, hit.collider.gameObject);
                }
                lastTapTime = Time.time;
            }
            else if (isZooming)
            {
                EndZoom();
            }
        }
    }

    protected virtual void HandleStandardInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Vector2 inputPos = GetInputPosition();
            Ray ray = targetCamera.ScreenPointToRay(inputPos);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, interactableLayer);

            if (hit.collider != null && !isZooming)
            {
                StartZoom(hit.point, hit.collider.gameObject);
            }
        }
        else if ((Input.GetMouseButtonUp(0) ||
                (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && isZooming)
        {
            EndZoom();
        }
    }

    protected virtual Vector2 GetInputPosition()
    {
        return Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
    }

    protected virtual void StartZoom(Vector3 worldPosition, GameObject targetObject)
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

    protected virtual void EndZoom()
    {
        if (!isZooming) return;

        isZooming = false;
        isReturning = true;
        returnProgress = 0f;
        returnStartPosition = targetCamera.transform.position;
        returnStartOrthoSize = targetCamera.orthographicSize;
    }

    protected virtual void ProcessZoom()
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

    protected virtual void ProcessReturn()
    {
        if (!isReturning) return;

        returnProgress += releaseSpeed * Time.deltaTime;
        float t = Mathf.Clamp01(returnProgress);

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