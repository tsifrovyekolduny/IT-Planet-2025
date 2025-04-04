using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FloatingText : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float _duration = 1f;
    [SerializeField] private float _moveHeight = 100f;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    [SerializeField] private AnimationCurve _moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 _startPosition;
    private float _progress;

    void Update()
    {
        _progress += Time.deltaTime / _duration;

        // Анимация движения и прозрачности
        transform.position = _startPosition + Vector3.up * (_moveCurve.Evaluate(_progress) * _moveHeight);
        _canvasGroup.alpha = _fadeCurve.Evaluate(_progress);

        if (_progress >= 1f)
        {
            FloatingTextFactory.Instance.ReturnToPool(this);
        }
    }

    public void Initialize(string message, Vector3 position)
    {
        _text.text = message;
        _startPosition = position;
        _progress = 0f;
        _canvasGroup.alpha = 1f;
    }
}