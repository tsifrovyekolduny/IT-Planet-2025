using UnityEngine;

public class ChoiceObjectClick : MonoBehaviour
{
    [HideInInspector] public BinaryChoiceSystem parentSystem;
    [HideInInspector] public bool isCorrectAnswer;

    private void Update()
    {
        // ��������� ���-�����
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckTouch(Input.GetTouch(0).position);
        }

        // ������������ ��������� ���� (��������� ��� ���������)
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            CheckTouch(Input.mousePosition);
        }
#endif
    }

    private void CheckTouch(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            parentSystem.HandleChoice(gameObject, isCorrectAnswer);
        }
    }

    // ������������ ����� ��������� ��� �������������
    private void OnMouseDown()
    {
        if (parentSystem != null)
        {
            parentSystem.HandleChoice(gameObject, isCorrectAnswer);
        }
    }
}