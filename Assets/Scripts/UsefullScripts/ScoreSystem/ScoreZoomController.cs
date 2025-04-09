using UnityEngine;

public class ScoreZoomController : ZoomController
{
    [Header("��������� �����")]
    [Rename("������� �����")]
    public bool enableScoreSystem = true;
    private bool scoreGiven = false;

    protected override void StartZoom(Vector3 worldPosition, GameObject targetObject)
    {
        Debug.Log(scoreGiven);
        // ���� ���� ����� ������� ����
        if (enableScoreSystem && !scoreGiven)
        {
            var scoreTrigger = targetObject.GetComponent<ScoreTrigger>();
            Debug.Log(scoreTrigger);
            if (scoreTrigger != null)
            {
                scoreTrigger.TryGiveScore();
                scoreGiven = true;
            }
        }

        base.StartZoom(worldPosition, targetObject);
    }

    protected override void EndZoom()
    {
        base.EndZoom();
        scoreGiven = false; // ���������� ���� ��� ���������� ����
    }
}