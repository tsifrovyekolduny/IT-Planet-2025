using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [Header("Компоненты")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;

    private float _duration;
    private float _timer;
    private Vector3 _targetPosition;
    private bool _isWorldSpace;
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    public void Setup(string message, Vector3 position, float duration, bool isWorldSpace)
    {
        _text.text = message;
        _duration = duration;
        _timer = 0f;
        _canvasGroup.alpha = 1f;
        _isWorldSpace = isWorldSpace;

        if (_isWorldSpace)
        {
            _targetPosition = position;
        }
        else
        {
            // Для экранного пространства сразу конвертируем в локальные координаты
            transform.localPosition = position;
        }
    }

    private void Update()
    {
        // Позиционирование
        if (_isWorldSpace && _mainCamera != null)
        {
            transform.position = _mainCamera.WorldToScreenPoint(_targetPosition);
            _targetPosition += Vector3.up * Time.deltaTime * 0.5f;
        }
        else
        {
            // Для экранного пространства просто двигаем вверх
            transform.localPosition += Vector3.up * Time.deltaTime * 50f;
        }

        // Анимация исчезновения
        _timer += Time.deltaTime;
        float progress = Mathf.Clamp01(_timer / _duration);
        _canvasGroup.alpha = 1f - progress;

        // Уничтожение по завершении
        if (_timer >= _duration)
        {
            Destroy(gameObject);
        }
    }
}