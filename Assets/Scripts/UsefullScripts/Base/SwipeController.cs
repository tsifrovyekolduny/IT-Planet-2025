using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [Header("��������� ������")]
    [Rename("���������������� ������")]
    public float swipeSensitivity = 1f;
    [Rename("��������� ������")]
    public float swipeSmoothness = 10f;
    [Rename("�������� ������")]
    [Tooltip("���� ��������, ������ ����� �������� � �������� �����������")]
    public bool reverseControls = false;

    [Header("������� �����������")]
    [Rename("������� ������� (������)")]
    [Tooltip("������, ���� �������� ������ �� ����������")]
    public Transform upperBoundaryObject;
    [Rename("������ ������� (������)")]
    [Tooltip("������, ���� �������� ������ �� ���������")]
    public Transform lowerBoundaryObject;

    [Header("��������� ������")]
    [Rename("������")]
    public Camera targetCamera;

    [Header("Sensitivity Settings")]
    [Rename("��������� ���������������� ������")]
    [Range(0.01f, 1f)]
    public float swipeSensitivityMultiplier = 0.5f; // ��������� ����� ����������������
    [Rename("")]
    [Range(0.1f, 2f)]
    public float swipeSpeedDamping = 0.8f;

    protected float returnProgress; // ������ ��������� ��������

    private float currentYPosition; // ������� ������� ������ �� Y
    private float targetYPosition; // ������� ������� ����� ������
    private Vector2 touchStartPos;
    private bool isEnabled = true;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        // ��������� ������� ������ � �������, �� ������������ ���������
        currentYPosition = targetCamera.transform.position.y;
        targetYPosition = currentYPosition;

        if (upperBoundaryObject == null || lowerBoundaryObject == null)
        {
            Debug.LogWarning("������� �� ���������! ������ ����� ��������� ��� �����������.");
        }
    }

    private void LateUpdate()
    {
        if (!isEnabled) return;

        HandleInput();
        ApplyMovement();
    }

    private void HandleInput()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                float deltaY = (touch.position.y - touchStartPos.y) * swipeSensitivity;
                if (reverseControls) deltaY *= -1;

                targetYPosition += deltaY;
                ClampTargetPosition(); // ������������ ������� ������� ���������
                touchStartPos = touch.position;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
            }
            else
            {
                float deltaY = (Input.mousePosition.y - touchStartPos.y) * swipeSensitivity;
                if (reverseControls) deltaY *= -1;

                targetYPosition += deltaY;
                ClampTargetPosition(); // ������������ ������� ������� ���������
                touchStartPos = Input.mousePosition;
            }
        }
    }

    private void ClampTargetPosition()
    {
        if (upperBoundaryObject != null && lowerBoundaryObject != null)
        {
            targetYPosition = Mathf.Clamp(
                targetYPosition,
                lowerBoundaryObject.position.y,
                upperBoundaryObject.position.y
            );
        }
    }

    private void ApplyMovement()
    {
        // ������� ����������� � ������� �������
        currentYPosition = Mathf.Lerp(
            currentYPosition,
            targetYPosition,
            swipeSmoothness * Time.deltaTime
        );

        // ��������� ������� ������ (��������� X � Z)
        Vector3 newPos = targetCamera.transform.position;
        newPos.y = currentYPosition;
        targetCamera.transform.position = newPos;
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
    }
}