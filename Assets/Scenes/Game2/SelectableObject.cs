using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public SequenceSelectionGame gameManager;

    private void Start()
    {
        var touchInputHandler = gameObject.GetComponent<TouchInputHandler>() ?? gameObject.AddComponent<TouchInputHandler>();
        touchInputHandler.OnSingleTap.AddListener(CheckTouch);
    }

    private void CheckTouch(Vector2 touchPosition)
    {
        // ����������� ������� ������� � ������� ����������
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touchPosition);

        // ���������, ���� �� ������� �� ����� �������
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            if (gameManager != null)
            {
                gameManager.OnObjectSelected(gameObject);
            }
        }
    }
}