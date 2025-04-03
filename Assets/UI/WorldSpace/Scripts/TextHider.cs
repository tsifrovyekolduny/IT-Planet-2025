using UnityEngine;
using TMPro;
using System.Collections;

public class TextHider : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private string _hiddenPattern = "━━━━━";
    [SerializeField] private float _fadeDuration = 0.3f;

    private string _originalText;
    private bool _isHidden = true;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        _originalText = _targetText.text;
    }

    // Основной метод переключения
    public void ToggleVisibility(bool show, bool instant = false)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        if (instant)
        {
            ApplyVisibility(show);
        }
        else
        {
            _fadeCoroutine = StartCoroutine(AnimateVisibility(show));
        }
    }

    public void ToggleVisibility()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(AnimateVisibility(!_isHidden));
    }

    // Показать текст (с анимацией)
    public void Show(bool instant = false) => ToggleVisibility(true, instant);

    // Скрыть текст (с анимацией)
    public void Hide(bool instant = false) => ToggleVisibility(false, instant);

    private IEnumerator AnimateVisibility(bool show)
    {
        _isHidden = show;

        float elapsed = 0f;
        float startAlpha = _targetText.alpha;
        float targetAlpha = show ? 1f : 0f;

        while (elapsed < _fadeDuration)
        {
            _targetText.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        ApplyVisibility(show);
    }

    private void ApplyVisibility(bool show)
    {
        _targetText.text = show ? _originalText : _hiddenPattern;
        _targetText.alpha = show ? 1f : 0f;
    }
}