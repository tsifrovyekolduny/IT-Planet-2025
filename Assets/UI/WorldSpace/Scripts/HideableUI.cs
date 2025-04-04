using UnityEngine;
using System.Collections;

public abstract class HideableUI : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float _fadeDuration = 0.3f;
    [SerializeField] protected bool _startHidden = true;
    [SerializeField] protected bool _withFade = true;
    [SerializeField] protected CanvasGroup _hidingCanvasGroup;

    protected bool _isHidden;
    protected Coroutine _fadeCoroutine;

    protected virtual void Start()
    {
        if (_startHidden)
        {
            if (_withFade)
            {
                ApplyVisibility(false);
            }
            else
            {
                ApplyVisibilityWithoutFade(false);
            }            
            _isHidden = true;
        }
        else
        {
            _isHidden = false;
        }
    }

    // Основные методы, которые должны быть реализованы
    public virtual void ToggleVisibility(bool show, bool instant = false)
    {
        // Заканчиваем затухание
        StopCurrentFade();
        
        // Сразу и без затухания
        if (instant && !_withFade)
        {
            ApplyVisibility(show);
        }
        // Не сразу и без затухания
        else if(!instant && !_withFade)
        {
            ApplyVisibilityWithoutFade(show);
        }
        // С затуханием
        else
        {
            _fadeCoroutine = StartCoroutine(FadeAnimation(show));
        }

        _isHidden = !show;
    }
    public virtual void ToggleVisibility() => ToggleVisibility(_isHidden);

    public abstract void ApplyVisibility(bool show);
    public abstract void ApplyVisibilityWithoutFade(bool show);    

    // Общие методы для анимации
    protected IEnumerator FadeAnimation(bool show)
    {
        float elapsed = 0f;
        float startAlpha = _hidingCanvasGroup.alpha;
        float targetAlpha = show ? 1f : 0f;

        _hidingCanvasGroup.blocksRaycasts = show;

        while (elapsed < _fadeDuration)
        {
            _hidingCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _hidingCanvasGroup.alpha = targetAlpha;
    }

    protected void StopCurrentFade()
    {
        if (_fadeCoroutine != null)
        {
            StopCoroutine(_fadeCoroutine);
            _fadeCoroutine = null;
        }
    }
}