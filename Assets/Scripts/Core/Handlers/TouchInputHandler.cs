using UnityEngine;

using Assets.Scripts.Core.Interfaces;
using UnityEngine.Events;

public class TouchInputHandler : MonoBehaviour, ITouchInputHandler
{
    // ������� ��� ��������� �������
    private UnityEvent<Vector2> _onSingleTap = new UnityEvent<Vector2>();
    private UnityEvent _onDoubleTap = new UnityEvent();
    private UnityEvent<Vector2> _onSwipe = new UnityEvent<Vector2>();
    private UnityEvent<Vector2> _onTouchEnd = new UnityEvent<Vector2>();

    public UnityEvent<Vector2> OnSingleTap { get => _onSingleTap; set => _onSingleTap = value; }
    public UnityEvent OnDoubleTap { get => _onDoubleTap; set => _onDoubleTap = value; }
    public UnityEvent<Vector2> OnSwipe { get => _onSwipe; set => _onSwipe = value; }
    public UnityEvent<Vector2> OnTouchEnd { get => _onTouchEnd; set => _onTouchEnd = value; }

    private void Update()
    {
        // ��������� ���������� �������
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // ��������� ���������� �������
                    HandleSingleTap(touch);
                    break;

                case TouchPhase.Moved:
                    // ��������� ���������� ������� �� ������
                    HandleSwipe(touch);
                    break;

                case TouchPhase.Ended:
                    // ��������� ��������� �������
                    HandleTouchEnd(touch);
                    break;
            }

            // �������� �� ������� �������
            if (Input.touchCount == 2 && touch.phase == TouchPhase.Began)
            {
                HandleDoubleTap();
            }
        }
    }

    private void HandleSingleTap(Touch touch)
    {
        //Debug.Log("��������� ������� � �������: " + touch.position);
        OnSingleTap?.Invoke(touch.position); // �������� ������� ���������� �������
    }

    private void HandleDoubleTap()
    {
        //Debug.Log("������� �������!");
        OnDoubleTap?.Invoke(); // �������� ������� �������� �������
    }

    private void HandleSwipe(Touch touch)
    {
        //Debug.Log("���������� ������� �� ������ � �������: " + touch.position);
        OnSwipe?.Invoke(touch.position); // �������� ������� ���������� �������
    }

    private void HandleTouchEnd(Touch touch)
    {
        //Debug.Log("������� ��������� � �������: " + touch.position);
        OnTouchEnd?.Invoke(touch.position); // �������� ������� ���������� �������
    }
}