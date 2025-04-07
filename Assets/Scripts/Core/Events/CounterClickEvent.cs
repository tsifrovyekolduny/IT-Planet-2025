using Assets.Scripts.Core.Interfaces.Events;
using UnityEngine;
using UnityEngine.Events;

public class CounterClickEvent : ICounterClickEvent
{
    private int _countCurrent = 0; // Текущий счетчик кликов
    private int _countForEvent; // Количество кликов для срабатывания события
    private float _lastTimeClick; // Время последнего клика
    private float _timeThreshold; // Порог времени между кликами

    private UnityEvent _onCountReached; // Событие, которое будет вызвано при достижении количества кликов

    public UnityEvent OnCountReached { get => _onCountReached; set => _onCountReached = value; }

    // Конструктор
    public CounterClickEvent(int countForEvent, float timeThreshold)
    {
        _countForEvent = countForEvent;
        _timeThreshold = timeThreshold;
        OnCountReached = new UnityEvent();
    }


    // Метод для обработки клика
    public void Click()
    {
        float currentTime = Time.time; // Получаем текущее время

        // Проверяем, прошло ли достаточно времени с последнего клика
        if (currentTime - _lastTimeClick <= _timeThreshold)
        {
            _countCurrent++; // Увеличиваем счетчик кликов

            // Проверяем, достигли ли мы необходимого количества кликов
            if (_countCurrent >= _countForEvent)
            {
                OnCountReached.Invoke(); // Вызываем событие
                _countCurrent = 0; // Сбрасываем счетчик
            }
        }
        else
        {
            // Если время между кликами больше порога, сбрасываем счетчик
            _countCurrent = 1; // Начинаем новый счетчик
        }

        _lastTimeClick = currentTime; // Обновляем время последнего клика
    }
}