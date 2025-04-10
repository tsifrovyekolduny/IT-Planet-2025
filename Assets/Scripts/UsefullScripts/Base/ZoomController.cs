using UnityEngine;
using UnityEngine.Serialization;

public class ZoomController : MonoBehaviour
{
    [Header("Настройки зума")]
    [Rename("Скорость приближения")]
    [SerializeField] protected float zoomSpeed = 5f;
    [Rename("Уровень приближения")]
    [SerializeField] protected float targetOrthoSize = 3f;
    [Rename("Скорость отдаления")]
    [SerializeField] protected float releaseSpeed = 5f;
    [Rename("Слой взаимодействия")]
    [SerializeField] protected LayerMask interactableLayer;

    [Header("Настройки камеры")]
    [Rename("Камера")]
    [SerializeField] protected Camera targetCamera;

    [Header("Поведение")]
    [Rename("Зум в центр объекта")]
    [SerializeField] protected bool zoomToColliderCenter = false;
    [Rename("Режим двойного нажатия")]
    [SerializeField] protected bool useDoubleTapMode = false;
    [Rename("Отключить свайп при зуме")]
    [SerializeField] protected bool disableSwipeDuringZoom = true;

    protected float defaultOrthoSize;
    protected bool isZooming = false;
    protected bool isReturning = false;
    protected Vector3 zoomTargetPosition;
    protected SwipeController swipeController;
    protected GameObject lastZoomedObject;
    protected Vector3 zoomStartPosition; // Точка начала зума
    protected float zoomStartSize; // Начальный размер при зуме

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

        if (isZooming)
            ProcessZoom();

        if (isReturning)
            ProcessReturn();
    }

    protected virtual void HandleInput()
    {
        if (useDoubleTapMode)
            HandleDoubleTapInput();
        else
            HandleStandardInput();
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
                if (isZooming) EndZoom();
                else StartZoom(hit.point, hit.collider.gameObject);
            }
            else if (isZooming) EndZoom();
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
                StartZoom(hit.point, hit.collider.gameObject);
        }
        else if ((Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)) && isZooming)
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
        zoomStartPosition = targetCamera.transform.position;
        zoomStartSize = targetCamera.orthographicSize;

        isZooming = true;
        isReturning = false;
        lastZoomedObject = targetObject;

        zoomTargetPosition = zoomToColliderCenter ?
            targetObject.GetComponent<Collider2D>().bounds.center :
            worldPosition;
        zoomTargetPosition.z = targetCamera.transform.position.z;

        if (swipeController != null && disableSwipeDuringZoom)
            swipeController.SetEnabled(false);
    }

    protected virtual void EndZoom()
    {
        if (!isZooming) return;

        isZooming = false;
        isReturning = true;

        if (swipeController != null && disableSwipeDuringZoom)
            swipeController.SetEnabled(true);
    }

    protected virtual void ProcessZoom()
    {
        // Плавное перемещение к цели
        float progress = Mathf.Clamp01(zoomSpeed * Time.deltaTime * 2f);
        targetCamera.transform.position = Vector3.Lerp(
            targetCamera.transform.position,
            zoomTargetPosition,
            progress
        );

        // Плавное изменение размера
        targetCamera.orthographicSize = Mathf.Lerp(
            targetCamera.orthographicSize,
            targetOrthoSize,
            progress
        );
    }

    protected virtual void ProcessReturn()
    {
        // Только изменение размера (позиция остается текущей)
        targetCamera.orthographicSize = Mathf.Lerp(
            targetCamera.orthographicSize,
            defaultOrthoSize,
            releaseSpeed * Time.deltaTime
        );

        // Автоматическая корректировка позиции для сохранения центра
        if (zoomStartSize > 0 && defaultOrthoSize > 0)
        {
            float scaleFactor = targetCamera.orthographicSize / zoomStartSize;
            Vector3 offset = (zoomStartPosition - zoomTargetPosition) * scaleFactor;
            targetCamera.transform.position = zoomTargetPosition + offset;
        }

        if (Mathf.Abs(targetCamera.orthographicSize - defaultOrthoSize) < 0.01f)
            isReturning = false;
    }
}