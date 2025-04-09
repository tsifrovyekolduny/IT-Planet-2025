using UnityEngine;
using UnityEngine.Events;

public class СhangedScoreTrigger : ScoreTrigger
{   
    [Header("Управление текстом")]
    [Rename("Изменять видимость текста")]
    [Tooltip("Если включено, будет вызывать ChangeVisibility у TextHider")]
    public bool changeTextVisibility = true;    
    
    private TextHider _textHider;

    private void Start()
    {
        // Ищем TextHider в дочерних объектах (включая неактивные)
        _textHider = GetComponentInChildren<TextHider>(true);
    }

    public override void TryGiveScore()
    {
        Debug.Log("TryGive");
        if (changeTextVisibility && _textHider != null)
        {
            Debug.Log("Can hide");            
            _textHider.ToggleVisibility(true); // Вызываем метод
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
}