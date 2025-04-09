using UnityEngine;
using UnityEngine.Events;

public class ScoreTrigger : MonoBehaviour
{
    [Header("Настройки")]
    [Rename("Очки за объект")]
    [Tooltip("Давать ли очки при взаимодействии с объектом")]
    public bool giveScoreOnZoom = true;

    [Rename("Количество очков")]
    [Tooltip("Сколько очков давать при взаимодействии с объектом")]
    public int scoreValue = 1;

    [Header("События")]
    [Tooltip("Вызов дополнительных событий при начислении очков")]
    public UnityEvent onScoreGiven;

    protected bool scoreGiven = false;

    // Вызывается из ZoomController при приближении
    public virtual void TryGiveScore()
    {
        if (giveScoreOnZoom && !scoreGiven)
        {
            ScoreManager.Instance.AddScore(scoreValue);
            scoreGiven = true;
            onScoreGiven.Invoke();
        }
    }

    // Сброс при необходимости (например, при рестарте уровня)
    public void ResetScoreState()
    {
        scoreGiven = false;
    }
}