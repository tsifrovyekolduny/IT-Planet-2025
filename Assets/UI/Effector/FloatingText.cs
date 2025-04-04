using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI), typeof(CanvasGroup))]
public class FloatingText : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float _worldSpaceMoveSpeed = 0.5f;
    [SerializeField] private float _screenSpaceMoveSpeed = 50f;
    [SerializeField] private TextMeshProUGUI _text;

    private CanvasGroup _canvasGroup;
    private float _duration;
    private float _timer;
    private Vector3 _targetPosition;
    private bool _isWorldSpace;
    private Camera _mainCamera;
    private Vector2 _offset;
    private TextAnchor _anchor;

    private void Awake()
    {
        Debug.Log($"New Floating appeared: {_text}");

        _text = GetComponent<TextMeshProUGUI>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _mainCamera = Camera.main;
    }

    public TextMeshProUGUI TextLabel => _text;

    public void SetupWorldSpace(string message, Vector3 worldPosition,
                              float duration, Vector2 offset)
    {
        _text.text = message;
        _duration = duration;
        _timer = 0f;
        _canvasGroup.alpha = 1f;
        _isWorldSpace = true;
        _targetPosition = worldPosition;
        _offset = offset;

        // Настройки для мирового пространства
        transform.SetParent(_mainCamera.transform, true);
    }

    public void SetupScreenSpace(string message, Vector2 screenPosition,
                               float duration, TextAnchor anchor)
    {
        _text.text = message;
        _duration = duration;
        _timer = 0f;
        _canvasGroup.alpha = 1f;
        _isWorldSpace = false;
        _anchor = anchor;

        // Настройки для экранного пространства
        var rt = GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = GetAnchorForTextAnchor(anchor);
        rt.anchoredPosition = screenPosition;
    }

    private Vector2 GetAnchorForTextAnchor(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.UpperLeft: return new Vector2(0, 1);
            case TextAnchor.UpperCenter: return new Vector2(0.5f, 1);
            case TextAnchor.UpperRight: return new Vector2(1, 1);
            case TextAnchor.MiddleLeft: return new Vector2(0, 0.5f);
            case TextAnchor.MiddleRight: return new Vector2(1, 0.5f);
            case TextAnchor.LowerLeft: return new Vector2(0, 0);
            case TextAnchor.LowerCenter: return new Vector2(0.5f, 0);
            case TextAnchor.LowerRight: return new Vector2(1, 0);
            default: return new Vector2(0.5f, 0.5f);
        }
    }

    private void Update()
    {
        // Позиционирование
        if (_isWorldSpace)
        {
            if (_mainCamera != null)
            {
                transform.position = _mainCamera.WorldToScreenPoint(_targetPosition) + (Vector3)_offset;
                _targetPosition += Vector3.up * Time.deltaTime * _worldSpaceMoveSpeed;
            }
        }
        else
        {
            transform.localPosition += Vector3.up * Time.deltaTime * _screenSpaceMoveSpeed;
        }

        // Анимация исчезновения
        _timer += Time.deltaTime;
        float progress = Mathf.Clamp01(_timer / _duration);
        _canvasGroup.alpha = 1f - progress;

        // Уничтожение по завершении
        if (_timer >= _duration * Time.deltaTime)
        {
            // Destroy(gameObject);
        }
    }
}