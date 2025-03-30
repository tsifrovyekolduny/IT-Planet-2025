using UnityEngine;
using UnityEngine.Events;

public class ScoreTrigger : MonoBehaviour
{
    [Header("���������")]
    [Tooltip("������ �� ���� ��� ����������� � ����� �������")]
    public bool giveScoreOnZoom = true;
    [Tooltip("������� ����� ������")]
    public int scoreValue = 1;

    [Header("�������")]
    public UnityEvent onScoreGiven; // ��� ���������� ��������

    private bool scoreGiven = false;

    // ���������� �� ZoomController ��� �����������
    public void TryGiveScore()
    {
        if (giveScoreOnZoom && !scoreGiven)
        {
            ScoreManager.Instance.AddScore(scoreValue);
            scoreGiven = true;
            onScoreGiven.Invoke();
        }
    }

    // ����� ��� ������������� (��������, ��� �������� ������)
    public void ResetScoreState()
    {
        scoreGiven = false;
    }
}