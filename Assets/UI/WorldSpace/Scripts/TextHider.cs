using UnityEngine;
using TMPro;
using System.Collections;

public class TextHider : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private string _hiddenPattern = "━━━━━";
    [SerializeField] private float _fadeDuration = 0.3f;
    [SerializeField] private bool _withFade = false;

    private string _originalText;
    [SerializeField] private bool _startHidden = true;
    private bool _isHidden = false;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        if (_startHidden)
        {
            Hide(true);
        }
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
        else if (_withFade)
        {
            _fadeCoroutine = StartCoroutine(AnimateVisibility(show));
        }
        else
        {
            EncryptText();
        }
    }

    public void ToggleVisibility()
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        if (_withFade)
        {
            _fadeCoroutine = StartCoroutine(AnimateVisibility(!_isHidden));
        }
        else
        {
            EncryptText();
        }
    }

    void EncryptText()
    {
        if (_isHidden)
        {
            _targetText.text = _originalText;
        }
        else
        {
            _targetText.text = _hiddenPattern;
        }

        _isHidden = !_isHidden;
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
        _targetText.alpha = show ? 1f : 0f;
    }
}