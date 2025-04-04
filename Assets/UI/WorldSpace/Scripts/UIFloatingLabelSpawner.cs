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

    private void Start()
    {
        _root = _uiDocument.rootVisualElement;
    }

    // Перегрузка 1: Позиция через Anchor + offset
    public void SpawnFloatingText(
        string text,
        LabelAnchor anchor,
        Vector2 offset = default,
        float duration = -1
    )
    {
        Vector2 screenPosition = GetAnchorPosition(anchor);
        SpawnFloatingText(text, screenPosition, anchor, offset, duration);
    }

    // Перегрузка 2: Позиция через Vector2 (игнорирует Anchor, но учитывает offset)
    public void SpawnFloatingText(
        string text,
        Vector2 screenPosition,
        Vector2 offset = default,
        float duration = -1
    )
    {
        if (duration <= 0) duration = _defaultDuration;

        var labelElement = _labelTemplate.CloneTree().Q<Label>();
        labelElement.text = text;

        // Позиция = screenPosition + offset (без привязки к краям)
        labelElement.style.position = Position.Absolute;
        labelElement.style.left = screenPosition.x + offset.x;
        labelElement.style.top = screenPosition.y + offset.y;

        _root.Add(labelElement);
        StartCoroutine(FloatAndFade(labelElement, duration));
    }

    // Внутренний метод для расчёта позиции с Anchor
    private void SpawnFloatingText(
        string text,
        Vector2 screenPosition,
        LabelAnchor anchor,
        Vector2 offset,
        float duration
    )
    {
        if (duration <= 0) duration = _defaultDuration;

        var labelElement = _labelTemplate.Instantiate().Q<Label>();
        labelElement.text = text;

        Vector2 finalPosition = CalculatePosition(screenPosition, anchor, offset, labelElement);
        labelElement.style.position = Position.Absolute;
        labelElement.style.left = finalPosition.x;
        labelElement.style.top = finalPosition.y;

        _root.Add(labelElement);
        StartCoroutine(FloatAndFade(labelElement, duration));
    }

    // Получаем позицию Anchor относительно корневого элемента (всего экрана)
    private Vector2 GetAnchorPosition(LabelAnchor anchor)
    {
        float rootWidth = _root.contentRect.width;
        float rootHeight = _root.contentRect.height;

        return anchor switch
        {
            LabelAnchor.TopLeft => new Vector2(0, 0),
            LabelAnchor.TopRight => new Vector2(rootWidth, 0),
            LabelAnchor.BottomLeft => new Vector2(0, rootHeight),
            LabelAnchor.BottomRight => new Vector2(rootWidth, rootHeight),
            LabelAnchor.Center => new Vector2(rootWidth / 2, rootHeight / 2),
            _ => Vector2.zero
        };
    }

    // Расчёт позиции с учётом Anchor и размера Label
    private Vector2 CalculatePosition(Vector2 screenPosition, LabelAnchor anchor, Vector2 offset, Label label)
    {
        float labelWidth = label.contentRect.width != label.contentRect.width ? 0f : label.contentRect.width;
        float labelHeight = label.contentRect.height != label.contentRect.height ? 0f : label.contentRect.height;

        return anchor switch
        {
            LabelAnchor.TopLeft => screenPosition + offset,
            LabelAnchor.TopRight => new Vector2(
                screenPosition.x - labelWidth + offset.x,
                screenPosition.y + offset.y
            ),
            LabelAnchor.BottomLeft => new Vector2(
                screenPosition.x + offset.x,
                screenPosition.y - labelHeight + offset.y
            ),
            LabelAnchor.BottomRight => new Vector2(
                screenPosition.x - labelWidth + offset.x,
                screenPosition.y - labelHeight + offset.y
            ),
            LabelAnchor.Center => new Vector2(
                screenPosition.x - (labelWidth * 0.5f) + offset.x,
                screenPosition.y - (labelHeight * 0.5f) + offset.y
            ),
            _ => screenPosition + offset
        };
    }

    // Анимация (без изменений)
    private IEnumerator FloatAndFade(Label label, float duration)
    {
        float elapsed = 0f;
        float startY = label.worldBound.y;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / duration;

            float newY = startY - (_floatSpeed * elapsed);
            label.style.top = newY;

            if (progress > 0.7f)
                label.AddToClassList("fade-out");

            yield return null;
        }

        label.RemoveFromHierarchy();
    }
}

public enum LabelAnchor
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center
}