using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasGroup))]
public class PuzzlePiece : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Основные настройки")]
    [SerializeField] private float snapDistance = 50f;
    [SerializeField] private float snapAnimationDuration = 0.2f;

    [Header("Визуальные эффекты")]
    [SerializeField] private float dragAlpha = 0.8f;
    [SerializeField] private float scaleDuringDrag = 1.05f;

    public Vector2 CorrectPosition { get; set; }
    public bool IsInPlace { get; private set; }

    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;
        CorrectPosition = rectTransform.anchoredPosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsInPlace) return;

        isDragging = true;
        IsInPlace = false;
        canvasGroup.alpha = dragAlpha;
        canvasGroup.blocksRaycasts = false;
        transform.localScale = originalScale * scaleDuringDrag;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        // Получаем мировые координаты точки касания
        Vector3 worldPoint;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out worldPoint))
        {
            // Устанавливаем позицию объекта
            transform.position = worldPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;

        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        transform.localScale = originalScale;

        if (ShouldSnap())
        {
            StartCoroutine(SnapToPosition());
        }
    }

    private bool ShouldSnap()
    {
        return Vector2.Distance(rectTransform.anchoredPosition, CorrectPosition) < snapDistance;
    }

    private IEnumerator SnapToPosition()
    {
        IsInPlace = true;
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < snapAnimationDuration)
        {
            rectTransform.anchoredPosition = Vector2.Lerp(
                startPos,
                CorrectPosition,
                elapsed / snapAnimationDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = CorrectPosition;
        PuzzleManager.Instance.CheckPuzzleComplete();
    }

    public void ResetPiece()
    {
        IsInPlace = false;
        rectTransform.anchoredPosition = CorrectPosition;
    }
}