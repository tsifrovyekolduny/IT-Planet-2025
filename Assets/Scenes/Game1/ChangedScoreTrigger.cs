using UnityEngine;
using UnityEngine.Events;

public class �hangedScoreTrigger : MonoBehaviour
{
    [Header("���������")]
    [Rename("������ ����")]
    public bool giveScoreOnZoom = true;
    [Rename("���������� �����")]
    public int scoreValue = 1;

    [Header("���������� �������")]
    [Rename("�������� ��������� ������")]
    [Tooltip("���� ��������, ����� �������� ChangeVisibility � TextHider")]
    public bool changeTextVisibility = true;

    [Header("�������")]
    public UnityEvent onScoreGiven;

    private bool scoreGiven = false;
    private TextHider _textHider;

    private void Start()
    {
        // ���� TextHider � �������� �������� (������� ����������)
        _textHider = GetComponentInChildren<TextHider>(true);
    }

    public void TryGiveScore()
    {
        if (changeTextVisibility && _textHider != null)
        {
            _textHider.ToggleVisibility(); // �������� �����
        }
        if (giveScoreOnZoom && !scoreGiven)
        {
            // ��� ����
            ScoreManager.Instance.AddScore(scoreValue);
            scoreGiven = true;
            onScoreGiven.Invoke();

            // ���� �������� ���������� ������� � TextHider ������
        }
    }

    public void ResetScoreState()
    {
        scoreGiven = false;
        // ������������� ����� �������� �����, ���� �����
    }
}