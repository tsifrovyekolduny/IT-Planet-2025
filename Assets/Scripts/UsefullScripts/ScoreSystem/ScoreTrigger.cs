using UnityEngine;
using UnityEngine.Events;

public class ScoreTrigger : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Давать ли очки при приближении к этому объекту")]
    public bool giveScoreOnZoom = true;
    [Tooltip("Сколько очков давать")]
    public int scoreValue = 1;

    [Header("События")]
    public UnityEvent onScoreGiven; // Для визуальных эффектов

    private bool scoreGiven = false;

    // Вызывается из ZoomController при приближении
    public void TryGiveScore()
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