using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class UIFloatingLabelSpawner : Singletone<UIFloatingLabelSpawner>
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private VisualTreeAsset _labelTemplate;
    [SerializeField] private float _defaultDuration = 2f;
    [SerializeField] private float _floatSpeed = 50f;

    private VisualElement _root;

    // Варианты выравнивания (аналогично TextAnchor)
    public enum LabelAnchor
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center
    }

    private void Awake()
    {
        _root = _uiDocument.rootVisualElement;
    }

    // Основной метод с поддержкой выравнивания и смещения
    public void SpawnFloatingText(
        string text,
        Vector2 screenPosition,
        LabelAnchor anchor = LabelAnchor.TopLeft,
        Vector2 offset = default,
        float duration = -1
    )
    {
        if (duration <= 0) duration = _defaultDuration;

        // Создаем Label из шаблона
        var labelElement = _labelTemplate.CloneTree().Q<Label>();
        labelElement.text = text;

        // Устанавливаем позицию с учетом выравнивания
        Vector2 finalPosition = CalculatePosition(screenPosition, anchor, offset, labelElement);
        labelElement.style.position = Position.Absolute;
        labelElement.style.left = finalPosition.x;
        labelElement.style.top = finalPosition.y;

        // Добавляем в UI
        _root.Add(labelElement);

        // Запускаем анимацию
        StartCoroutine(FloatAndFade(labelElement, duration));
    }

    // Рассчитываем позицию с учетом выравнивания и смещения
    private Vector2 CalculatePosition(Vector2 screenPosition, LabelAnchor anchor, Vector2 offset, Label label)
    {
        float labelWidth = label.contentRect.width;
        float labelHeight = label.contentRect.height;

        switch (anchor)
        {
            case LabelAnchor.TopLeft:
                return screenPosition + offset;

            case LabelAnchor.TopRight:
                return new Vector2(
                    screenPosition.x - labelWidth + offset.x,
                    screenPosition.y + offset.y
                );

            case LabelAnchor.BottomLeft:
                return new Vector2(
                    screenPosition.x + offset.x,
                    screenPosition.y - labelHeight + offset.y
                );

            case LabelAnchor.BottomRight:
                return new Vector2(
                    screenPosition.x - labelWidth + offset.x,
                    screenPosition.y - labelHeight + offset.y
                );

            case LabelAnchor.Center:
                return new Vector2(
                    screenPosition.x - (labelWidth * 0.5f) + offset.x,
                    screenPosition.y - (labelHeight * 0.5f) + offset.y
                );

            default:
                return screenPosition + offset;
        }
    }

    // Анимация движения вверх и исчезновения (без изменений)
    private IEnumerator FloatAndFade(Label label, float duration)
    {
        float elapsed = 0f;
        float startY = label.worldBound.y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            // Движение вверх
            float newY = startY - (_floatSpeed * elapsed);
            label.style.top = newY;

            // Плавное исчезновение
            if (progress > 0.7f)
            {
                label.AddToClassList("fade-out");
            }

            yield return null;
        }

        // Удаляем элемент после завершения
        label.RemoveFromHierarchy();
    }
}