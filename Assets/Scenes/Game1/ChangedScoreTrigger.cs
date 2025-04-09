using UnityEngine;
using UnityEngine.Events;

public class �hangedScoreTrigger : ScoreTrigger
{   
    [Header("���������� �������")]
    [Rename("�������� ��������� ������")]
    [Tooltip("���� ��������, ����� �������� ChangeVisibility � TextHider")]
    public bool changeTextVisibility = true;    
    
    private TextHider _textHider;

    private void Start()
    {
        // ���� TextHider � �������� �������� (������� ����������)
        _textHider = GetComponentInChildren<TextHider>(true);
    }

    public override void TryGiveScore()
    {
        Debug.Log("TryGive");
        if (changeTextVisibility && _textHider != null)
        {
            Debug.Log("Can hide");            
            _textHider.ToggleVisibility(true); // �������� �����
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
}