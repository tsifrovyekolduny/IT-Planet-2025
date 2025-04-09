using UnityEngine;
using UnityEngine.Events;

public class СhangedScoreTrigger : MonoBehaviour
{
    [Header("Настройки")]
    [Rename("Давать очки")]
    public bool giveScoreOnZoom = true;
    [Rename("Количество очков")]
    public int scoreValue = 1;

    [Header("Управление текстом")]
    [Rename("Изменять видимость текста")]
    [Tooltip("Если включено, будет вызывать ChangeVisibility у TextHider")]
    public bool changeTextVisibility = true;

    [Header("События")]
    public UnityEvent onScoreGiven;

    private bool scoreGiven = false;
    private TextHider _textHider;

    private void Start()
    {
        // Ищем TextHider в дочерних объектах (включая неактивные)
        _textHider = GetComponentInChildren<TextHider>(true);
    }

    public void TryGiveScore()
    {
        if (changeTextVisibility && _textHider != null)
        {
            _textHider.ToggleVisibility(); // Вызываем метод
        }
        if (giveScoreOnZoom && !scoreGiven)
        {
            // Даём очки
            ScoreManager.Instance.AddScore(scoreValue);
            scoreGiven = true;
            onScoreGiven.Invoke();

            // Если включено управление текстом и TextHider найден
        }
    }

    public void ResetScoreState()
    {
        scoreGiven = false;
        // Дополнительно можно сбросить текст, если нужно
    }
}