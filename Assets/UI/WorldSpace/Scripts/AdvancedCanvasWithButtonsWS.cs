using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[ExecuteAlways]
public class WithButtons : AdvancedTMPElement
{
    [Header("Button Settings")]
    [SerializeField] private List<ButtonData> _buttonsData = new List<ButtonData>();
    [SerializeField] private Transform _buttonsContainer;

    [SerializeField] private float _fadeDuration = 0.3f;

    private CanvasGroup _buttonsCanvasGroup;

    [Header("Hider Settings")]
    [SerializeField] private TextHider _textHider;
    [SerializeField] private bool _startHidden = true;
    private bool _isHidden = false;

    private void Start()
    {       
        _buttonsCanvasGroup = _buttonsContainer.GetComponent<CanvasGroup>();
        if (_buttonsCanvasGroup == null)
            _buttonsCanvasGroup = _buttonsContainer.gameObject.AddComponent<CanvasGroup>();

        LinkButtons();

        if (_startHidden)
        {
            _isHidden = _startHidden;
            _textHider.ToggleVisibility(false, true);
            SetButtonsVisibility(false, true);
        }
    }

    // Связываем данные с реальными кнопками
    private void LinkButtons()
    {
        // Получаем все кнопки из контейнера
        Button[] buttonComponents = _buttonsContainer.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < _buttonsData.Count; i++)
        {
            if (i < buttonComponents.Length)
            {
                _buttonsData[i].linkedButton = buttonComponents[i];
                _buttonsData[i].ApplyToButton();
                buttonComponents[i].gameObject.SetActive(true);
            }
        }

        // Скрываем лишние кнопки
        for (int i = _buttonsData.Count; i < buttonComponents.Length; i++)
        {
            buttonComponents[i].gameObject.SetActive(false);
        }
    }

    // Обновляем все кнопки при изменениях в редакторе
    private void OnValidate()
    {
        if (!Application.isPlaying && _buttonsContainer != null)
        {
            LinkButtons();
        }
    }

    // Добавление новой кнопки
    public void AddButton(ButtonData data)
    {
        _buttonsData.Add(data);
        LinkButtons();
    }

    public void ToggleVisibility()
    {
        if (_isHidden)
        {
            ShowFullInfo();
        }
        else
        {
            HideFullInfo();
        }
    }

    public void ShowFullInfo()
    {
        _isHidden = false;

        _textHider.ToggleVisibility(false);
        SetButtonsVisibility(true);
    }

    public void HideFullInfo()
    {
        _isHidden = true;

        _textHider.ToggleVisibility(true);
        SetButtonsVisibility(false);
    }

    private void SetButtonsVisibility(bool show, bool instant = false)
    {
        if (instant)
        {
            _buttonsCanvasGroup.alpha = show ? 1 : 0;
            _buttonsCanvasGroup.blocksRaycasts = show;
        }
        else
        {
            StartCoroutine(FadeButtons(show));
        }
    }

    private IEnumerator FadeButtons(bool show)
    {
        float elapsed = 0f;
        float startAlpha = _buttonsCanvasGroup.alpha;
        float targetAlpha = show ? 1 : 0;

        _buttonsCanvasGroup.blocksRaycasts = show;

        while (elapsed < _fadeDuration)
        {
            _buttonsCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / _fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _buttonsCanvasGroup.alpha = targetAlpha;
    }
}

[System.Serializable]
public class ButtonData
{
    // Ссылка на реальную кнопку
    [HideInInspector] public Button linkedButton;

    // Настраиваемые параметры
    public string buttonText;
    public Sprite buttonIcon;
    public Color buttonColor = Color.white;
    [Range(0.5f, 2f)] public float iconScale = 1f;
    public UnityEvent onClick;

    // Метод для синхронизации данных с кнопкой
    public void ApplyToButton()
    {
        if (linkedButton == null) return;

        // Текст
        var tmp = linkedButton.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = buttonText;

        // Иконка
        var icon = linkedButton.transform.Find("Icon")?.GetComponent<Image>();
        if (icon != null)
        {
            icon.sprite = buttonIcon;
            icon.color = buttonColor;
            icon.transform.localScale = Vector3.one * iconScale;
        }

        // Обработчик клика
        linkedButton.onClick.RemoveAllListeners();
        linkedButton.onClick.AddListener(() => onClick.Invoke());
    }
}