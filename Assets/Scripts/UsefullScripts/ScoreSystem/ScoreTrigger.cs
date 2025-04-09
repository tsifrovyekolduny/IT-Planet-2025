using UnityEngine;
using UnityEngine.Events;

public class ScoreTrigger : MonoBehaviour
{
    [Header("���������")]
    [Rename("���� �� ������")]
    [Tooltip("������ �� ���� ��� �������������� � ��������")]
    public bool giveScoreOnZoom = true;

    [Rename("���������� �����")]
    [Tooltip("������� ����� ������ ��� �������������� � ��������")]
    public int scoreValue = 1;

    [Header("�������")]
    [Tooltip("����� �������������� ������� ��� ���������� �����")]
    public UnityEvent onScoreGiven;

    protected bool scoreGiven = false;

    // ���������� �� ZoomController ��� �����������
    public virtual void TryGiveScore()
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