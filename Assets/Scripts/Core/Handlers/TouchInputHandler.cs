using UnityEngine;

using Assets.Scripts.Core.Interfaces;
using UnityEngine.Events;

public class TouchInputHandler : MonoBehaviour, ITouchInputHandler
{
    // События для обработки касаний
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
        // Проверяем количество касаний
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Обработка одиночного нажатия
                    HandleSingleTap(touch);
                    break;

                case TouchPhase.Moved:
                    // Обработка проведения пальцем по экрану
                    HandleSwipe(touch);
                    break;

                case TouchPhase.Ended:
                    // Обработка окончания касания
                    HandleTouchEnd(touch);
                    break;
            }

            // Проверка на двойное нажатие
            if (Input.touchCount == 2 && touch.phase == TouchPhase.Began)
            {
                HandleDoubleTap();
            }
        }
    }

    private void HandleSingleTap(Touch touch)
    {
        //Debug.Log("Одиночное нажатие в позиции: " + touch.position);
        OnSingleTap?.Invoke(touch.position); // Вызываем событие одиночного нажатия
    }

    private void HandleDoubleTap()
    {
        //Debug.Log("Двойное нажатие!");
        OnDoubleTap?.Invoke(); // Вызываем событие двойного нажатия
    }

    private void HandleSwipe(Touch touch)
    {
        //Debug.Log("Проведение пальцем по экрану в позиции: " + touch.position);
        OnSwipe?.Invoke(touch.position); // Вызываем событие проведения пальцем
    }

    private void HandleTouchEnd(Touch touch)
    {
        //Debug.Log("Касание завершено в позиции: " + touch.position);
        OnTouchEnd?.Invoke(touch.position); // Вызываем событие завершения касания
    }
}