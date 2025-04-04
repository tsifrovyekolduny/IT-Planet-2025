using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class WithButtons : HideableUI
{
    [Header("Button Settings")]
    [SerializeField] private List<ButtonData> _buttonsData = new List<ButtonData>();
    [SerializeField] private Transform _buttonsContainer;    

    private CanvasGroup _buttonsCanvasGroup;    
    
    protected override void Start()
    {     
        _buttonsCanvasGroup = _buttonsContainer.GetComponent<CanvasGroup>();
        if (_buttonsCanvasGroup == null)
            _buttonsCanvasGroup = _buttonsContainer.gameObject.AddComponent<CanvasGroup>();

        _hidingCanvasGroup = _buttonsCanvasGroup;

        LinkButtons();        
        base.Start();
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

    public override void ApplyVisibility(bool show)
    {
        _buttonsCanvasGroup.alpha = show ? 1 : 0;
        _buttonsCanvasGroup.blocksRaycasts = show;
    }

    public override void ApplyVisibilityWithoutFade(bool show)
    {
        ApplyVisibility(show);
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
        var icon = linkedButton.GetComponent<Image>();
        if (icon != null)
        {            
            icon.sprite = buttonIcon;            
            icon.transform.localScale = Vector3.one * iconScale;
        }
        else
        {
            icon.color = buttonColor;
        }

        // Обработчик клика
        linkedButton.onClick.RemoveAllListeners();
        linkedButton.onClick.AddListener(() => onClick.Invoke());
    }
}